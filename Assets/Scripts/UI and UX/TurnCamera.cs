using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Camera))]
public class TurnCamera : MonoBehaviour
{
    public float buffer = 50f;
    Camera camera;
    GameObject target;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!target)
        {
            if (NetworkManager.Singleton.LocalClient.PlayerObject)
                target = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
            else return;
        }

        Vector3 screenPosObj = camera.WorldToScreenPoint(new Vector3(target.transform.position.x, 0, target.transform.position.z));

        if (screenPosObj.z < 0) transform.Rotate(Vector3.up, 180, Space.World);

        if(screenPosObj.x < buffer)
        {
            transform.Rotate(Vector3.up, -Time.deltaTime * Mathf.Abs(screenPosObj.x - buffer), Space.World);
        }
        else if (screenPosObj.x > Screen.width - buffer)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * Mathf.Abs(buffer - (Screen.width - screenPosObj.x)), Space.World);
        }

    }
}
