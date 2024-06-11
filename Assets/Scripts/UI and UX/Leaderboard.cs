using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using GoalModes;
using UnityEngine.UI;

public class Leaderboard : NetworkBehaviour
{
    //public static Dictionary<string, float> lifetimeDict;
    //public static Dictionary<string, int> killsDict;
    //public static Dictionary<string, float> altitudeDict;
    //public static Dictionary<string, int> bouncesDict;
    //public static Dictionary<string, int> flipsDict;

    //public static Dictionary<string, float> leaderDict;

    public static Leaderboard singleton;

    public GameObject[] Orbs;

    private TMPro.TMP_Text uiText;

    public Goal startingGoal;

    public NetworkVariable<Goal> currentGoal = new NetworkVariable<Goal>();

    public static IGoalMode GoalObj { get; set; }

    public float goalDuration = 0;
    private static float goalStart = 0;

    private static List<Goal> unvisited;

    //public GameObject runnerCrown;
    public GameObject ufoPrefab;

    private string lastWinner;
    private int consecutiveWins;

    public enum Goal {
        Lifetime,
        Kills,
        Altitude,
        Bounces,
        Flips,
        Beans,
        Runner,
        Absorption
    }

    private static Dictionary<Goal, IGoalMode> goalObjDict = new Dictionary<Goal, IGoalMode>
    {
        { Goal.Lifetime, new Lifetime() },
        { Goal.Kills, new Kills() },
        { Goal.Altitude, new Altitude() },
        { Goal.Bounces, new Bounces() },
        { Goal.Flips, new Flips() },
        { Goal.Beans, new BeanCollection() },
        { Goal.Runner, new Runner() },
        { Goal.Absorption, new Absorption() }
    };

    private void Start()
    {
        unvisited = new List<Goal>();
        uiText = GetComponent<TMPro.TMP_Text>();
    }

    public override void OnNetworkSpawn()
    {
        singleton = this;

        if (IsServer)
        {
            ResetUnvisited();
            InitiateGoal(startingGoal);
            unvisited.Remove(startingGoal);
        }



        base.OnNetworkSpawn();
    }

    private static void ResetUnvisited()
    {
        unvisited.AddRange(new Goal[]{ Goal.Kills, Goal.Altitude, Goal.Bounces, Goal.Flips, Goal.Beans, Goal.Runner, Goal.Absorption});
    }

    private static void InitiateGoal(Goal goal)
    {
        singleton.currentGoal.Value = goal;
        goalStart = NetworkManager.Singleton.ServerTime.TimeAsFloat;
        unvisited.Remove(goal);
        GoalObj = goalObjDict[goal];
        GoalObj.InitRpc();

        print(singleton.currentGoal.Value);
    }
    
    #region Log Calls
    /*
    public static void LogFlips(string guyName, int count)
    {
        print(guyName + " called LogFlips with " + count);

        if (currentGoal.Value != Goal.Flips) return;

        float existingRecord = 0;
        leaderDict.TryGetValue(guyName, out existingRecord);
        if (existingRecord < count) leaderDict[guyName] = count;
    }

    public static void LogBounces(string guyName, int count)
    {
        print(guyName + " called LogBounces with " + count);
        if (currentGoal.Value != Goal.Bounces) return;

        float existingRecord = 0;
        leaderDict.TryGetValue(guyName, out existingRecord);
        if (existingRecord < count) leaderDict[guyName] = count;
    }

    public static void LogAltitude(string guyName, float altitude)
    {
        if (currentGoal.Value != Goal.Altitude) return;

        float existingRecord = 0;
        leaderDict.TryGetValue(guyName, out existingRecord);

        if (existingRecord < altitude) leaderDict[guyName] = altitude;
    }

    public static void LogLifetime(string guyName, float guyTime)
    {
        if (currentGoal.Value != Goal.Lifetime) return;

        leaderDict[guyName] = guyTime;
    }

    public static void LogDeath(string killerName)
    {
        if (currentGoal.Value != Goal.Kills) return;

        float count = 0;

        leaderDict.TryGetValue(killerName, out count);

        leaderDict[killerName] = count + 1;
    }

    public static void LogBeanPickup(string guyName)
    {
        if (currentGoal.Value != Goal.Beans) return;

        float count = 0;

        leaderDict.TryGetValue(guyName, out count);

        leaderDict[guyName] = count + 1;
    }

    private static float lastTransferTime = 0;

    public static void LogCollision(string guyName, string otherGuyName)
    {
        if (currentGoal.Value != Goal.Runner) return;

        float value = 0;
        leaderDict.TryGetValue(guyName, out value);

        if((int)value == 1 && Time.time - lastTransferTime > 0.5f)
        {
            leaderDict[guyName] = 0;
            leaderDict[otherGuyName] = 1;
            lastTransferTime = Time.time;
        }
    }

    public static void LogCrownPickup(string guyName)
    {
        if (!leaderDict.ContainsValue(1))
        {
            leaderDict[guyName] = 1;
        }
    }

    public static void LogDestroyed(string guyName)
    {
        if(currentGoal.Value == Goal.Runner)
        {
            leaderDict[guyName] = 0;
        }
    }
    */
    #endregion
    
