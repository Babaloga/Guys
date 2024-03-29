using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeathZoneBehavior : NetworkBehaviour
{
    public GameObject playerPrefab;
    public static GameObject playerPrefabStatic;

    private void Start()
    {
        playerPrefabStatic = playerPrefab;
    }


}
