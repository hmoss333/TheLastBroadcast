using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LogController : MonoBehaviour
{
    [SerializeField] List<string> filterWords;

    string filename = "";
    void OnEnable() { Application.logMessageReceived += Log; }
    void OnDisable() { Application.logMessageReceived -= Log; }

    public void Log(string logString, string stackTrace, LogType type)
    {
        if (filename == "")
        {
            string d = System.IO.Path.Combine(Application.persistentDataPath + "/logs");
            System.IO.Directory.CreateDirectory(d);
            filename = d + "/log_data.txt";
        }

        try
        {
            foreach (string word in filterWords)
            {
                if (logString.Contains(word))
                    return;
            }

            System.IO.File.AppendAllText(filename, DateTime.Now + " " + logString + "\n");
        }
        catch { }
    }
}
