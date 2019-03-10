using FORGE3D;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerBuilder : MonoBehaviourPun
{
    public Text CashText;
    public int Cash { get; set; }

    public static GameObject LocalPlayerBuilder;

    [SerializeField] Building[] _buildings;

    [SerializeField] TowerPanel _towerPanel;
    [SerializeField] Building _buyBuilding;
    [SerializeField] Building _selectBuilding;

    Button _buildButton;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            PlayerBuilder.LocalPlayerBuilder = this.gameObject;
        }
        DontDestroyOnLoad(this.gameObject);

    }

    public void DepositCash(int cash)
    {
        this.Cash += cash;
        if(this.CashText != null)
        {
            this.CashText.text = Cash.ToString();
        }
    }

    private void Start()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }


        _towerPanel = FindObjectOfType<TowerPanel>();
        _towerPanel.gameObject.SetActive(false);

        _buildButton = GameObject.Find("BuildButton").GetComponent<Button>();
        _buildButton.onClick.AddListener(() => EnableBuilder(0));

        this.CashText = GameObject.Find("Cash").GetComponent<Text>();
        this.DepositCash(10000);
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }        

        if (Input.GetMouseButtonDown(0) && _buyBuilding != null)
        {
            InteractWithBoard(0);
        }
        else if(Input.GetMouseButtonDown(0) && _buyBuilding == null)
        {
            InteractWithBoard(1);
        }

        if (Input.GetMouseButton(1) && _selectBuilding != null)
        {
            InteractWithUnits();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetEnableBuilding();
            ResetSelectBuilding();
        }
    }

    void InteractWithUnits()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Layer 11 are Units
        int layerMask = 1 << 11;
        //Check if we are Hit a Unit
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                //Change Target from a Building to the Unit
                _selectBuilding.GetComponent<F3DTurret>().SetNewTargetManual(hit.transform);
            }
        }
    }

    void InteractWithBoard(int action)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Setup Layer Layer 10 is for Place Zone, Layer 9 is No Place Zone
        int boardLayer = 1 << 10;
        int noBuildLayer = 1 << 9;
        //Check if we are not on a No Build Zone
        if (!Physics.Raycast(ray, out hit, 100, noBuildLayer))
        {
            //If we are on a Build Zone
            if (Physics.Raycast(ray, out hit, 100, boardLayer))
            {
                //Calculate the Position we Hit for Mathf.Round
                Vector3 gridPosition = Board.instance.CalculateGridPosition(hit.point);
                
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (action == 0 && Board.instance.CheckForBuildingAtPosition(gridPosition) == null)
                    {
                        //When Cash More then Building Cost
                        if (Cash >= _buyBuilding.Cost)
                        {
                            //Remove Building Cost from Player
                            DepositCash(-_buyBuilding.Cost);
                            //Place Tower at calculated Position
                            Board.instance.AddBuilding(_buyBuilding.name, gridPosition);
                        }
                    }
                    else if (action == 1 && Board.instance.CheckForBuildingAtPosition(gridPosition) != null)
                    {
                        //Select Building
                        SelectBuilding(Board.instance.CheckForBuildingAtPosition(gridPosition).GetComponent<Building>());
                    }
                    else
                    {
                        //If we Hit somewhere in the Scene we simple Unselect everything
                        ResetEnableBuilding();
                        ResetSelectBuilding();
                    }
                }
            }
        }
    }

    public void SelectBuilding(Building building)
    {
        ResetEnableBuilding();
        ResetSelectBuilding();
        _selectBuilding = building;
        SelectBuildingUI(building);
        building.SelectionCircle.SetActive(true);        
    }

    void SelectBuildingUI(Building building)
    {
        _towerPanel.gameObject.SetActive(true);
        _towerPanel.SetTowerPanel(building, this);
    }

    void ResetSelectBuilding()
    {
        if (_selectBuilding != null)
        {
            _towerPanel.gameObject.SetActive(false);
            _selectBuilding.SelectionCircle.SetActive(false);
            _selectBuilding = null;
        }
    }

    void ResetEnableBuilding()
    {
        if (_buyBuilding != null)
        {
            _buyBuilding = null;
        }
    }

    public void EnableBuilder(int building)
    {
        ResetSelectBuilding();
        _buyBuilding = _buildings[building];
        Debug.Log("Selected Building: " + _buyBuilding.ToString());
    }
}
