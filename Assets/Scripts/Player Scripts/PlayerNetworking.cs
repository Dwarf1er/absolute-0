﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerStats))]
public class PlayerNetworking : NetworkBehaviour
{
    //References
    [SerializeField]
    List<Behaviour> offlineComponents = new List<Behaviour>();
    [SerializeField]
    string onlineEnnemyLayerName = "Ennemy";
    [SerializeField]
    string onlineAllyLayerName = "Ally";
    Camera lobbyCamera;
    [SerializeField]
    GameObject playerUIPrefab;
    public GameObject playerUIInstance;

    void Start()
    {
        //Switches every online components to offline components if not related to the local player (added to the list in UI)
        //Adds the layer Ennemies to every online entities
        if (!isLocalPlayer)
        {
            DisableOnlineComponents();
            AssignOnlineLayers();
        }

        //Deactivate the LobbyCamera when player logs in
        else
        {
            lobbyCamera = Camera.main;

            //Prevents error if there is no lobbyCamera in the scene
            if (lobbyCamera != null)
                lobbyCamera.enabled = true;

            //Create PlayerUI when spawned
            playerUIInstance = Instantiate(playerUIPrefab, gameObject.transform);
            playerUIInstance.name = playerUIPrefab.name;

            //Configuration of the PlayerUI
            PlayerUI playerUi = playerUIInstance.GetComponent<PlayerUI>();

            //Checks if the playerUI exists
            if (playerUi == null)
                Debug.LogError("No playerUI on the playerUI prefab");

            else
                playerUi.SetPlayer(GetComponent<PlayerStats>());

            //Set references for other scripts
            GetComponent<PlayerStats>().SetPlayerStats();
            GetComponent<PlayerShootingController>().playerUI = playerUi;

        }

        //Gives the player a unique identifier
        EnlistPlayer();
    }

    void DisableOnlineComponents()
    {
        foreach (Behaviour putOffline in offlineComponents)
            putOffline.enabled = false;
    }

    void AssignOnlineLayers()
    {
        //Assigns the layer Ally to all non-LocalPlayer gameobjects tagged as Player(s)
        if (gameObject.tag == "Player")
            gameObject.layer = LayerMask.NameToLayer(onlineAllyLayerName);

        //Assigns the layer Ennemy to all non-localPlayer gameobjects that are not tagged tagged as Player(s)
        else
            gameObject.layer = LayerMask.NameToLayer(onlineEnnemyLayerName);
    }

    void EnlistPlayer()
    {
        //Gives the player a unique identifier using his netID
        string playerID = "Player" + GetComponent<NetworkIdentity>().netId;
        transform.name = playerID;
    }

    //Reactivate the LobbyCamera
    void OnDisable()
    {
        //Removes the player UI on death
        Destroy(playerUIInstance);

        if (lobbyCamera != null)
            lobbyCamera.enabled = true;
    }
}
