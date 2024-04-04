using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

namespace GoalModes {
    public class Flips : IGoalMode
    {
        public Dictionary<string, float> LeaderDict { get; set; }
        public string Unit { get { return "flips"; } }

        private Dictionary<string, float> runningTotals;
        private Dictionary<string, bool> flipEligible;

        public void InitRpc()
        {
            LeaderDict = new Dictionary<string, float>();
            runningTotals = new Dictionary<string, float>();
            flipEligible = new Dictionary<string, bool>();
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
            string guyName = guy.GuyName;

            if (guy.Grounded)
            {
                runningTotals[guyName] = 0;
                flipEligible[guyName] = true;
            }
            else
            {
                if (flipEligible[guyName])
                {
                    if (Vector3.Angle(guy.transform.up, Vector3.down) < 45)
                    {
                        flipEligible[guyName] = false;
                        runningTotals[guyName]++;

                        float existingRecord = 0;
                        LeaderDict.TryGetValue(guyName, out existingRecord);
                        if (existingRecord < runningTotals[guyName]) LeaderDict[guyName] = runningTotals[guyName];
                    }
                }
                else
                {
                    if (Vector3.Angle(guy.transform.up, Vector3.up) < 45)
                    {
                        flipEligible[guyName] = true;
                    }
                }
            }
        }
    }
}
