using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using System.Drawing.Printing;

namespace GoalModes {
    public class Runner : IGoalMode
    {
        public Dictionary<string, float> LeaderDict { get; set; }
        public string Unit { get { return "rad crown"; } }
        public float lastTransferTime = 0;

        public void InitRpc()
        {
            BeansSupervisor.singleton.BeginRunnerPhase();
            LeaderDict = new Dictionary<string, float>();
        }
        public void EndRpc()
        {
            BeansSupervisor.singleton.EndRunnerPhase();
        }

        public void LogEventOnCollisionRpc(GuyBehavior guy, NetworkObjectReference objRef)
        {
            NetworkObject result;
            objRef.TryGet(out result);

            if (result == null) throw new System.Exception("Invalid Network Object Reference");

            GuyBehavior guyIHit = result.GetComponent<GuyBehavior>();

            Debug.Log(guy.GuyName + " hit " + guyIHit.GuyName);

            Debug.Log(guy.GuyName + ": " + LeaderDict.ContainsKey(guy.GuyName));
            Debug.Log(guyIHit.GuyName + ": " + LeaderDict.ContainsKey(guyIHit.GuyName));

            if (guyIHit != null)
            {
                float value = 0;
                LeaderDict.TryGetValue(guy.GuyName, out value);

                Debug.Log("Time since last transfer: " + (Time.time - lastTransferTime));

                if ((int)value == 1 && Time.time - lastTransferTime > 0.5f)
                {
                    LeaderDict.Remove(guy.GuyName);
                    LeaderDict[guyIHit.GuyName] = 1;
                    lastTransferTime = Time.time;
                }
            }
        }

        public void LogEventOnTriggerRpc(GuyBehavior guy, NetworkObjectReference objRef)
        {
            NetworkObject result;
            objRef.TryGet(out result);

            if (result == null) throw new System.Exception("Invalid Network Object Reference");

            if (result.gameObject.tag == "RunnerCrown")
            {
                string guyName = guy.GuyName;
                if (!LeaderDict.ContainsValue(1))
                {
                    LeaderDict[guyName] = 1;
                }
                result.gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }

        public void LogEventOnDestroyedRpc(GuyBehavior guy)
        {
            LeaderDict[guy.GuyName] = 0;
        }

        public void LogEventOnUpdateRpc(GuyBehavior guy)
        {
            return;
        }
    }
}
