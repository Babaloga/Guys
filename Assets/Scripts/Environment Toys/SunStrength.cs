using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunStrength : MonoBehaviour
{
    Light light;

    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        float scaleFactor = Mathf.Clamp01(10f * Mathf.Sin(transform.rotation.eulerAngles.x / (18 * Mathf.PI)) - 0.1f);
        light.intensity = scaleFactor * 0.7f;

        RenderSettings.ambientIntensity = Mathf.Clamp01(10f * Mathf.Sin(transform.rotation.eulerAngles.x / (18 * Mathf.PI)) - 1f);
        RenderSettings.reflectionIntensity = Mathf.Clamp01(10f * Mathf.Sin(transform.rotation.eulerAngles.x / (18 * Mathf.PI)) -1f);
    }
}
