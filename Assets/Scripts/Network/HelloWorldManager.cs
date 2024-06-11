using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;
using System.Net;
using System;
using Unity.VisualScripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
#if UNITY_WEBGL
            StartCoroutine(WebGLConnect(transport));
#else
            transport.ConnectionData.Address = Dns.GetHostEntry("play.babaloga.me").AddressList[0].ToString();
            NetworkManager.Singleton.StartClient();
#endif

        }
    }

    IEnumerator WebGLConnect(Unity.Netcode.Transports.UTP.UnityTransport transport)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://dns.google/resolve?name=play.babaloga.me"))
        {
            webRequest.SetRequestHeader("accept", "application/dns-message");

            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    string jsonString = webRequest.downloadHandler.text;
                    Debug.Log("Received: " + jsonString);



                    JObject recievedJson = JObject.Parse(jsonString);

                    //print((string)recievedJson.Root["answer"]);

                    string ip = (string)recievedJson["Answer"][0]["data"];

                    print(ip);
                    transport.ConnectionData.Address = ip;
                    NetworkManager.Singleton.StartClient();

                    break;
            }

            //webRequest.SetRequestHeader("accept", "application/dns-message");
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