    private void LateUpdate()
    {
        if (IsServer)
        {
            //Goal Period End
            if (goalDuration > 0 && NetworkManager.Singleton.ServerTime.TimeAsFloat - goalStart >= goalDuration)
            {
                bool someoneHasCrown = false;

                foreach (GuyBehavior g in GuyBehavior.activeGuys)
                {
                    if (g.m_crownActive.Value)
                    {
                        someoneHasCrown = true;

                        g.m_legacyCrown.Value = true;
                        g.m_crownActive.Value = false;

                        string guyName = g.GuyName;

                        if (guyName == lastWinner)
                        {
                            consecutiveWins++;

                            if (consecutiveWins >= 3)
                            {
                                NetworkObject ufo = Instantiate(ufoPrefab, new Vector3(0, 0, -50), Quaternion.identity).GetComponent<NetworkObject>();
                                ufo.Spawn();
                                ufo.GetComponent<UFOBehavior>().AcquireTarget(g.transform);
                            }
                        }
                        else
                        {
                            lastWinner = guyName;
                            consecutiveWins = 1;
                        }
                    }
                }

                if (!someoneHasCrown)
                {
                    lastWinner = "";
                    consecutiveWins = 0;
                }

                if (unvisited.Count == 0) 
                {
                    ResetUnvisited();
                }

                GoalObj.EndRpc();

                string toPrint = "";
                foreach (Goal g in unvisited)
                {
                    toPrint += " " + g.ToString();
                }
                print(toPrint);

                Goal nextGoal = unvisited.RandomEntry();

                if(nextGoal == currentGoal.Value)
                {
                    nextGoal = unvisited[(int)Mathf.Repeat((int)nextGoal + 1, unvisited.Count)];
                }

                InitiateGoal(nextGoal);
            }

            ConstructLeaderboard();
        }
    }

    private void ConstructLeaderboard()
    {
        string displayText = "<b><align=\"center\">Top Five Guys™</b>\n";
        IOrderedEnumerable<KeyValuePair<string, float>> sortedDictionary = GoalObj.OrderedLeaderboardDictionary;
        string unit = GoalObj.Unit;

        for (int i = 0; i < Mathf.Clamp(5, 0, sortedDictionary.Count()); i++)
        {
            var element = sortedDictionary.ElementAt(i);
            displayText += string.Format("<align=\"left\">{0}: {1}<line-height=0>\n<align=\"right\">{2} {3}<line-height=1em>\n", i + 1, element.Key, element.Value.ToString("#####0.##"), unit);
        }

        if (sortedDictionary.Count() > 0)
        {
            foreach (GuyBehavior g in GuyBehavior.activeGuys)
            {
                if (g.GuyName == sortedDictionary.ElementAt(0).Key)
                {
                    g.m_crownActive.Value = true;
                }
                else
                {
                    g.m_crownActive.Value = false;
                }
            }
        }

        RenderLeaderboardRPC(displayText);
    }

    [Rpc(SendTo.Everyone)]
    private void RenderLeaderboardRPC(string displayText)
    {
        uiText.text = displayText;
    }
}
