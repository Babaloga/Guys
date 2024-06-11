using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trembles : MonoBehaviour
{
    List<Transform> panels;

    // Start is called before the first frame update
    void Start()
    {
        panels = new List<Transform>(GetComponentsInChildren<Transform>());
        panels.Remove(transform);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            Transform t = panels[i];
            t.localScale = Vector3.one * Mathf.Lerp(0.5f, 1.5f, Mathf.PerlinNoise1D((i * 100f) + Time.time * 0.1f));
            t.Rotate(t.rotation.eulerAngles.x + Mathf.Lerp(0.5f, 1.5f, Mathf.PerlinNoise1D((i * 100f) + Time.time * 1f)), t.rotation.eulerAngles.y + Mathf.Lerp(0.5f, 1.5f, Mathf.PerlinNoise1D((i * 100f) + Time.time * 1f)), t.rotation.eulerAngles.z + Mathf.Lerp(0.5f, 1.5f, Mathf.PerlinNoise1D((i * 100f) + Time.time * 1f)) * 1.2f);
        }
    }
}
