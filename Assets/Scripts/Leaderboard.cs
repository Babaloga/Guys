using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.UI;

public class Leaderboard : NetworkBehaviour
{
    public static Dictionary<string, float> leaderboardDict;
    public static Dictionary<string, int> killCounts;

    public bool killCount = true;

    private TMPro.TMP_Text uiText;

    private void Start()
    {
        leaderboardDict = new Dictionary<string, float>();
        killCounts = new Dictionary<string, int>();
        uiText = GetComponent<TMPro.TMP_Text>();
    }

    public static void UpdateLeaderboard(string guyName, float guyTime)
    {
        if(leaderboardDict != null) leaderboardDict[guyName] = guyTime;
    }

    public static void LogDeath(string killerName)
    {
        int count = 0;

        killCounts.TryGetValue(killerName, out count);

        killCounts[killerName] = count + 1;
    }

    private void LateUpdate()
    {
        if (IsServer) ConstructLeaderboard();
    }

    private void ConstructLeaderboard()
    {
        string displayText = "<b><align=\"center\">Top Five Guys™</b>\n";

        if (killCount)
        {
            var sortedDictionary = killCounts.OrderByDescending(pair => pair.Value);

            for (int i = 0; i < Mathf.Clamp(5, 0, sortedDictionary.Count()); i++)
            {
                var element = sortedDictionary.ElementAt(i);
                displayText += string.Format("<align=\"left\">{0}: {1}<line-height=0>\n<align=\"right\">{2} kills<line-height=1em>\n", i + 1, element.Key, element.Value);
            }
        }
        else
        {
            var sortedDictionary = leaderboardDict.OrderByDescending(pair => pair.Value);

            for (int i = 0; i < Mathf.Clamp(5, 0, sortedDictionary.Count()); i++)
            {
                var element = sortedDictionary.ElementAt(i);
                displayText += string.Format("<align=\"left\">{0}: {1}<line-height=0>\n<align=\"right\">{2}<line-height=1em>\n", i + 1, element.Key, element.Value.ToString("###0.00"));
            }
        }

        RenderLeaderboardRPC(displayText);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RenderLeaderboardRPC(string displayText)
    {
        uiText.text = displayText;
    }
}
