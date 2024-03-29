using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SeeSaw : NetworkBehaviour
{
    public SeeSaw partner;
    List<GuyBehavior> padGuys;

    Vector3 startingPos;

    void Start()
    {
        padGuys = new List<GuyBehavior>();
        startingPos = transform.position;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void LaunchRpc(float force)
    {
        float forceEach = Mathf.Abs(force / (float)padGuys.Count);

        foreach(GuyBehavior gb in padGuys)
        {
            gb.GetComponent<Rigidbody>().AddForce(Vector3.up * forceEach, ForceMode.VelocityChange);
        }

        StartCoroutine(BumpAnim());
    }

    IEnumerator BumpAnim()
    {
        GetComponent<MeshCollider>().enabled = false;
        transform.position = startingPos + (Vector3.up * 0.15f);
        yield return new WaitForSeconds(0.33f);
        transform.position = startingPos;
        GetComponent<MeshCollider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        GuyBehavior guy = other.GetComponent<GuyBehavior>();
        if (guy != null)
        {
            padGuys.Add(guy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GuyBehavior guy = other.GetComponent<GuyBehavior>();
        if (guy != null)
        {
            padGuys.Remove(guy);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.relativeVelocity.y);
        partner.LaunchRpc(collision.relativeVelocity.y);
    }
}
