using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

namespace GoalModes {
    public class Lifetime : IGoalMode
    {
        public Dictionary<string, float> LeaderDict { get; set; }
        public string Unit { get { return "s"; } }

        public void InitRpc()
        {
            LeaderDict = new Dictionary<string, float>();
        }
        public void EndRpc()
        {
            return;
        }

        public void LogEventOnCollisionRpc(GuyBehavior guy, NetworkObjectReference objRef)
        {
            return;
        }

        public void LogEventOnTriggerRpc(GuyBehavior guy, NetworkObjectReference objRef)
        {
            return;
        }

        public void LogEventOnDestroyedRpc(GuyBehavior guy)
        {
            return;
        }

        public void LogEventOnUpdateRpc(GuyBehavior guy)
        {
            float lifeTime = guy.StartTime;
            LeaderDict[guy.GuyName] = lifeTime;
        }
    }
}
