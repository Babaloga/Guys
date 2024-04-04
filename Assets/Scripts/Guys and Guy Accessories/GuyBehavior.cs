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

    public string GuyName { get { return m_guyName.Value.ToString(); } }

    private NetworkVariable<FixedString64Bytes> m_guyName = new NetworkVariable<FixedString64Bytes>();
    private NetworkVariable<bool> m_emitParticles = new NetworkVariable<bool>(default, default, NetworkVariableWritePermission.Owner);

    public TMPro.TMP_Text guyNametag;
    Rigidbody rb;
    new AudioSource audio;
    ParticleSystem particles;
    ParticleSystem.EmissionModule particleEmission;

    string[] namesList1;
    string[] namesList2;
    string[] nouns;

    public bool Grounded { get { return grounded; } }
    public Vector3 Velocity { get { return rb.velocity; } }

    bool grounded = true;

    private float startTime;

    public float StartTime { get { return startTime; } }

    public GameObject playerRing;
    public GameObject playerCrown;
    public GameObject playerLegacyCrown;

    public GameObject deathPrefab;

    public static List<GuyBehavior> activeGuys;

    public AudioClip[] lowClips;
    public AudioClip[] highClips;

    public NetworkVariable<bool> m_crownActive = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> m_legacyCrown = new NetworkVariable<bool>(false);

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
        audio = GetComponent<AudioSource>();
        particles = GetComponentInChildren<ParticleSystem>();
        particleEmission = particles.emission;
        playerCrown.transform.parent.localRotation = Quaternion.Euler(UnityEngine.Random.Range(-30, 30), UnityEngine.Random.Range(0, 360), 0);

        if(playerCrown.transform.localRotation.x > 0)
            playerLegacyCrown.transform.localRotation = Quaternion.Euler(UnityEngine.Random.Range(-playerCrown.transform.parent.localRotation.x, 0), 0, 0);
        else
            playerLegacyCrown.transform.localRotation = Quaternion.Euler(UnityEngine.Random.Range(0, -playerCrown.transform.parent.localRotation.x), 0, 0);

        if (IsServer)
        {
            string namesList1In = Resources.Load<TextAsset>("Name1").text;

            namesList1 = namesList1In.Split('\n');

            string namesList2In = Resources.Load<TextAsset>("Name2").text;

            namesList2 = namesList2In.Split('\n');

            string nounsListIn = Resources.Load<TextAsset>("nouns").text;

            nouns = nounsListIn.Split('\n');

            //m_lastHit.Value = "";

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
        if(IsOwner) GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere * 5;
    }

    private void OnEnable()
    {
        if (activeGuys == null) activeGuys = new List<GuyBehavior>();
        activeGuys.Add(this);
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer) Leaderboard.GoalObj.LogEventOnDestroyedRpc(this);

        activeGuys.Remove(this);

        base.OnNetworkDespawn();
    }

    public override void OnDestroy()
    {
        activeGuys.Remove(this);

        base.OnDestroy();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = false;
        RaycastHit groundRay;
        if(Physics.Raycast(transform.position, Vector3.down, out groundRay, 0.51f * transform.localScale.y))
        {
            if(groundRay.collider.gameObject.tag != "Slippery")
            {
                grounded = true;
            }
        }

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

        if (grounded)
        {
            //if (forceDirection == Vector3.zero)
            //{
            //    rb.AddForce(0.5f * speed * Time.fixedDeltaTime * -rb.velocity);
            //}
            //else
            //{
                rb.AddForce(forceDirection * forceMagnitude);
            //}
        }
        else
        {
            rb.AddForce(forceDirection * (forceMagnitude / 5f));
        }
    }

    private float lastLeaderboardUpdateTime = 0;
    private float lastTimeAirborne = 0;

    private void Update()
    {
        playerCrown.SetActive(m_crownActive.Value);

        playerLegacyCrown.SetActive(m_legacyCrown.Value);

        if (IsServer)
        {
            Leaderboard.GoalObj.LogEventOnUpdateRpc(this);
        }

        if (!grounded)
        {
            lastTimeAirborne = Time.time;
            if (IsOwner && particles != null)
            {
                m_emitParticles.Value = false;
            }
        }
        else if (IsOwner && rb.velocity.sqrMagnitude > 9)
        {
            if (particles != null)
            {
                m_emitParticles.Value = true;
            }
        }

        particleEmission.enabled = m_emitParticles.Value;

        if (!IsOwner) return;

        ApplyMovementRPC(Input.GetButtonDown("Jump"));

        if(Time.time - lastLeaderboardUpdateTime >= 0.25f)
        {
            lastLeaderboardUpdateTime = Time.time;
        }

        if (transform.position.y < -1 || Input.GetButtonUp("KillMe"))
        {
            KillMeRpc();
        }
    }

    //[Rpc(SendTo.Server)]
    //private void UnsetLastHitRpc()
    //{
    //    print("UNSETTING LAST HIT");
    //    m_lastHit.Value = "";
    //}

    private void UpdateLeaderboard()
    {
        //switch (Leaderboard.currentGoal.Value)
        //{
        //    case Leaderboard.Goal.Lifetime:
        //        Leaderboard.LogLifetime(m_guyName.Value.ToString(), Time.time - startTime);
        //        break;

        //    case Leaderboard.Goal.Altitude:
        //        Leaderboard.LogAltitude(m_guyName.Value.ToString(), transform.position.y);
        //        break;

        //    case Leaderboard.Goal.Bounces:
        //        if(grounded) consecutiveBounces = 0;
        //        break;

        //    case Leaderboard.Goal.Flips:

        //        //print(string.Format("{0} | Grounded: {1}, FlipEligible: {2}", m_guyName.Value.ToString(), grounded, flipEligible));

        //        if (grounded)
        //        {
        //            consecutiveFlips = 0;
        //            flipEligible = true;
        //        }
        //        else
        //        {
        //            if (flipEligible)
        //            {
        //                if(Vector3.Angle(transform.up, Vector3.down) < 45)
        //                {
        //                    flipEligible = false;
        //                    consecutiveFlips++;
        //                    Leaderboard.LogFlips(m_guyName.Value.ToString(), consecutiveFlips);
        //                }
        //            }
        //            else
        //            {
        //                if (Vector3.Angle(transform.up, Vector3.up) < 45)
        //                {
        //                    flipEligible = true;
        //                }
        //            }
        //        }

        //        break;
        //}
    }

    private bool setToDestroy = false;

    [Rpc(SendTo.Server)]
    public void KillMeRpc()
    {
        if (!setToDestroy)
        {
            setToDestroy = true;

            DeathExplosionRpc();
            
            KillAndRespawnRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void KillAndRespawnRpc()
    {
        ulong clientID = OwnerClientId;
        GetComponent<NetworkObject>().Despawn();
        NetworkObject newPlayer = Instantiate(NetworkManager.NetworkConfig.PlayerPrefab/*, new Vector3(UnityEngine.Random.Range(-3, 3), 5, UnityEngine.Random.Range(-3, 3)), Quaternion.identity*/).GetComponent<NetworkObject>();

        newPlayer.SpawnAsPlayerObject(clientID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeathExplosionRpc()
    {
        Instantiate(deathPrefab, transform.position, transform.rotation);
    }

    private int consecutiveUngroundedFrames = 0;

    //[Rpc(SendTo.Server)]
    private void ApplyMovementRPC(bool jumping)
    {
        if (jumping && grounded)
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);

        if (grounded && Vector3.Angle(transform.up, Vector3.up) < 90f)
        {
            consecutiveUngroundedFrames = 0;

            rb.constraints = RigidbodyConstraints.FreezeRotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.zero), 40 * Time.deltaTime);
        }
        else
        {
            consecutiveUngroundedFrames++;
            rb.constraints = RigidbodyConstraints.None;

            if(consecutiveUngroundedFrames <= 10)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.zero), 40 * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<GuyBehavior>() && collision.gameObject.GetComponent<GuyBehavior>().IsSpawned)
        {
            float collisionSpeed = collision.relativeVelocity.sqrMagnitude;
            Vector3 relativePosition = (transform.position - collision.transform.position).normalized;

            CollisionRpc(collisionSpeed, relativePosition, collision.contacts[0].point);
            CallCollisionRpc(collision.gameObject.GetComponent<NetworkObject>());
        }
    }

    [Rpc(SendTo.Server)]
    private void CallCollisionRpc(NetworkObjectReference objRef)
    {
        Leaderboard.GoalObj.LogEventOnCollisionRpc(this, objRef);
    }

    private void OnTriggerEnter(Collider other)
    {
        NetworkObject netObj = other.gameObject.GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsSpawned) CallTriggerRpc(netObj);
    }

    [Rpc(SendTo.Server)]
    private void CallTriggerRpc(NetworkObjectReference objRef)
    {
        Leaderboard.GoalObj.LogEventOnTriggerRpc(this, objRef);
    }

    [Rpc(SendTo.Owner)]
    public void SetScaleRpc(Vector3 newScale)
    {
        transform.localScale = newScale;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void CollisionRpc(float collisionSpeed, Vector3 relativePosition, Vector3 contactPoint)
    {
        if (relativePosition.z > 0 || (relativePosition.z == 0 && relativePosition.y > 0))
        {
            audio.volume = Mathf.Clamp(collisionSpeed / 300f, 0, 2);

            if (collisionSpeed < 1500)
            {
                audio.PlayOneShot(lowClips.RandomEntry());
            }
            else
            {
                audio.PlayOneShot(highClips.RandomEntry());
            }
        }
        rb.AddForceAtPosition(relativePosition * collisionSpeed * 2, contactPoint);
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
