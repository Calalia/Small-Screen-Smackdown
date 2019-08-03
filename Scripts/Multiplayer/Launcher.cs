using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace Com.TeamFlop.MukkelisMakkelis
{
    public class Launcher : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
        public GameObject roomListContent;
        public GameObject roomInfoPrefab;
        public GameObject roomNameInputField;

        public List<GameObject> knownRooms;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        // This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).

        string gameVersion = "1";
        bool isConnecting;
        // Start is called before the first frame update
        private void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        void Start()
        {
            knownRooms = new List<GameObject>();
            progressLabel.SetActive(false);
            controlPanel.SetActive(false);
        }

        public void Connect()
        {
            isConnecting = true;
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                //PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        public void JoinRoom(string name)
        {
            PhotonNetwork.JoinRoom(name);
        }
        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(roomNameInputField.GetComponent<InputField>().text, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("OnRoomListUpdate called!");
            foreach (GameObject item in knownRooms)
            {
                Destroy(item);
            }
            foreach (RoomInfo info in roomList)
            {
                if (info.PlayerCount == 0) return;
                GameObject roominfo = Instantiate(roomInfoPrefab, roomListContent.transform);
                roominfo.SetActive(true);
                roominfo.transform.Find("RoomNameText").GetComponent<Text>().text = info.Name;
                roominfo.transform.Find("PlayerText").GetComponent<Text>().text = "Players: " + info.PlayerCount+"/2";
                roominfo.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(() => JoinRoom(info.Name));


                knownRooms.Add(roominfo);
            }

        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            PhotonNetwork.JoinLobby();
            if (isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                //PhotonNetwork.JoinRandomRoom();
            }
        }



        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }
        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            PhotonNetwork.LoadLevel("SampleScene");
        }

        #endregion
    }
}
