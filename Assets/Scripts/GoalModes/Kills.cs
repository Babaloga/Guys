using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

namespace GoalModes {
    public class Kills : IGoalMode
    {
        public Dictionary<string, float> LeaderDict { get; set; }
        public string Unit { get { return "kills"; } }

        Dictionary<string, string> lastHits;

        public void InitRpc()
        {
            LeaderDict = new Dictionary<string, float>();
            lastHits = new Dictionary<string, string>();
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

            GuyBehavior otherGuy = result.gameObject.GetComponent<GuyBehavior>();

            if(otherGuy != null)
            {
                lastHits[guy.GuyName] = otherGuy.GuyName;
            }
        }

        public void LogEventOnTriggerRpc(GuyBehavior guy, NetworkObjectReference objRef)
        {
            return;
        }

        public void LogEventOnDestroyedRpc(GuyBehavior guy)
        {
            string killerName = "";
            lastHits.TryGetValue(guy.GuyName, out killerName);
            if (killerName != "")
            {
                float count = 0;
                LeaderDict.TryGetValue(killerName, out count);
                LeaderDict[killerName] = count + 1;
            }
        }

        public void LogEventOnUpdateRpc(GuyBehavior guy)
        {
            if (guy.Grounded && guy.Velocity.sqrMagnitude < 64) lastHits[guy.GuyName] = "";

            return;
        }
    }
}
