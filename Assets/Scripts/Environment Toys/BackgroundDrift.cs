using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;

public class BackgroundDrift : MonoBehaviour
{
    MeshRenderer rendered;
    public double scale = 1;
    public float offset = 0;

    Vector3 sPos;
    private double startSeconds;

    // Start is called before the first frame update
    void Start()
    {
        sPos = transform.localRotation.eulerAngles;
        startSeconds = (double)(DateTime.UtcNow.Ticks - new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks) / (double)10000000;
        //startSeconds = startSeconds % (360 / scale);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(sPos.x + 0, (float)((sPos.y + ((startSeconds + (Time.timeAsDouble * (double)100.0)) * scale / (double)100))%(double)360), sPos.z + 0);
    }
}
