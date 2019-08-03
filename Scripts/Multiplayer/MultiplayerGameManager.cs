using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
public enum Faction
{
    Skeleton,
    Angel,
    Sorceress
}
public class MultiplayerGameManager : MonoBehaviourPunCallbacks
{
    public static MultiplayerGameManager instance;

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;
    #region Photon Callbacks

    public GameObject player1;
    public GameObject player2;
    public Faction player1Faction = Faction.Sorceress;
    public Faction player2Faction = Faction.Sorceress;

    public string[] towerPrefabs;
    public string[] basePrefabs;
    public string[] playerPrefabs;

    private bool started = false;

    private void Start()
    {
        #region Singleton Setup
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(this);
        }
        #endregion

        if (!PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.OfflineMode = true;
            player2 = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 25f), Quaternion.identity, 0);
        }
        else
        {

            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            GameObject player = null;
            if (PlayerData.instance != null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene().name);
                player = PhotonNetwork.Instantiate(playerPrefabs[(int)PlayerData.instance.faction], new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                if (PhotonNetwork.IsMasterClient)
                {
                    player1 = player;
                    player1Faction = PlayerData.instance.faction;
                }
                else
                {
                    photonView.RPC("RpcRegisterPlayer", RpcTarget.MasterClient,(int) PlayerData.instance.faction);
                    player2 = player;
                    foreach (GameObject item in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        if (player2 != item) player1 = item;
                    }
                }
            }
        }

    }
    private void Update()
    {
        if (player1 == null)
        {
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player2 != item) player1 = item;
            }
        }
        if (PhotonNetwork.IsMasterClient && !started && player2 != null)
        {
            SetupGameArea();
            started = true;
        }
    }
    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    [PunRPC]
    public void RpcRegisterPlayer(int faction)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player1 != item) player2 = item;
            }
            player2Faction = (Faction)faction;
        }
    }

    #endregion
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void SetupGameArea()
    {
        GameObject[] all = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (GameObject a in all)
        {
            int puoli = 0;
            if (a.transform.parent.name == "Puoli 1")
            {
                puoli = 1;
            }
            else if (a.transform.parent.name == "Puoli 2")
            {
                puoli = 2;
            }
            else
            {
                return;
            }
            if (a.name.Contains("torni"))
            {
                GameObject obj = PhotonNetwork.Instantiate(towerPrefabs[puoli == 1 ? (int)player1Faction : (int)player2Faction], a.transform.position, a.transform.rotation);
                obj.GetComponent<Healthbar>().photonView.RPC("RpcSetSide", RpcTarget.All, puoli);
            }
            else if (a.name.Contains("Linna"))
            {
                GameObject obj = PhotonNetwork.Instantiate(basePrefabs[puoli == 1 ? (int)player1Faction : (int)player2Faction], a.transform.position, a.transform.rotation);
                obj.GetComponent<Healthbar>().photonView.RPC("RpcSetSide", RpcTarget.All, puoli);
            }
            else if (a.name.Contains("Pelaaja"))
            {
                GameObject obj = (puoli == 1 ? player1 : player2);
                obj.GetComponent<Healthbar>().photonView.RPC("RpcSetSide", RpcTarget.All, puoli);
                obj.GetComponent<Healthbar>().photonView.RPC("RpcSetPositionAndRotation", RpcTarget.All, a.transform.position, a.transform.rotation);
            }
        }
    }
    public void EndGame(int losingSide)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerData.instance.gamewon = 3 - losingSide;
        }
        else
        {
            PlayerData.instance.gamewon =losingSide;
        }
        LeaveRoom();
    }
}
