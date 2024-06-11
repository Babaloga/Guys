using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJump : MonoBehaviour
{
    public float force = 20;
    ParticleSystem particles;

    private void Start()
    {
        particles = transform.parent.GetComponentInChildren<ParticleSystem>();

        ParticleSystem.MainModule particlesMain = particles.main;
        particlesMain.startSpeed = force / 400f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
        {
            Vector3 relative = (other.transform.position - transform.position);
            float relativeAngleCos = Mathf.Cos(Vector3.Angle(relative, transform.up) * Mathf.Deg2Rad);
            other.attachedRigidbody.AddForce((force * (Time.deltaTime/(relative.magnitude * relativeAngleCos))) * transform.up, ForceMode.Acceleration);
        }
    }
}
