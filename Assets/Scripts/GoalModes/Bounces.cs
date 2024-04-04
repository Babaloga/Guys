using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

namespace GoalModes {
    public class Bounces : IGoalMode
    {
        public Dictionary<string, float> LeaderDict { get; set; }
        public string Unit { get { return "bounces"; } }

        private Dictionary<string, float> runningTotals;
        private Dictionary<string, float> lastBounceTracker;

        public void InitRpc()
        {
            LeaderDict = new Dictionary<string, float>();
            runningTotals = new Dictionary<string, float>();
            lastBounceTracker = new Dictionary<string, float>();
        }
        public void EndRpc()
        {
            return;
        }

        public void LogEventOnCollisionRpc(GuyBehavior guy, NetworkObjectReference objRef)
        {
            NetworkObject result;
            objRef.TryGet(out result);

            if (result == null) throw new System.Exception("Invalid Network Object Reference");

            string guyName = guy.GuyName;
            float lastBounce = 0;
            lastBounceTracker.TryGetValue(guyName, out lastBounce);

            if(Time.time - lastBounce > 0.2f)
            {
                Vector3 relativePosition = (guy.transform.position - result.transform.position).normalized;

                if (relativePosition.y > 0.2f)
                {
                    float guyRecord = 0;
                    LeaderDict.TryGetValue(guyName, out guyRecord);
                    runningTotals[guyName] += 1;
                    if (runningTotals[guyName] > guyRecord) LeaderDict[guyName] = runningTotals[guyName];
                }
            }
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
            if (guy.Grounded) runningTotals[guy.GuyName] = 0;
        }
    }
}
