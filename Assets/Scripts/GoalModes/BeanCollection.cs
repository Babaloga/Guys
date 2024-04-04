using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

namespace GoalModes {
    public class BeanCollection : IGoalMode
    {
        public Dictionary<string, float> LeaderDict { get; set; }
        public string Unit { get { return "\"beans\""; } }

        public void InitRpc()
        {
            BeansSupervisor.singleton.BeginBeanPhase();
            LeaderDict = new Dictionary<string, float>();
        }
        public void EndRpc()
        {
            BeansSupervisor.singleton.EndBeanPhase();
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


            if (result.gameObject.GetComponent<Beans>())
            {
                string guyName = guy.GuyName;
                float count = 0;
                LeaderDict.TryGetValue(guyName, out count);
                LeaderDict[guyName] = count + 1;
                result.gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }

        public void LogEventOnDestroyedRpc(GuyBehavior guy)
        {
            return;
        }

        public void LogEventOnUpdateRpc(GuyBehavior guy)
        {
            return;
        }
    }
}
