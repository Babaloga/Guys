using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

namespace GoalModes {
    public class Altitude : IGoalMode
    {
        public Dictionary<string, float> LeaderDict { get; set; }
        public string Unit { get { return "m"; } }

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
            float altitude = guy.transform.position.y;
            float existingRecord = 0;
            string guyName = guy.GuyName;
            LeaderDict.TryGetValue(guyName, out existingRecord);

            if (existingRecord < altitude) LeaderDict[guyName] = altitude;
        }
    }
}
