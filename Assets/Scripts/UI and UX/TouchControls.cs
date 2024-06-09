using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class TouchControls : MonoBehaviour
{

#if UNITY_ANDROID

    GravitySensor gravity;

    Vector3 startGravity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        //InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
        
    }

    private void Update()
    {
        //gravity = GetRemoteDevice<GravitySensor>();
        gravity = GravitySensor.current;
        InputSystem.EnableDevice(gravity);
        //if (gravity.enabled)
        //{
        //    Debug.Log("GravitySensor is enabled");
        //    Debug.Log(gravity.gravity.ReadValue());
        //}

        //InputSystem.EnableDevice(GravitySensor.current);

        if(startGravity == Vector3.zero) startGravity = gravity.gravity.ReadValue();

        Vector3 gravityVector = gravity.gravity.ReadValue();
        SendMessage("OnMove", new Vector2(Mathf.Clamp(Vector3.SignedAngle(startGravity, gravityVector, Vector3.forward)/15f, -1, 1), Mathf.Clamp(Vector3.SignedAngle(startGravity, gravityVector, Vector3.right)/15f, -1, 1)).normalized);
    }

    private static TDevice GetRemoteDevice<TDevice>()
    where TDevice : InputDevice
    {
        foreach (var device in InputSystem.devices)
            if (device.remote && device is TDevice deviceOfType)
                return deviceOfType;
        return default;
    }
#endif
}
