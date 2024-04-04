using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

namespace GoalModes {
    public class Runner : IGoalMode
    {
        public Dictionary<string, float> LeaderDict { get; set; }
        public string Unit { get { return "rad crown"; } }

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
            return;
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
