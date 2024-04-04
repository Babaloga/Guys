using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace GoalModes
{
    public class Absorption : IGoalMode
    {
        public Dictionary<string, float> LeaderDict { get; set; }

        public string Unit { get { return "Guys™ per Guy™"; } }

        public void EndRpc()
        {
            return;
        }

        public void InitRpc()
        {
            LeaderDict = new Dictionary<string, float>();
        }

        public void LogEventOnCollisionRpc(GuyBehavior guy, NetworkObjectReference objRef)
        {
            NetworkObject result;
            objRef.TryGet(out result);

            if (result == null) throw new System.Exception("Invalid Network Object Reference");

            GuyBehavior otherGuy = result.GetComponent<GuyBehavior>();

            if (otherGuy != null)
            {
                if(guy.transform.position.y > otherGuy.transform.position.y)
                {
                    guy.GetComponent<Rigidbody>().mass += 0.075f;
                    guy.SetScaleRpc(guy.transform.localScale + Vector3.one * (0.25f + (otherGuy.transform.localScale.y - 1)));

                    float value = 1;
                    LeaderDict.TryGetValue(guy.GuyName, out value);
                    Debug.Log(value);
                    if (value == 0) value = 1;

                    float otherValue = 0;
                    LeaderDict.TryGetValue(otherGuy.GuyName, out otherValue);
                    if (otherValue == 0) otherValue = 1;
                    
                    LeaderDict[guy.GuyName] = value + otherValue;

                    if(value == 5)
                    {
                        guy.guyNametag.text = "Big " + guy.GuyName;
                    }
                }
                else if (guy.transform.position.y < otherGuy.transform.position.y)
                {
                    guy.KillAndRespawnRpc();
                }
            }
        }

        public void LogEventOnDestroyedRpc(GuyBehavior guy)
        {
            return;
        }

        public void LogEventOnTriggerRpc(GuyBehavior guy, NetworkObjectReference objRef)
        {
            return;
        }

        public void LogEventOnUpdateRpc(GuyBehavior guy)
        {
            return;
        }
    }
}
