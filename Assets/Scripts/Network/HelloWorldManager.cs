using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;

public class HelloWorldManager : MonoBehaviour
{
    public bool host = false;
    public bool server = false;

    private void Start()
    {
        if (server)
        {
            NetworkManager.Singleton.StartServer();
        }
        else if (host)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as Unity.Netcode.Transports.UTP.UnityTransport;

            print(Dns.GetHostEntry("play.babaloga.me").AddressList[0].ToString());
            transport.ConnectionData.Address = Dns.GetHostEntry("play.babaloga.me").AddressList[0].ToString();
            NetworkManager.Singleton.StartClient();
        }
    }

    void OnGUI()
    {
        
        if (host)
        {
            GUILayout.Label("Host");
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }
        else if (server)
        {
            GUILayout.Label("S");
        }
    }

    static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
}
