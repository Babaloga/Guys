using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using Unity.Netcode;
using System;

public class HelloWorldManager : MonoBehaviour
{
    public bool host = false;
    public bool server = false;

    string caCert = @"-----BEGIN CERTIFICATE-----
MIIFFjCCAv6gAwIBAgIRAJErCErPDBinU/bWLiWnX1owDQYJKoZIhvcNAQELBQAw
TzELMAkGA1UEBhMCVVMxKTAnBgNVBAoTIEludGVybmV0IFNlY3VyaXR5IFJlc2Vh
cmNoIEdyb3VwMRUwEwYDVQQDEwxJU1JHIFJvb3QgWDEwHhcNMjAwOTA0MDAwMDAw
WhcNMjUwOTE1MTYwMDAwWjAyMQswCQYDVQQGEwJVUzEWMBQGA1UEChMNTGV0J3Mg
RW5jcnlwdDELMAkGA1UEAxMCUjMwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEK
AoIBAQC7AhUozPaglNMPEuyNVZLD+ILxmaZ6QoinXSaqtSu5xUyxr45r+XXIo9cP
R5QUVTVXjJ6oojkZ9YI8QqlObvU7wy7bjcCwXPNZOOftz2nwWgsbvsCUJCWH+jdx
sxPnHKzhm+/b5DtFUkWWqcFTzjTIUu61ru2P3mBw4qVUq7ZtDpelQDRrK9O8Zutm
NHz6a4uPVymZ+DAXXbpyb/uBxa3Shlg9F8fnCbvxK/eG3MHacV3URuPMrSXBiLxg
Z3Vms/EY96Jc5lP/Ooi2R6X/ExjqmAl3P51T+c8B5fWmcBcUr2Ok/5mzk53cU6cG
/kiFHaFpriV1uxPMUgP17VGhi9sVAgMBAAGjggEIMIIBBDAOBgNVHQ8BAf8EBAMC
AYYwHQYDVR0lBBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMBMBIGA1UdEwEB/wQIMAYB
Af8CAQAwHQYDVR0OBBYEFBQusxe3WFbLrlAJQOYfr52LFMLGMB8GA1UdIwQYMBaA
FHm0WeZ7tuXkAXOACIjIGlj26ZtuMDIGCCsGAQUFBwEBBCYwJDAiBggrBgEFBQcw
AoYWaHR0cDovL3gxLmkubGVuY3Iub3JnLzAnBgNVHR8EIDAeMBygGqAYhhZodHRw
Oi8veDEuYy5sZW5jci5vcmcvMCIGA1UdIAQbMBkwCAYGZ4EMAQIBMA0GCysGAQQB
gt8TAQEBMA0GCSqGSIb3DQEBCwUAA4ICAQCFyk5HPqP3hUSFvNVneLKYY611TR6W
PTNlclQtgaDqw+34IL9fzLdwALduO/ZelN7kIJ+m74uyA+eitRY8kc607TkC53wl
ikfmZW4/RvTZ8M6UK+5UzhK8jCdLuMGYL6KvzXGRSgi3yLgjewQtCPkIVz6D2QQz
CkcheAmCJ8MqyJu5zlzyZMjAvnnAT45tRAxekrsu94sQ4egdRCnbWSDtY7kh+BIm
lJNXoB1lBMEKIq4QDUOXoRgffuDghje1WrG9ML+Hbisq/yFOGwXD9RiX8F6sw6W4
avAuvDszue5L3sz85K+EC4Y/wFVDNvZo4TYXao6Z0f+lQKc0t8DQYzk1OXVu8rp2
yJMC6alLbBfODALZvYH7n7do1AZls4I9d1P4jnkDrQoxB3UqQ9hVl3LEKQ73xF1O
yK5GhDDX8oVfGKF5u+decIsH4YaTw7mP3GFxJSqv3+0lUFJoi5Lc5da149p90Ids
hCExroL1+7mryIkXPeFM5TgO9r0rvZaBFOvV2z0gp35Z0+L4WPlbuEjN/lxPFin+
HlUjr8gRsI3qfJOQFy/9rKIJR0Y/8Omwt/8oTWgy1mdeHmmjk7j1nYsvC9JSQ6Zv
MldlTTKB3zhThV1+XWYp6rjd5JW1zbVWEkLNxE7GJThEUG3szgBVGP7pSWTUTsqX
nLRbwHOoq7hHwg==
-----END CERTIFICATE-----";

//private void Setup()
//{
//    NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
//    NetworkManager.Singleton.StartHost();
//}

//private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
//{
//    // The client identifier to be authenticated
//    var clientId = request.ClientNetworkId;

//    // Additional connection data defined by user code
//    var connectionData = request.Payload;

//    // Your approval logic determines the following values
//    response.Approved = true;
//    response.CreatePlayerObject = true;

//    // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
//    response.PlayerPrefabHash = null;

//    // Position to spawn the player object (if null it uses default of Vector3.zero)
//    response.Position = Vector3.zero;

//    // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
//    response.Rotation = Quaternion.identity;

//    // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
//    // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
//    response.Reason = "Some reason for not approving the client";

//    // If additional approval steps are needed, set this to true until the additional steps are complete
//    // once it transitions from true to false the connection approval response will be processed.
//    response.Pending = false;
//}

private void Start()
    {
        if (server)
        {
            //var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as Unity.Netcode.Transports.UTP.UnityTransport;
            //string privateKey = File.ReadAllText("Y:\\GuysPrivateKey.txt");
            //string cert = File.ReadAllText("Y:\\GuysCert.txt");
            //transport.SetServerSecrets(cert, privateKey);
            //Setup();
            //NetworkManager.Singleton.NetworkConfig.NetworkTransport = transport;
            NetworkManager.Singleton.StartServer();
        }
        else if (host)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            //print(Dns.GetHostEntry("play.babaloga.me").AddressList[0].ToString());
            //print("First DNS resolve passed");

            //var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as Unity.Netcode.Transports.UTP.UnityTransport;
            //transport.SetClientSecrets("play.babaloga.me");
            //transport.SetConnectionData(Dns.GetHostEntry("play.babaloga.me").AddressList[0].ToString(), 7878, "0.0.0.0");
            //NetworkManager.Singleton.NetworkConfig.NetworkTransport = transport;
            //StartCoroutine(TryConnect());
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as Unity.Netcode.Transports.UTP.UnityTransport;

            print(Dns.GetHostEntry("play.babaloga.me").AddressList[0].ToString());
            transport.ConnectionData.Address = Dns.GetHostEntry("play.babaloga.me").AddressList[0].ToString();

            NetworkManager.Singleton.StartClient();
        }
    }

    IEnumerator TryConnect()
    {
        bool succeeded = false;

        while (!succeeded)
        {
            try
            {
                //print(Dns.GetHostEntry("play.babaloga.me").AddressList[0].ToString());
                //print("First DNS resolve passed");

                var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as Unity.Netcode.Transports.UTP.UnityTransport;
                transport.SetClientSecrets("play.babaloga.me");
                //transport.SetConnectionData("173.73.95.133", 7878, "0.0.0.0");

                NetworkManager.Singleton.StartClient();
                succeeded = true;
            }
            catch(Exception e)
            {
                print(e.Message);
                
            }
            yield return new WaitForSeconds(2);
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
