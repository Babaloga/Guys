using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(AudioListener))]
public class AudioToggle : MonoBehaviour
{
    new Camera camera;
    AudioListener listener;

    private void Start()
    {
        listener = GetComponent<AudioListener>();
        camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        camera.fieldOfView = Camera.HorizontalToVerticalFieldOfView(92, camera.aspect);
        if (camera.fieldOfView < 65) camera.fieldOfView = 65;
    }

    private void OnGUI()
    {
        if (NetworkManager.Singleton.IsServer) return;

        GUILayout.BeginArea(new Rect(0, Screen.height-20, 150, 20));

        if (GUILayout.Button(listener.enabled ? "Sound Off" : "Sound On"))
        {
            listener.enabled = !listener.enabled;
        }

        GUILayout.EndArea();
      
    }

}
