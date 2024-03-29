using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.UI;

public class Leaderboard : NetworkBehaviour
{
    public static Dictionary<string, float> lifetimeDict;
    public static Dictionary<string, int> killsDict;
    public static Dictionary<string, float> altitudeDict;
    public static Dictionary<string, int> bouncesDict;
    public static Dictionary<string, int> flipsDict;

    private TMPro.TMP_Text uiText;

    public Goal startingGoal;

    public static NetworkVariable<Goal> currentGoal = new NetworkVariable<Goal>();

    public float goalDuration = 0;
    private static float goalStart = 0;

    private static List<Goal> unvisited;

    public enum Goal {
        Lifetime,
        Kills,
        Altitude,
        Bounces,
        Flips
    }

    private void Start()
    {
        unvisited = new List<Goal>();
        uiText = GetComponent<TMPro.TMP_Text>();
    }

    public override void OnNetworkSpawn()
    {
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
        unvisited.AddRange(new Goal[]{ Goal.Lifetime, Goal.Kills, Goal.Altitude, Goal.Bounces, Goal.Flips});
    }

    private static void InitiateGoal(Goal goal)
    {
        currentGoal.Value = goal;
        goalStart = NetworkManager.Singleton.ServerTime.TimeAsFloat;
        unvisited.Remove(goal);

        print(currentGoal.Value);

        switch (currentGoal.Value)
        {
            case Goal.Lifetime:
                lifetimeDict = new Dictionary<string, float>();
                break;

            case Goal.Kills:
                killsDict = new Dictionary<string, int>();
                break;

            case Goal.Flips:
                flipsDict = new Dictionary<string, int>();
                break;

            case Goal.Bounces:
                bouncesDict = new Dictionary<string, int>();
                break;

            case Goal.Altitude:
                altitudeDict = new Dictionary<string, float>();
                break;
        }
    }
    public static void LogFlips(string guyName, int count)
    {
        print(guyName + " called LogFlips with " + count);

        if (currentGoal.Value != Goal.Flips) return;

        int existingRecord = 0;
        flipsDict.TryGetValue(guyName, out existingRecord);
        if (existingRecord < count) flipsDict[guyName] = count;
    }

    public static void LogBounces(string guyName, int count)
    {
        print(guyName + " called LogBounces with " + count);
        if (currentGoal.Value != Goal.Bounces) return;

        int existingRecord = 0;
        bouncesDict.TryGetValue(guyName, out existingRecord);
        if (existingRecord < count) bouncesDict[guyName] = count;
    }

    public static void LogAltitude(string guyName, float altitude)
    {
        if (currentGoal.Value != Goal.Altitude) return;

        float existingRecord = 0;
        altitudeDict.TryGetValue(guyName, out existingRecord);

        if (existingRecord < altitude) altitudeDict[guyName] = altitude;
    }

    public static void LogLifetime(string guyName, float guyTime)
    {
        if (currentGoal.Value != Goal.Lifetime) return;

        lifetimeDict[guyName] = guyTime;
    }

    public static void LogDeath(string killerName)
    {
        if (currentGoal.Value != Goal.Kills) return;

        int count = 0;

        killsDict.TryGetValue(killerName, out count);

        killsDict[killerName] = count + 1;
    }

    private void LateUpdate()
    {
        if (IsServer)
        {
            if(goalDuration > 0 && NetworkManager.Singleton.ServerTime.TimeAsFloat - goalStart >= goalDuration)
            {
                if (unvisited.Count == 0) 
                {
                    ResetUnvisited();
                }

                string toPrint = "";
                foreach (Goal g in unvisited)
                {
                    toPrint += " " + g.ToString();
                }
                print(toPrint);

                Goal nextGoal = unvisited.RandomEntry();

                if(nextGoal == currentGoal.Value)
                {
                    nextGoal = (Goal)Mathf.Repeat((int)nextGoal++, unvisited.Count);
                }

                InitiateGoal(nextGoal);
            }

            ConstructLeaderboard();
        }
    }

    private void ConstructLeaderboard()
    {
        string displayText = "<b><align=\"center\">Top Five Guys™</b>\n";

        switch (currentGoal.Value)
        {
            case Goal.Lifetime:
                var sortedLifeDictionary = lifetimeDict.OrderByDescending(pair => pair.Value);

                for (int i = 0; i < Mathf.Clamp(5, 0, sortedLifeDictionary.Count()); i++)
                {
                    var element = sortedLifeDictionary.ElementAt(i);
                    displayText += string.Format("<align=\"left\">{0}: {1}<line-height=0>\n<align=\"right\">{2}<line-height=1em>\n", i + 1, element.Key, element.Value.ToString("###0.00"));
                }
                break;

            case Goal.Kills:
                var sortedKillsDictionary = killsDict.OrderByDescending(pair => pair.Value);

                for (int i = 0; i < Mathf.Clamp(5, 0, sortedKillsDictionary.Count()); i++)
                {
                    var element = sortedKillsDictionary.ElementAt(i);
                    displayText += string.Format("<align=\"left\">{0}: {1}<line-height=0>\n<align=\"right\">{2} kills<line-height=1em>\n", i + 1, element.Key, element.Value);
                }
                break;

            case Goal.Flips:
                var sortedFlipsDictionary = flipsDict.OrderByDescending(pair => pair.Value);

                for (int i = 0; i < Mathf.Clamp(5, 0, sortedFlipsDictionary.Count()); i++)
                {
                    var element = sortedFlipsDictionary.ElementAt(i);
                    displayText += string.Format("<align=\"left\">{0}: {1}<line-height=0>\n<align=\"right\">{2} flips<line-height=1em>\n", i + 1, element.Key, element.Value);
                }
                break;

            case Goal.Bounces:
                var sortedBouncesDictionary = bouncesDict.OrderByDescending(pair => pair.Value);

                for (int i = 0; i < Mathf.Clamp(5, 0, sortedBouncesDictionary.Count()); i++)
                {
                    var element = sortedBouncesDictionary.ElementAt(i);
                    displayText += string.Format("<align=\"left\">{0}: {1}<line-height=0>\n<align=\"right\">{2} bounces<line-height=1em>\n", i + 1, element.Key, element.Value);
                }
                break;

            case Goal.Altitude:
                var sortedAltitudeDictionary = altitudeDict.OrderByDescending(pair => pair.Value);

                for (int i = 0; i < Mathf.Clamp(5, 0, sortedAltitudeDictionary.Count()); i++)
                {
                    var element = sortedAltitudeDictionary.ElementAt(i);
                    displayText += string.Format("<align=\"left\">{0}: {1}<line-height=0>\n<align=\"right\">{2} m<line-height=1em>\n", i + 1, element.Key, element.Value.ToString("###0.00"));
                }
                break;
        }

        RenderLeaderboardRPC(displayText);
    }

    [Rpc(SendTo.Everyone)]
    private void RenderLeaderboardRPC(string displayText)
    {
        uiText.text = displayText;
    }
}
