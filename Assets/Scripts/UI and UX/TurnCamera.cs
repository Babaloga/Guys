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

        /*
        Vector3 cameraToTarget = FlattenAndNormalize(target.transform.position - transform.parent.position);
        Vector3 flatForward = FlattenAndNormalize(transform.parent.forward);

        float angle = Vector3.SignedAngle(flatForward, cameraToTarget, Vector3.up);

        if(angle < 0)
        {
            angle += camera.fieldOfView / 3f;
            angle = Mathf.Clamp(angle, float.NegativeInfinity, 0);
        }
        else
        {
            angle -= camera.fieldOfView / 3f;
            angle = Mathf.Clamp(angle, 0, float.PositiveInfinity);
        }

        transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, transform.parent.rotation * Quaternion.Euler(0, angle, 0), 0.25f);
        */

        float offset = target.transform.position.x - transform.position.x;

        if(offset < -buffer)
        {
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, target.transform.position.x + buffer, 0.25f), transform.position.y, transform.position.z);
        }
        else if (offset > buffer)
        {
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, target.transform.position.x - buffer, 0.25f), transform.position.y, transform.position.z);
        }
    }

    Vector3 FlattenAndNormalize(Vector3 source)
    {
        return new Vector3(source.x, 0, source.z).normalized;
    }
}
