using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;

public class BackgroundDrift : MonoBehaviour
{
    MeshRenderer rendered;
    public float scale = 1;
    public float offset = 0;

    Vector3 sPos;

    // Start is called before the first frame update
    void Start()
    {
        sPos = transform.localRotation.eulerAngles;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(sPos.x + 0, sPos.y + (DateTime.UtcNow.Ticks/ (50f * 10000000f)) * scale, sPos.z + 0);
    }
}
