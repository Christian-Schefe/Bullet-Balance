using Persistence;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private int maxLogLength = 700;

    private string logBuffer = "--- Log ---\n";
    private string filename = null;
    private bool doShow = true;

    private void Awake()
    {
        Globals<DebugConsole>.RegisterOrDestroy(this);
#if UNITY_EDITOR
        doShow = false;
#endif
    }

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            doShow = !doShow;
        }
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        var str = $"[{type}] {logString}\n";
        if (type == LogType.Error || type == LogType.Exception)
        {
            str += $"{stackTrace}\n";
        }

        logBuffer += str;

        if (logBuffer.Length > maxLogLength)
        {
            logBuffer = logBuffer[^maxLogLength..];
        }

        if (filename == null)
        {
            var dirPath = FilePersistence.BuildPersistencePath("logs");
            System.IO.Directory.CreateDirectory(dirPath);
            var dateStr = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            filename = dirPath + "/log-" + dateStr + ".txt";
        }

        try
        {
            System.IO.File.AppendAllText(filename, str);
        }
        catch { }
    }

    void OnGUI()
    {
        if (!doShow) return;
        var scale = new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f);
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
        GUI.TextArea(new Rect(10, 10, 540, 370), logBuffer);
    }
}
