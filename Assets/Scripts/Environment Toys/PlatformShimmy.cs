using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlatformShimmy : NetworkBehaviour
{
    public float range = 2;
    public float speed = 1;
    private float startPos;
    private float offset;

    void Start()
    {
        startPos = transform.localPosition.y;
        offset = transform.position.x + transform.position.z;
    }

    void Update()
    {
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            startPos + (Mathf.PerlinNoise1D(offset + (speed * Time.time/500f)) * 2 * range) - range,
            transform.localPosition.z);
    }
}
