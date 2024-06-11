using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Beans : NetworkBehaviour
{
    private void Start()
    {
        if (IsServer) 
        {
            float randMult = Random.Range(0.3f, 0.5f);
            GetComponent<Rigidbody>().velocity = new Vector3(-transform.position.x * randMult, 0, -transform.position.z * randMult) + (Random.insideUnitSphere * 5);
        }
    }

    private void Update()
    {
        if (IsServer && transform.position.y < -10f) Destroy(gameObject);
    }

    private void OnEnable()
    {
        BeansSupervisor.masterBeansList.Add(this);
    }

    public override void OnDestroy()
    {
        BeansSupervisor.masterBeansList.Remove(this);
        base.OnDestroy();
    }
}
