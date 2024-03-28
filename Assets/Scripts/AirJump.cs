using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJump : MonoBehaviour
{
    public float force = 20;

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
        {
            other.attachedRigidbody.AddForce((force * (Time.deltaTime/(other.transform.position.y - transform.position.y))) * Vector3.up, ForceMode.Acceleration);
        }
    }
}
