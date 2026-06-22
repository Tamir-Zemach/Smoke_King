using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class PerformanceOverlay : SingletonMonoBehaviourAcrossScenes<PerformanceOverlay>
{
    [Header("Controls")]
    public KeyCode ToggleKey = KeyCode.F1;
    public KeyCode SaveKey  = KeyCode.F2;
    public bool Visible = true;

    [Header("Thresholds")]
    public float SpikeThresholdMs = 25f;
    public float TargetFps = 60f;

    // ── Frame timing ──────────────────────────────────────────────────────────
    private int   _frames;
    private float _timer;
    private float _fps;
    private float _avgMs;
    private float _lastFrameMs;
    private float _minFps = float.MaxValue;
    private float _maxFps;

    // Ring buffer – last 300 frames (~5 s at 60 fps)
    private const int HistorySize = 300;
    private float[] _frameTimeHistory = new float[HistorySize];
    private int  _historyHead;
    private int  _historyCount;

    // Cached percentiles (recomputed every second)
    private float _p50, _p95, _p99, _pWorst;

    // ── GPU timing ────────────────────────────────────────────────────────────
    private FrameTiming[] _frameTimings = new FrameTiming[1];
    private float _gpuFrameMs;

    // ── Spikes ────────────────────────────────────────────────────────────────
    private List<float> _recentSpikes = new List<float>();
    private const int MaxRecentSpikes = 100;
    private int _totalSpikeCount;

    // ── GC ───────────────────────────────────────────────────────────────────
    private long _lastMemory;
    private long _gcAllocThisFrame;
    private long _totalGcAlloc;

    // ── Scene tracking ────────────────────────────────────────────────────────
    private string _currentScene;

    private class SceneStats
    {
        public string Name;
        public float MinFps = float.MaxValue;
        public float MaxFps;
        public double TotalFps;
        public int Samples;
        public int SpikeCount;
        public float WorstSpike;
    }
    private Dictionary<string, SceneStats> _sceneStats = new Dictionary<string, SceneStats>();

    // ── Cached scene-object counts (refreshed every 0.5 s) ───────────────────
    private float _cacheTimer;
    private const float CacheInterval = 0.5f;
    private int _cachedParticleCount;
    private int _cachedParticleSystems;
    private int _cachedLights;
    private int _cachedAudio;

    // ── GUI ───────────────────────────────────────────────────────────────────
    private GUIStyle _styleNormal;
    private GUIStyle _styleWarn;
    private GUIStyle _styleAlert;
    private StringBuilder _sb = new StringBuilder(512);

    // ─────────────────────────────────────────────────────────────────────────

    void Start()
    {
        _lastMemory   = System.GC.GetTotalMemory(false);
        _currentScene = SceneManager.GetActiveScene().name;

        _styleNormal = new GUIStyle { fontSize = 19, fontStyle = FontStyle.Bold };
        _styleNormal.normal.textColor = Color.white;

        _styleWarn  = new GUIStyle(_styleNormal);
        _styleWarn.normal.textColor  = Color.yellow;

        _styleAlert = new GUIStyle(_styleNormal);
        _styleAlert.normal.textColor = Color.red;

        FrameTimingManager.CaptureFrameTimings();
        SceneManager.sceneLoaded += (s, _) => _currentScene = s.name;
    }

    void Update()
    {
        HandleInput();
        TrackFrameTime();
        TrackGPU();
        TrackGC();
        RefreshCachedCounts();
        UpdateSceneStats();
    }

    // ── Input ─────────────────────────────────────────────────────────────────

    void HandleInput()
    {
        if (Keyboard.current[ToKey(ToggleKey)].wasPressedThisFrame) Visible = !Visible;
        if (Keyboard.current[ToKey(SaveKey)].wasPressedThisFrame)   SaveReport();
    }

    // ── Per-frame tracking ────────────────────────────────────────────────────

    void TrackFrameTime()
    {
        _lastFrameMs = Time.unscaledDeltaTime * 1000f;

        _frameTimeHistory[_historyHead] = _lastFrameMs;
        _historyHead = (_historyHead + 1) % HistorySize;
        if (_historyCount < HistorySize) _historyCount++;

        _frames++;
        _timer += Time.unscaledDeltaTime;

        if (_timer >= 1f)
        {
            _fps   = _frames / _timer;
            _avgMs = (_timer / _frames) * 1000f;
            _frames = 0;
            _timer  = 0f;

            if (_fps < _minFps) _minFps = _fps;
            if (_fps > _maxFps) _maxFps = _fps;

            ComputePercentiles();
        }

        if (_lastFrameMs > SpikeThresholdMs)
        {
            _totalSpikeCount++;
            _recentSpikes.Add(_lastFrameMs);
            if (_recentSpikes.Count > MaxRecentSpikes)
                _recentSpikes.RemoveAt(0);
        }
    }

    void TrackGPU()
    {
        FrameTimingManager.CaptureFrameTimings();
        if (FrameTimingManager.GetLatestTimings(1, _frameTimings) > 0)
            _gpuFrameMs = (float)_frameTimings[0].gpuFrameTime;
    }

    void TrackGC()
    {
        long mem = System.GC.GetTotalMemory(false);
        _gcAllocThisFrame = mem > _lastMemory ? mem - _lastMemory : 0;
        if (_gcAllocThisFrame > 0) _totalGcAlloc += _gcAllocThisFrame;
        _lastMemory = mem;
    }

    void RefreshCachedCounts()
    {
        _cacheTimer += Time.unscaledDeltaTime;
        if (_cacheTimer < CacheInterval) return;
        _cacheTimer = 0f;

        var pSystems = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        _cachedParticleSystems = 0;
        _cachedParticleCount   = 0;
        foreach (var ps in pSystems)
        {
            _cachedParticleCount += ps.particleCount;
            if (ps.isPlaying || ps.isEmitting) _cachedParticleSystems++;
        }

        _cachedLights = FindObjectsByType<Light>(FindObjectsSortMode.None)
                            .Count(l => l.isActiveAndEnabled);
        _cachedAudio  = FindObjectsByType<AudioSource>(FindObjectsSortMode.None)
                            .Count(a => a.isPlaying);
    }

    void UpdateSceneStats()
    {
        if (_fps <= 0) return;
        if (!_sceneStats.TryGetValue(_currentScene, out var s))
        {
            s = new SceneStats { Name = _currentScene };
            _sceneStats[_currentScene] = s;
        }
        s.TotalFps += _fps;
        s.Samples++;
        if (_fps < s.MinFps) s.MinFps = _fps;
        if (_fps > s.MaxFps) s.MaxFps = _fps;
        if (_lastFrameMs > SpikeThresholdMs)
        {
            s.SpikeCount++;
            if (_lastFrameMs > s.WorstSpike) s.WorstSpike = _lastFrameMs;
        }
    }

    // ── Percentiles ───────────────────────────────────────────────────────────

    void ComputePercentiles()
    {
        if (_historyCount == 0) return;
        var sorted = new float[_historyCount];
        for (int i = 0; i < _historyCount; i++) sorted[i] = _frameTimeHistory[i];
        System.Array.Sort(sorted);
        _p50    = sorted[Idx(sorted, 0.50f)];
        _p95    = sorted[Idx(sorted, 0.95f)];
        _p99    = sorted[Idx(sorted, 0.99f)];
        _pWorst = sorted[_historyCount - 1];
    }

    int Idx(float[] arr, float pct) =>
        Mathf.Clamp((int)(pct * arr.Length), 0, arr.Length - 1);

    // ── OnGUI ─────────────────────────────────────────────────────────────────

    void OnGUI()
    {
        if (!Visible) return;

        bool isSpike = _lastFrameMs > SpikeThresholdMs;
        bool isWarn  = _fps > 0 && _fps < TargetFps;

        _sb.Clear();
        _sb.AppendLine($"FPS  {_fps:F0}   [{_minFps:F0}–{_maxFps:F0}]");
        _sb.AppendLine($"CPU  {_avgMs:F1} ms    GPU  {_gpuFrameMs:F1} ms");
        _sb.AppendLine($"P50 {_p50:F1}  P95 {_p95:F1}  P99 {_p99:F1}  Worst {_pWorst:F1} ms");

        if (isSpike)
            _sb.AppendLine($"!! SPIKE  {_lastFrameMs:F1} ms !!");

        if (_gcAllocThisFrame > 0)
            _sb.AppendLine($"GC  {_gcAllocThisFrame / 1024f:F1} KB this frame");

        _sb.AppendLine();
        _sb.AppendLine($"Scene   {_currentScene}");
        _sb.AppendLine($"Particles  {_cachedParticleCount}  ({_cachedParticleSystems} systems)");
        _sb.AppendLine($"Lights     {_cachedLights}");
        _sb.AppendLine($"Audio      {_cachedAudio} playing");
        _sb.AppendLine();
        _sb.AppendLine($"Mono   {Profiler.GetMonoUsedSizeLong() / 1048576f:F1} / {Profiler.GetMonoHeapSizeLong() / 1048576f:F1} MB");
        _sb.AppendLine($"Unity  {Profiler.GetTotalAllocatedMemoryLong() / 1048576f:F1} MB");
        _sb.AppendLine($"GPU    {Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576f:F1} MB");
        _sb.AppendLine();
        _sb.AppendLine($"Spikes {_totalSpikeCount}   F2 = save report");

        var style = isSpike ? _styleAlert : (isWarn ? _styleWarn : _styleNormal);

        GUI.Box(new Rect(5, 5, 340, 370), GUIContent.none);
        GUI.Label(new Rect(14, 10, 330, 360), _sb.ToString(), style);

        DrawFrameGraph(new Rect(5, 380, 340, 55));
    }

    void DrawFrameGraph(Rect rect)
    {
        GUI.Box(rect, GUIContent.none);
        int n = Mathf.Min(_historyCount, 120);
        if (n < 2) return;

        float targetMs = 1000f / Mathf.Max(TargetFps, 1f);
        float barW     = rect.width / n;

        for (int i = 0; i < n; i++)
        {
            int idx = (_historyHead - n + i + HistorySize) % HistorySize;
            float ms = _frameTimeHistory[idx];
            float h  = Mathf.Clamp01(ms / 50f) * rect.height;

            GUI.color = ms > SpikeThresholdMs ? Color.red
                      : ms > targetMs         ? Color.yellow
                                              : Color.green;

            GUI.DrawTexture(
                new Rect(rect.x + i * barW, rect.y + rect.height - h, Mathf.Max(barW - 1f, 1f), h),
                Texture2D.whiteTexture);
        }

        GUI.color = Color.white;
    }

    // ── Report ────────────────────────────────────────────────────────────────

    void OnApplicationQuit() => SaveReport();

    void SaveReport()
    {
        var r = new StringBuilder();
        r.AppendLine("=== SMOKE KING PERFORMANCE REPORT ===");
        r.AppendLine($"Time:  {System.DateTime.Now}");
        r.AppendLine($"Scene: {_currentScene}");
        r.AppendLine();

        r.AppendLine("=== HARDWARE ===");
        r.AppendLine($"CPU:  {SystemInfo.processorType} ({SystemInfo.processorCount} cores)");
        r.AppendLine($"GPU:  {SystemInfo.graphicsDeviceName} ({SystemInfo.graphicsMemorySize} MB VRAM)");
        r.AppendLine($"RAM:  {SystemInfo.systemMemorySize} MB");
        r.AppendLine($"Res:  {Screen.currentResolution.width}x{Screen.currentResolution.height} @ {Screen.currentResolution.refreshRateRatio.value:F0} Hz");
        r.AppendLine($"Fullscreen: {Screen.fullScreen}");
        r.AppendLine();

        r.AppendLine("=== FRAME STATS ===");
        r.AppendLine($"FPS (current): {_fps:F1}");
        r.AppendLine($"FPS min/max:   {(_minFps == float.MaxValue ? 0 : _minFps):F1} / {_maxFps:F1}");
        r.AppendLine($"Avg CPU frame: {_avgMs:F2} ms");
        r.AppendLine($"GPU frame:     {_gpuFrameMs:F2} ms");
        r.AppendLine();

        r.AppendLine("=== PERCENTILES (last 300 frames) ===");
        r.AppendLine($"P50:   {_p50:F2} ms");
        r.AppendLine($"P95:   {_p95:F2} ms");
        r.AppendLine($"P99:   {_p99:F2} ms");
        r.AppendLine($"Worst: {_pWorst:F2} ms");
        r.AppendLine();

        r.AppendLine("=== SPIKES ===");
        r.AppendLine($"Threshold: {SpikeThresholdMs} ms");
        r.AppendLine($"Total:     {_totalSpikeCount}");
        if (_recentSpikes.Count > 0)
        {
            r.AppendLine($"Worst:  {_recentSpikes.Max():F1} ms");
            r.AppendLine($"Avg:    {_recentSpikes.Average():F1} ms");
            r.AppendLine("Recent: " + string.Join(", ", _recentSpikes.Select(s => s.ToString("F1"))));
        }
        else
        {
            r.AppendLine("None recorded.");
        }
        r.AppendLine();

        r.AppendLine("=== MEMORY ===");
        r.AppendLine($"Mono used/heap: {Profiler.GetMonoUsedSizeLong() / 1048576f:F2} / {Profiler.GetMonoHeapSizeLong() / 1048576f:F2} MB");
        r.AppendLine($"Unity alloc:    {Profiler.GetTotalAllocatedMemoryLong() / 1048576f:F2} MB");
        r.AppendLine($"Unity reserved: {Profiler.GetTotalReservedMemoryLong() / 1048576f:F2} MB");
        r.AppendLine($"GPU driver:     {Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576f:F2} MB");
        r.AppendLine($"Total GC alloc: {_totalGcAlloc / 1048576f:F3} MB");
        r.AppendLine();

        r.AppendLine("=== SCENE OBJECTS ===");
        r.AppendLine($"Particle systems (active): {_cachedParticleSystems}");
        r.AppendLine($"Particles (active):        {_cachedParticleCount}");
        r.AppendLine($"Lights (active):           {_cachedLights}");
        r.AppendLine($"Audio sources (playing):   {_cachedAudio}");
        r.AppendLine($"GameObjects:               {FindObjectsByType<GameObject>(FindObjectsSortMode.None).Length}");
        r.AppendLine($"Cameras:                   {FindObjectsByType<Camera>(FindObjectsSortMode.None).Length}");
        r.AppendLine();

        r.AppendLine("=== PER-SCENE BREAKDOWN ===");
        foreach (var kv in _sceneStats)
        {
            var s   = kv.Value;
            float avg = s.Samples > 0 ? (float)(s.TotalFps / s.Samples) : 0f;
            r.AppendLine($"[{s.Name}]");
            r.AppendLine($"  Avg {avg:F1} fps  |  Min {(s.MinFps == float.MaxValue ? 0 : s.MinFps):F1}  Max {s.MaxFps:F1}");
            r.AppendLine($"  Spikes {s.SpikeCount}  |  Worst {s.WorstSpike:F1} ms");
        }

        string folder = Path.Combine(Application.dataPath, "../PerformanceReports");
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, $"perf_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
        File.WriteAllText(path, r.ToString());
        Debug.Log($"[PerformanceOverlay] Saved → {path}");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static Key ToKey(KeyCode k) => (Key)System.Enum.Parse(typeof(Key), k.ToString());
}
