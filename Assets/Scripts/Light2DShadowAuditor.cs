using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Profiling;
using UnityEngine.Rendering.Universal;
using Utilities;

/// <summary>
/// Light2D / shadow performance auditor (URP 2D).
///
/// WHAT IT DOES — press F3 in a Play session or build:
///   1. AUDIT  — lists every Light2D in the loaded scenes with the settings that
///               drive cost: type, shadows on/off, # target sorting layers, blend style, intensity.
///   2. SWEEP  — turns each shadow-casting light's shadows OFF one at a time and measures
///               the SetPass drop, ranking which light's shadows cost the most.
///   3. CASTERS— counts ShadowCaster2D objects (the things lights re-render per layer).
///   Writes light2d_shadow_audit.txt (Application.persistentDataPath) and logs to console.
///
/// WHY IT MATTERS — in URP 2D the shadow cost is roughly:
///       (shadow-casting lights) x (sorting layers each targets) x (caster geometry).
///   The sweep tells you which lights to cut shadows on, and the layer count tells you
///   which lights to narrow. Use it before AND after a change (same scene/moment) to compare.
///
/// SETUP — drop this file anywhere in Assets/. It auto-spawns; no scene wiring needed.
///   Requires the Input System package (uses Keyboard.current). Change RUN_KEY if F3 is taken.
///   This is a DEBUG tool — remove (or wrap in #if DEVELOPMENT_BUILD) before shipping.
///
/// NOTE — SetPass is read via ProfilerRecorder. On some platforms "Render Thread" / "GPU Frame Time"
///   recorders read -1 (not exposed in player); SetPass count is reliable and is what this uses.
/// </summary>
public class Light2DShadowAuditor : SingletonMonoBehaviourAcrossScenes<Light2DShadowAuditor>
{
    private const Key RUN_KEY = Key.F3;
    private const int SettleFrames = 3;    // frames to wait after a toggle before sampling
    private const int SampleFrames = 10;   // frames averaged per measurement

    private bool _running;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        var go = new GameObject("[Light2DShadowAuditor]");
        DontDestroyOnLoad(go);
        go.AddComponent<Light2DShadowAuditor>();
    }

    private void Update()
    {
        if (_running || Keyboard.current == null) return;
        if (Keyboard.current[RUN_KEY].wasPressedThisFrame)
            StartCoroutine(Run());
    }

    // ---- reflection helpers (these Light2D fields are internal) ----
    private static int SortingLayerCount(Light2D l)
    {
        var f = typeof(Light2D).GetField("m_ApplyToSortingLayers", BindingFlags.NonPublic | BindingFlags.Instance);
        var arr = f?.GetValue(l) as int[];
        return arr?.Length ?? -1;
    }

    private static int BlendStyleIndex(Light2D l)
    {
        var f = typeof(Light2D).GetField("m_BlendStyleIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        return f != null ? (int)f.GetValue(l) : -1;
    }

    private static string Path(Transform t)
    {
        var sb = new StringBuilder(t.name);
        for (var p = t.parent; p != null; p = p.parent) sb.Insert(0, p.name + "/");
        return sb.ToString();
    }

    private IEnumerator MeasureSetPass(ProfilerRecorder rec, System.Action<double> onResult)
    {
        for (int i = 0; i < SettleFrames; i++) yield return new WaitForEndOfFrame();
        double sum = 0; int n = 0;
        for (int i = 0; i < SampleFrames; i++)
        {
            yield return new WaitForEndOfFrame();
            if (rec.Valid) { sum += rec.LastValue; n++; }
        }
        onResult(n > 0 ? sum / n : -1);
    }

    private IEnumerator Run()
    {
        _running = true;

        var all = FindObjectsByType<Light2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var shadowLights = new List<Light2D>();
        foreach (var l in all) if (l.shadowsEnabled) shadowLights.Add(l);

        var rec = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");

        // --- baseline + per-light sweep ---
        double baseline = 0;
        yield return MeasureSetPass(rec, v => baseline = v);

        var cost = new List<(Light2D light, double drop)>();
        foreach (var l in shadowLights)
        {
            l.shadowsEnabled = false;
            double without = 0;
            yield return MeasureSetPass(rec, v => without = v);
            l.shadowsEnabled = true;
            cost.Add((l, baseline - without));
        }

        foreach (var l in shadowLights) l.shadowsEnabled = false;
        double allOff = 0;
        yield return MeasureSetPass(rec, v => allOff = v);
        foreach (var l in shadowLights) l.shadowsEnabled = true;

        rec.Dispose();
        cost.Sort((a, b) => b.drop.CompareTo(a.drop));

        // --- caster count ---
        int casters = 0;
        var behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var b in behaviours) if (b is ShadowCaster2D) casters++;

        // --- build report ---
        var sb = new StringBuilder();
        sb.AppendLine("=== LIGHT2D / SHADOW AUDIT ===");
        sb.AppendLine($"Timestamp: {System.DateTime.Now}");
        sb.AppendLine($"Total Light2D: {all.Length}   Shadow-casting: {shadowLights.Count}   ShadowCaster2D objects: {casters}");
        sb.AppendLine($"SetPass baseline (all shadows ON):  {baseline:F0}");
        sb.AppendLine($"SetPass (all shadows OFF):          {allOff:F0}");
        sb.AppendLine($"Total shadow SetPass cost:          {baseline - allOff:F0}");
        sb.AppendLine();

        sb.AppendLine("--- SHADOW COST RANKED (SetPass drop when this light's shadows turn off) ---");
        sb.AppendLine($"{"SetPass",8}  {"Layers",6}  {"Blend",5}  {"Intens",7}  Light");
        foreach (var c in cost)
            sb.AppendLine($"{c.drop,8:F0}  {SortingLayerCount(c.light),6}  {BlendStyleIndex(c.light),5}  {c.light.intensity,7:F2}  {Path(c.light.transform)}");
        sb.AppendLine();

        sb.AppendLine("--- ALL LIGHT2D ---");
        sb.AppendLine($"{"Shadows",7}  {"Type",10}  {"Layers",6}  {"Blend",5}  {"Intens",7}  Light");
        foreach (var l in all)
            sb.AppendLine($"{(l.shadowsEnabled ? "ON" : "off"),7}  {l.lightType,10}  {SortingLayerCount(l),6}  {BlendStyleIndex(l),5}  {l.intensity,7:F2}  {Path(l.transform)}");
        sb.AppendLine();
        sb.AppendLine("READING IT:");
        sb.AppendLine("- High SetPass + many Layers = prime target. Cut its shadows or narrow its Target Sorting Layers.");
        sb.AppendLine("- Run the same scene/moment before & after a change and compare 'Total shadow SetPass cost'.");

        string report = sb.ToString();
        Debug.Log(report);
        string path = System.IO.Path.Combine(Application.persistentDataPath, "light2d_shadow_audit.txt");
        File.WriteAllText(path, report);
        Debug.Log($"[Light2DShadowAuditor] saved to: {path}");

        _running = false;
    }
}
