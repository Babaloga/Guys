using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ConsoleToGUI : MonoBehaviour
{
    //#if !UNITY_EDITOR
    static string myLog = "";
    private string output;
    private string stack;

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        myLog = output + "\n" + myLog;
        if (myLog.Length > 5001)
        {
            myLog = myLog.Substring(0, 5000);
        }
    }

    void OnGUI()
    {
        //if (NetworkManager.Singleton.IsServer)
        //{
            myLog = GUI.TextArea(new Rect(10, 10, Screen.width / 3f, Screen.height - 10), myLog);
        //}
    }
    //#endif
}

