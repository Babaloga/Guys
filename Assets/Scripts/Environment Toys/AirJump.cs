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
            other.attachedRigidbody.AddForce((force * (Time.deltaTime/(other.transform.position.y - transform.position.y))) * Vector3.up, ForceMode.Acceleration);
        }
    }
}
