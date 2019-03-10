using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.StusseCodes.CommanderDefense
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance;

        #region public Serializable Fields

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        [Tooltip("Unit To Spawn")]
        public GameObject unitPrefab;
        [Tooltip("Setup Gameplay")]
        public bool isCoop;


        #endregion

        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }


        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion

        #region Private Methods

        private void Start()
        {
            Instance = this;
            if (PhotonNetwork.IsMasterClient)
            {
                if (isCoop)
                {
                    PhotonNetwork.Instantiate("CoopFloor", new Vector3(15,0,15), Quaternion.identity);
                }

                GameObject go = GameObject.Find("Start");
                Spawner sp = go.AddComponent<Spawner>();
                sp.spawnTime = 2f;
                sp.spawnObject = unitPrefab;
            }

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
        
        }

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Playfield One");
        }


        #endregion
    }
}
