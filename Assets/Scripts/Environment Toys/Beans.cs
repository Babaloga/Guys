using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Beans : NetworkBehaviour
{
    private void Start()
    {
        if (IsServer) GetComponent<Rigidbody>().velocity = Random.insideUnitSphere * 5;
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
