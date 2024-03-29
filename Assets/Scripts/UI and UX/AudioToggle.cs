using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class AudioToggle : MonoBehaviour
{

    AudioListener listener;

    private void Start()
    {
        listener = GetComponent<AudioListener>();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, Screen.height-50, 150, 150));

        if (GUILayout.Button(listener.enabled ? "Sound Off" : "Sound On"))
        {
            listener.enabled = !listener.enabled;
        }

        GUILayout.EndArea();
      
    }

}
