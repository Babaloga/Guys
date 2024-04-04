using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Lexone.UnityTwitchChat;

public class TwitchIntegration : MonoBehaviour
{
    public GameObject ufoPrefab;
    IRC twitchIRC;

    // Start is called before the first frame update
    void Start()
    {
        twitchIRC = GetComponent<IRC>();
        twitchIRC.OnChatMessage += ChatRecieved;
    }

    public void ChatRecieved(Chatter message)
    {
        foreach(GuyBehavior gb in GuyBehavior.activeGuys)
        {
            if (message.message.Contains(gb.GuyName))
            {
                NetworkObject ufo = Instantiate(ufoPrefab, new Vector3(0, 0, -50), Quaternion.identity).GetComponent<NetworkObject>();
                ufo.Spawn();
                ufo.GetComponent<UFOBehavior>().AcquireTarget(gb.transform);
                return;
            }
        }
    }
}
