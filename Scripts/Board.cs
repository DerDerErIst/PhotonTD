using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviourPun
{
    #region Singleton
    public static Board instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Private Fields
    GameObject[,] _buildings = new GameObject[100, 100];
    #endregion

    #region Public Methods
    public void AddBuilding(string building, Vector3 position)
    {
        _buildings[(int)position.x, (int)position.z] = PhotonNetwork.Instantiate(building, position, Quaternion.identity);
    }

    public GameObject CheckForBuildingAtPosition(Vector3 position)
    {
        return _buildings[(int)position.x, (int)position.z];
    }

    public void RemoveBuilding(Vector3 position)
    {
        PhotonNetwork.Destroy(_buildings[(int)position.x, (int)position.z]);
        _buildings[(int)position.x, (int)position.z] = null;
    }

    public Vector3 CalculateGridPosition(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), 1f, Mathf.Round(position.z));
    }
    #endregion
}
