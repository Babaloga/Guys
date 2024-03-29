using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemSelfDelete : MonoBehaviour
{
    ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        StartCoroutine(WaitForParticleSystemToFinish());
    }

    IEnumerator WaitForParticleSystemToFinish()
    {
        yield return new WaitForSeconds(ps.main.startLifetime.constant + 1f);

        Destroy(gameObject);
    }
}
