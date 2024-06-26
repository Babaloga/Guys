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
    private NetworkVariable<Vector3> m_velocity = new NetworkVariable<Vector3>(default, default, NetworkVariableWritePermission.Owner);

    public TMPro.TMP_Text guyNametag;
    Rigidbody rb;
    new AudioSource audio;
    ParticleSystem particles;
    ParticleSystem.EmissionModule particleEmission;

    string[] namesList1;
    string[] namesList2;
    string[] nouns;

    public bool Grounded { get { return grounded; } }
    public Vector3 Velocity { get { return m_velocity.Value; } }

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

    private float lastLeaderboardUpdateTime = 0;
    private bool setToDestroy = false;

    #region Startup
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
            if (rand < 1) //1
            {
                m_guyName.Value = string.Concat("Good Ol' ", namesList2.RandomEntry().ToTitleCase(), " ", namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
            }
            else if (rand < 6) //5
            {
                m_guyName.Value = namesList1.RandomEntry().ToTitleCase().Replace("\r", "");
            }
            else if(rand < 247) //240
            {
                if(rand < 47) //40
                {
                    m_guyName.Value = string.Format("{0} the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 87) //40
                {
                    m_guyName.Value = string.Format("{0} with the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 127) //40
                {
                    m_guyName.Value = string.Format("{0} of the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 167) //40
                {
                    m_guyName.Value = string.Format("{0} the {1}less", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 177) //10
                {
                    m_guyName.Value = string.Format("{0}, holder of the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 187) //10
                {
                    m_guyName.Value = string.Format("{0}, keeper of the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 207) //20
                {
                    m_guyName.Value = string.Format("{0} {1} {2}", namesList2.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase(), namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else //40
                {
                    m_guyName.Value = string.Format("{0} from the {1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
            }
            else if(rand < 307) //60
            {
                if(rand < 257) //10
                {
                    m_guyName.Value = string.Format("{0} (not the {1} one)", namesList1.RandomEntry().ToTitleCase(), namesList2.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 267) //10
                {
                    m_guyName.Value = string.Format("{0} the {1}-Man", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 277) //10
                {
                    m_guyName.Value = string.Format("{0} the {1}-Woman", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 287) //10
                {
                    m_guyName.Value = string.Format("{0} the Were{1}", namesList1.RandomEntry().ToTitleCase(), nouns.RandomEntry().ToLower()).Replace("\r", "");
                }
                else if (rand < 292) //5
                {
                    m_guyName.Value = string.Format("Thoroughly {0} {1}", namesList2.RandomEntry().ToTitleCase(), namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 297) //5
                {
                    m_guyName.Value = string.Format("Intensely {0} {1}", namesList2.RandomEntry().ToTitleCase(), namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else if (rand < 302) //5
                {
                    m_guyName.Value = string.Format("Off-Puttingly {0} {1}", namesList2.RandomEntry().ToTitleCase(), namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
                else //10
                {
                    m_guyName.Value = string.Format("The {0}er {1}", namesList2.RandomEntry().ToTitleCase(), namesList1.RandomEntry().ToTitleCase()).Replace("\r", "");
                }
            }
            else //693
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
    #endregion

    #region Destroy and Despawn
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

    private void OnApplicationPause(bool paused)
    {
        if (!IsClient) return;

        if (paused)
        {
            KillMeRpc(false);
        }
        else
        {
            KillAndRespawnRpc(false, true);
        }
    }

    #endregion

    #region Updates

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
        C_ApplyFixedMovement(forceDirection);

        m_velocity.Value = rb.velocity;
    }

    private void C_ApplyFixedMovement(Vector3 forceDirection)
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
            rb.AddForce(forceDirection * forceMagnitude);
        }
        else
        {
            rb.AddForce(forceDirection * (forceMagnitude / 5f));
        }
    }

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

        C_ApplyMovement(Input.GetButtonDown("Jump"));

        if(Time.time - lastLeaderboardUpdateTime >= 0.25f)
        {
            lastLeaderboardUpdateTime = Time.time;
        }

        if (transform.position.y < -5 || Input.GetButtonUp("KillMe"))
        {
            KillMeRpc();
        }
    }

    private void C_ApplyMovement(bool jumping)
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

            if (consecutiveUngroundedFrames <= 10)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.zero), 40 * Time.deltaTime);
            }
        }
    }

    #endregion

    [Rpc(SendTo.Server)]
    public void KillMeRpc(bool respawn = true)
    {
        if (!setToDestroy)
        {
            setToDestroy = true;

            DeathExplosionRpc();
            
            KillAndRespawnRpc(true, respawn);
        }
    }

    [Rpc(SendTo.Server)]
    public void KillAndRespawnRpc(bool kill = true, bool respawn = true)
    {
        ulong clientID = OwnerClientId;
        if(kill) GetComponent<NetworkObject>().Despawn();

        if (respawn)
        {
            NetworkObject newPlayer = Instantiate(NetworkManager.NetworkConfig.PlayerPrefab).GetComponent<NetworkObject>();
            newPlayer.SpawnAsPlayerObject(clientID);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeathExplosionRpc()
    {
        Instantiate(deathPrefab, transform.position, transform.rotation);
    }

    private int consecutiveUngroundedFrames = 0;

    [Rpc(SendTo.Owner)]
    public void SetScaleRpc(Vector3 newScale)
    {
        transform.localScale = newScale;
    }

    #region Collision And Trigger

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

    #endregion

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
