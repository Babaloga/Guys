using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
///     Base class for all Goal Modes
/// </summary>
/// <typeparam name="T">The value type tracked by the goalmode</typeparam>
namespace GoalModes
{
    public interface IGoalMode
    {
        IOrderedEnumerable<KeyValuePair<string, float>> OrderedLeaderboardDictionary
        {
            get
            {
                return LeaderDict.OrderByDescending(pair => pair.Value);
            }
        }

        Dictionary<string, float> LeaderDict { get; set; }

        string Unit { get; }

        [Rpc(SendTo.Server)]
        void InitRpc();

        [Rpc(SendTo.Server)]
        void EndRpc();

        [Rpc(SendTo.Server)]
        void LogEventOnUpdateRpc(GuyBehavior guy);

        [Rpc(SendTo.Server)]
        void LogEventOnCollisionRpc(GuyBehavior guy, NetworkObjectReference objRef);

        [Rpc(SendTo.Server)]
        void LogEventOnTriggerRpc(GuyBehavior guy, NetworkObjectReference objRef);

        [Rpc(SendTo.Server)]
        void LogEventOnDestroyedRpc(GuyBehavior guy);
    }
}
