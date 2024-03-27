using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System;
using Unity.Collections;

public class GuyBehavior : NetworkBehaviour
{
    public float speed = 10;
    public float jumpSpeed = 10;

    public bool cameraRelativeMovement = false;

    public NetworkVariable<FixedString64Bytes> m_guyName = new NetworkVariable<FixedString64Bytes>();
    TMPro.TMP_Text guyNametag;
    Rigidbody rb;

    string[] namesList1;
    string[] namesList2;
    string[] nouns;

    bool grounded = true;

    private float startTime;

    public GameObject playerRing;
    public GameObject deathPrefab;

    public static List<GuyBehavior> activeGuys;

    public override void OnNetworkSpawn()
    {
        if (activeGuys == null) activeGuys = new List<GuyBehavior>();

        playerRing.SetActive(IsOwner);

        if (IsServer) startTime = Time.time;

        Initialize();

        base.OnNetworkSpawn();
    }

    private void Initialize()
    {
        guyNametag = GetComponentInChildren<TMPro.TMP_Text>();
        rb = GetComponent<Rigidbody>();

        if (IsServer)
        {


            string namesList1In = Resources.Load<TextAsset>("Name1").text;

            namesList1 = namesList1In.Split('\n');

            string namesList2In = Resources.Load<TextAsset>("Name2").text;

            namesList2 = namesList2In.Split('\n');

            string nounsListIn = Resources.Load<TextAsset>("nouns").text;

            nouns = nounsListIn.Split('\n');

            int rand = UnityEngine.Random.Range(0, 1000);
            if (rand < 1)
            {
                m_guyName.Value = string.Concat("Good Ol' ", namesList2.RandomEntry().ToTitleCase(), " ", namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
            }
            else if(rand < 201)
            {
                if(rand < 41)
                {
                    m_guyName.Value = string.Format("{0} the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 81)
                {
                    m_guyName.Value = string.Format("{0} with the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 121)
                {
                    m_guyName.Value = string.Format("{0} of the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 161)
                {
                    m_guyName.Value = string.Format("{0} the {1}less", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else
                {
                    m_guyName.Value = string.Format("{0} from the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
            }
            else if (rand < 206)
            {
                m_guyName.Value = namesList1.RandomEntry().ToTitleCase().Replace("\r", "");
            }
            else
            {
                m_guyName.Value = string.Concat(namesList1.RandomEntry().ToTitleCase(), " the ", namesList2.RandomEntry().ToTitleCase()).Replace("\r", "");
            }
        }
        //SetNameRpc(m_guyName.Value);
    }

    private void Start()
    {
        
        guyNametag.SetText(m_guyName.Value.ToString());

    }

    private void OnEnable()
    {
        if (activeGuys == null) activeGuys = new List<GuyBehavior>();
        activeGuys.Add(this);
    }

    public override void OnDestroy()
    {
        activeGuys.Remove(this);
        base.OnDestroy();
    }

    //[Rpc(SendTo.ClientsAndHost)]
    //public void SetNameRpc(string newName)
    //{
    //    m_guyName.Value = newName;

    //    guyNametag.SetText(newName);
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner) return;

        Vector3 forceDirection = new Vector3(Input.GetAxis("Horizontal"), rb.velocity.y, Input.GetAxis("Vertical")).normalized;
        ApplyFixedMovementRPC(forceDirection);
    }

    //[Rpc(SendTo.Server)]
    private void ApplyFixedMovementRPC(Vector3 forceDirection)
    {
        float forceMagnitude = Time.fixedDeltaTime * speed;

        if (cameraRelativeMovement)
        {
            Transform camera = Camera.main.transform;

            Vector3 zVector = transform.position - camera.position;
            zVector.y = 0;
            zVector = zVector.normalized;
            Vector3 xVector = Quaternion.Euler(0, 90, 0) * zVector;

            forceDirection = (forceDirection.x * xVector) + (forceDirection.z * zVector);
        }

        grounded = Physics.Raycast(transform.position - (transform.up/2f), Vector3.down, 0.51f);

        if (grounded)
        {
            if (forceDirection == Vector3.zero)
            {
                rb.AddForce(0.5f * speed * Time.fixedDeltaTime * -rb.velocity);
            }
            else
            {
                rb.AddForce(forceDirection * forceMagnitude);
            }
        }
        else
        {
            rb.AddForce(forceDirection * (forceMagnitude / 10f));
        }
    }

    private float lastLeaderboardUpdateTime = 0;

    private void Update()
    {
        if (IsServer)
        {
            if(lastHit != null && grounded && rb.velocity.sqrMagnitude <= 25)
            {
                lastHit = null;
            }

            UpdateLeaderboard();
        }

        if (!IsOwner) return;

        ApplyMovementRPC(Input.GetButtonDown("Jump"));

        if(Time.time - lastLeaderboardUpdateTime >= 0.25f)
        {
            lastLeaderboardUpdateTime = Time.time;
        }

        if (transform.position.y < -1) KillMeRpc();
    }

    private void UpdateLeaderboard()
    {
        Leaderboard.UpdateLeaderboard(m_guyName.Value.ToString(), Time.time - startTime);
    }

    private void RegisterDeath()
    {
        if(lastHit != null) Leaderboard.LogDeath(lastHit.m_guyName.Value.ToString());
    }

    private bool setToDestroy = false;

    [Rpc(SendTo.Server)]
    public void KillMeRpc()
    {
        if (!setToDestroy)
        {
            setToDestroy = true;
            ulong clientID = OwnerClientId;

            DeathExplosionRpc();
            RegisterDeath();

            Destroy(gameObject);

            NetworkObject newPlayer = Instantiate(NetworkManager.NetworkConfig.PlayerPrefab).GetComponent<NetworkObject>();

            newPlayer.SpawnAsPlayerObject(clientID);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeathExplosionRpc()
    {
        Instantiate(deathPrefab, transform.position, transform.rotation);
    }

    //[Rpc(SendTo.Server)]
    private void ApplyMovementRPC(bool jumping)
    {
        if (jumping && grounded)
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);

        if (grounded && Vector3.Angle(transform.up, Vector3.up) < 90f)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.GetComponent<GuyBehavior>())
    //    {
    //        float collisionSpeed = (other.attachedRigidbody.velocity - rb.velocity).magnitude;
    //        Vector3 relativePosition = (transform.position - other.transform.position).normalized;
    //        rb.AddForce(relativePosition * collisionSpeed * 3);
    //    }
    //}

    public GuyBehavior lastHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<GuyBehavior>())
        {
            float collisionSpeed = collision.relativeVelocity.sqrMagnitude;
            Vector3 relativePosition = (transform.position - collision.transform.position).normalized;
            lastHit = collision.gameObject.GetComponent<GuyBehavior>();
            CollisionRpc(collisionSpeed, relativePosition);
            //rb.AddForce(relativePosition * collisionSpeed * 3);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void CollisionRpc(float collisionSpeed, Vector3 relativePosition)
    {
        rb.AddForce(relativePosition * collisionSpeed * 3);
    }
}

public static class RandomExtension
{
    public static T RandomEntry<T> (this IEnumerable<T> inCollection)
    {
        return inCollection.ElementAt(UnityEngine.Random.Range(0, inCollection.Count()));
    }

    public static string ToTitleCase(this string str)
    {
        var firstword = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.Split(' ')[0].ToLower());
        str = str.Replace(str.Split(' ')[0], firstword);
        return str;
    }
}
