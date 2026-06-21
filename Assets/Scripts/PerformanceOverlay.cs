using UnityEngine;
using System.Text;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Utilities;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using System.Linq;

public class PerformanceOverlay : SingletonMonoBehaviourAcrossScenes<PerformanceOverlay>
{
    public KeyCode ToggleKey = KeyCode.F1;
    public bool Visible = true;

    private int _frames;
    private float _timer;
    private float _fps;
    private float _ms;

    private long _lastMemory;
    private long _gcAlloc;

    private GUIStyle _style;
    private StringBuilder _sb = new StringBuilder();

    private float _spikeThresholdMs = 25f;
    private float _lastFrameMs;

    private List<float> _spikeHistory = new List<float>();
    private const int MaxSpikeHistory = 50;

    void Start()
    {
        _lastMemory = System.GC.GetTotalMemory(false);

        _style = new GUIStyle();
        _style.fontSize = 22;
        _style.fontStyle = FontStyle.Bold;
        _style.normal.textColor = Color.white;
    }

    private static Key ConvertKeyCode(KeyCode keyCode)
    {
        return (Key)System.Enum.Parse(typeof(Key), keyCode.ToString());
    }

    void Update()
    {
        // Toggle overlay
        if (Keyboard.current[ConvertKeyCode(ToggleKey)].wasPressedThisFrame)
            Visible = !Visible;

        // FPS + MS
        _frames++;
        _timer += Time.unscaledDeltaTime;

        if (_timer >= 1f)
        {
            _fps = _frames / _timer;
            _ms = (_timer / _frames) * 1000f;

            _frames = 0;
            _timer = 0f;
        }

        // Spike detection
        _lastFrameMs = Time.unscaledDeltaTime * 1000f;

        if (_lastFrameMs > _spikeThresholdMs)
        {
            _spikeHistory.Add(_lastFrameMs);

            if (_spikeHistory.Count > MaxSpikeHistory)
                _spikeHistory.RemoveAt(0);
        }

        // GC allocation detection
        long mem = System.GC.GetTotalMemory(false);
        _gcAlloc = mem > _lastMemory ? mem - _lastMemory : 0;
        _lastMemory = mem;
    }

    // Auto-save on exit
    void OnApplicationQuit()
    {
        SavePerformanceReport();
    }

    void SavePerformanceReport()
    {
        StringBuilder report = new StringBuilder();

        report.AppendLine("=== PERFORMANCE REPORT ===");
        report.AppendLine($"Timestamp: {System.DateTime.Now}");
        report.AppendLine($"Scene: {SceneManager.GetActiveScene().name}");
        report.AppendLine("");

        // Basic frame stats
        report.AppendLine($"FPS: {_fps:F0}");
        report.AppendLine($"Frame Time (avg): {_ms:F2} ms");
        report.AppendLine($"Last Frame: {_lastFrameMs:F2} ms");
        report.AppendLine($"GC Alloc Last Frame: {_gcAlloc} bytes");
        report.AppendLine("");

        // Spike history
        report.AppendLine("=== SPIKE HISTORY (ms) ===");
        if (_spikeHistory.Count > 0)
        {
            report.AppendLine($"Spike Count: {_spikeHistory.Count}");
            report.AppendLine($"Worst Spike: {_spikeHistory.Max():F1} ms");
            report.AppendLine($"Average Spike: {_spikeHistory.Average():F1} ms");
            report.AppendLine("");

            foreach (var spike in _spikeHistory)
                report.AppendLine(spike.ToString("F1"));
        }
        else
        {
            report.AppendLine("No spikes recorded.");
        }

        report.AppendLine("");

        // Advanced memory breakdown
        report.AppendLine("=== MEMORY BREAKDOWN ===");
        report.AppendLine($"Mono Heap: {Profiler.GetMonoHeapSizeLong() / 1048576f:F2} MB");
        report.AppendLine($"Mono Used: {Profiler.GetMonoUsedSizeLong() / 1048576f:F2} MB");
        report.AppendLine($"Unity Heap: {Profiler.GetTotalAllocatedMemoryLong() / 1048576f:F2} MB");
        report.AppendLine($"Unity Reserved: {Profiler.GetTotalReservedMemoryLong() / 1048576f:F2} MB");
        report.AppendLine($"Unity Unused Reserved: {Profiler.GetTotalUnusedReservedMemoryLong() / 1048576f:F2} MB");
        report.AppendLine("");

        // GPU memory
        report.AppendLine("=== GPU MEMORY ===");
        report.AppendLine($"Graphics Driver Alloc: {Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576f:F2} MB");
        report.AppendLine("");

        // Scene object stats (new API)
        report.AppendLine("=== SCENE OBJECTS ===");

        var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        report.AppendLine($"Total GameObjects: {allObjects.Length}");
        report.AppendLine($"Active GameObjects: {allObjects.Count(g => g.activeInHierarchy)}");

        var allComponents = FindObjectsByType<Component>(FindObjectsSortMode.None);
        report.AppendLine($"Total Components: {allComponents.Length}");

        var rigidbodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        report.AppendLine($"Rigidbodies: {rigidbodies.Length}");

        var colliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);
        report.AppendLine($"Colliders: {colliders.Length}");

