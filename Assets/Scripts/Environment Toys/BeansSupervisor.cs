using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BeansSupervisor : MonoBehaviour
{
    public static BeansSupervisor singleton;
    public static List<Beans> masterBeansList;
    public static bool inBeanPhase = false;

    public GameObject beanPrefab;

    public int startingBeans = 10;
    public float beansIntervalMin = 0.1f;
    public float beansIntervalMax = 1f;
    public int maxBeans = 30;

    public float spawnHeight = -1;
    public float beanSpawnRadius = 100;
    public float crownSpawnRadius = 14;

    private float nextBeanSpawn = 0;

    public GameObject runnerCrown;
    private GameObject runnerCrownInstance;

    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        masterBeansList = new List<Beans>();
    }

    float crownRespawnCountdown = 1;

    // Update is called once per frame
    void LateUpdate()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (inBeanPhase && NetworkManager.Singleton.ServerTime.TimeAsFloat >= nextBeanSpawn)
            {
                if (masterBeansList.Count < maxBeans)
                {
                    SpawnBean();
                }

                nextBeanSpawn = NetworkManager.Singleton.ServerTime.TimeAsFloat + Random.Range(beansIntervalMin, beansIntervalMax);
            }

            if (Leaderboard.singleton.currentGoal.Value == Leaderboard.Goal.Runner && !runnerCrownInstance)
            {
                bool crownOnTheField = false;
                foreach (GuyBehavior gb in GuyBehavior.activeGuys)
                {
                    if (gb.m_crownActive.Value)
                    {
                        crownOnTheField = true;
                        break;
                    }
                }

                if (!crownOnTheField)
                {
                    if (crownRespawnCountdown > 0)
                    {
                        crownRespawnCountdown -= Time.deltaTime;
                    }
                    else
                        BeginRunnerPhase();
                }
                else
                {
                    crownRespawnCountdown = 1;
                }
            }
        }
    }

    public void BeginBeanPhase()
    {
        for (int i = 0; i < startingBeans; i++)
        {
            SpawnBean();
        }

        nextBeanSpawn = Random.Range(beansIntervalMin, beansIntervalMax);
        inBeanPhase = true;
    }

    public void EndBeanPhase()
    {
        Beans[] allBeans = FindObjectsByType<Beans>(FindObjectsSortMode.None);

        foreach(Beans b in allBeans)
        {
            b.NetworkObject.Despawn(true);
        }
        inBeanPhase = false;
    }

    private void SpawnBean()
    {
        Vector3 randomPos = Quaternion.AngleAxis(Random.Range(0f, 359f), Vector3.up) * (Vector3.forward * beanSpawnRadius);
        Vector3 spawnPos = new Vector3(randomPos.x, spawnHeight, randomPos.z);
        GameObject instance = Instantiate(beanPrefab, spawnPos, Random.rotation);
        instance.GetComponent<NetworkObject>().Spawn();
    }

    public void BeginRunnerPhase()
    {
        Vector2 randomPos = Random.insideUnitCircle * crownSpawnRadius;
        Vector3 spawnPos = new Vector3(randomPos.x, 2, randomPos.y);

        runnerCrownInstance = Instantiate(runnerCrown, spawnPos, Quaternion.identity);
        runnerCrownInstance.GetComponent<NetworkObject>().Spawn();
    }

    public void EndRunnerPhase()
    {
        if (runnerCrownInstance) Destroy(runnerCrownInstance);
    }
}