        report.AppendLine("");

        // Hardware info
        report.AppendLine("=== HARDWARE INFO ===");
        report.AppendLine($"CPU: {SystemInfo.processorType}");
        report.AppendLine($"Cores: {SystemInfo.processorCount}");
        report.AppendLine($"GPU: {SystemInfo.graphicsDeviceName}");
        report.AppendLine($"VRAM: {SystemInfo.graphicsMemorySize} MB");
        report.AppendLine($"RAM: {SystemInfo.systemMemorySize} MB");
        report.AppendLine("");

        // Display info (new API)
        report.AppendLine("=== DISPLAY ===");
        report.AppendLine($"Resolution: {Screen.currentResolution.width}x{Screen.currentResolution.height}");
        report.AppendLine($"Refresh Rate: {Screen.currentResolution.refreshRateRatio.value} Hz");
        report.AppendLine($"Fullscreen: {Screen.fullScreen}");
        report.AppendLine("");

        // Save next to EXE
        string folder = Path.Combine(Application.dataPath, "../PerformanceReports");
        Directory.CreateDirectory(folder);

        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string path = Path.Combine(folder, $"performance_report_{timestamp}.txt");

        File.WriteAllText(path, report.ToString());

        Debug.Log($"Performance report saved to: {path}");
    }

    void OnGUI()
    {
        if (!Visible) return;

        GUI.Box(new Rect(5, 5, 500, 500), GUIContent.none);

        _sb.Clear();
        _sb.AppendLine($"FPS: {_fps:F0}");
        _sb.AppendLine($"Frame Time: {_ms:F1} ms");
        _sb.AppendLine($"Last Frame: {_lastFrameMs:F1} ms");

        if (_lastFrameMs > _spikeThresholdMs)
            _sb.AppendLine($"Spike: {_lastFrameMs:F1} ms");

        if (_gcAlloc > 0)
            _sb.AppendLine($"GC Alloc: {_gcAlloc} bytes");

        _sb.AppendLine("");
        _sb.AppendLine($"CPU: {SystemInfo.processorType}");
        _sb.AppendLine($"Cores: {SystemInfo.processorCount}");
        _sb.AppendLine($"GPU: {SystemInfo.graphicsDeviceName}");
        _sb.AppendLine($"VRAM: {SystemInfo.graphicsMemorySize} MB");
        _sb.AppendLine($"RAM: {SystemInfo.systemMemorySize} MB");

        _sb.AppendLine("");
        _sb.AppendLine("Spike History (ms):");

        foreach (var spike in _spikeHistory)
            _sb.AppendLine(spike.ToString("F1"));

        GUI.Label(new Rect(15, 15, 600, 600), _sb.ToString(), _style);
    }
}
