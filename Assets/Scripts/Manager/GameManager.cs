using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum GameMode
{
    Play,
    Edit
}

public class GameManager : MonoBehaviour
{

    [Header("SO References:")]
    [SerializeField] private PlayerDataSO defaultData;
    [SerializeField] private BuildingDatabaseSO buildingDatabaseSO;
    [SerializeField] private LevelSO levelSO;

    [Header("Rreferences:")]
    [SerializeField] private GameObject GridCollider;
    [SerializeField] private LayerMask placementLayers;
    [SerializeField] public GameplayUI ui;
    [SerializeField] public LevelUpgradeUI upgradeUI;
    [SerializeField] private CameraMovement cameraMovement;
    public PlacementSystem placementSystem { get; private set; }
    public PoolingManager poolManger { get; private set; }
    public LayerMask GetPlacementLayers => placementLayers;
    public GameMode currentGameMode { get; private set; }

    public PlayerDataSO currentData { get; private set; }

    public event Action<bool> ChangeModeAction;
    //Data
    public int numberPeople { get; private set; }
    public bool IsRayCastHit { get; private set; }

    private Material GridMaterial => GridCollider.GetComponent<Renderer>().material;

    #region Unity Methods
    private void Start()
    {
        Initialize();
        
        LoadDataForUI();

        LoadSaveData();

        currentGameMode = GameMode.Play;
    }

    private void LateUpdate()
    {
        placementSystem?.DoLateUpdate();
    }
    #endregion

    #region Intialize Methods
    private void Initialize()
    {
        //Init Data
        currentData = Instantiate(defaultData);

        //Init InputManager
        InputManager.Initialize();
        AddInputAction();

        cameraMovement.Initialize();
        //Init Placement System
        placementSystem = GetComponent<PlacementSystem>();
        placementSystem.Initilize(this);

        //Init Pool Manager
        poolManger = GetComponent<PoolingManager>();
        poolManger.Initialzie();
    }

    private void LoadSaveData()
    {
        //Check has save

        //Load Save Data

        //Change size by Level
        placementSystem.ChangePlacementSize(levelSO.GetPlacementSize(currentData.CityLevel));

        //Load Building Data from Save
        foreach (KeyValuePair<Vector3Int, UnitData> unitData in currentData.dictionaryUnitData)
        {
            BuildingData data = buildingDatabaseSO.GetBuildingDataByKey(unitData.Value.key);
            if (data == null) continue;

            placementSystem.AddBuildingUnit(unitData.Key, unitData.Value.key, data, unitData.Value.direction);
        }
    }

    private void LoadDataForUI()
    {
        ui?.Initialize(this);

        ui?.SetData(currentData.CityLevel, currentData.CityName, numberPeople, currentData.Coin);

        foreach (var data in buildingDatabaseSO.dictionaryData)
        {
            ui?.AddBuildingItem(data.Key, data.Value, OnUIItemClicked);
        }
    }

    private void AddInputAction()
    {
        InputManager.MouseMoveAction += SelectCellPosition;
        ui.OnShowBuildingListAction += ChangeMode;
    }

    private void RemoveInputActoin()
    {
        InputManager.MouseMoveAction -= SelectCellPosition;
        ui.OnShowBuildingListAction -= ChangeMode;
    }

    #endregion

    #region Main Methods
    private void SelectCellPosition(Vector2 mousePosition)
    {
        //Check is pointer in UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            placementSystem?.HidePreview();
            return;
        }

        Vector3 mapPosition = GetSelectedMapPosition(mousePosition);
        placementSystem?.ReadCellPosotion(mapPosition);
        placementSystem?.UpdatePreview();
    }

    private void ChangeMode(bool value)
    {
        ChangeModeAction?.Invoke(value);

        currentGameMode = (value) ? GameMode.Edit : GameMode.Play;
        GridMaterial.SetFloat("_Alpha", (value) ? 1 : 0.1f);
        if (value) return;
        placementSystem.SetKeyAndBuildingData(null, null);
        placementSystem.StopPlacement();
    }

    //Raycast into map to get position
    private Vector3 GetSelectedMapPosition(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayers))
        {
            IsRayCastHit = true;
            return hit.point;
        }
        IsRayCastHit = false;
        return Vector3.zero;
    }

    public void ChangeCoin(int coin)
    {
        currentData.Coin += coin;
        ui?.SetData(currentData.CityLevel, currentData.CityName, numberPeople, currentData.Coin);
    }

    public void ChangeNumberPeople(int people)
    {
        numberPeople += people;
        ui?.SetData(currentData.CityLevel, currentData.CityName, numberPeople, currentData.Coin);
    }

    public void ShowUpgradeUI()
    {
        int nextLevel = currentData.CityLevel + 1;
        RequireData requireData = levelSO.GetRequireData(nextLevel);

        //SetData
        upgradeUI.SetData(nextLevel, requireData, UpgradeCitySuccess);

        if (requireData == null)
        {
            upgradeUI.SetCanUpgrade(false);
        }
        else
        {
            upgradeUI.SetCanUpgrade((numberPeople >= requireData.people && currentData.Coin >= requireData.free));
        }
        upgradeUI.gameObject.SetActive(true);
    }

    public void UpgradeCitySuccess()
    {
        upgradeUI.gameObject.SetActive(false);
        currentData.CityLevel += 1;
        placementSystem.ChangePlacementSize(levelSO.GetPlacementSize(currentData.CityLevel));
        RequireData requireData = levelSO.GetRequireData(currentData.CityLevel);

        if(requireData != null)
        {
            ChangeCoin(-requireData.free);
        }

    }
    #endregion

    #region Callback Methods
    //UI callback event
    private void OnUIItemClicked(object obj)
    {
        if (currentGameMode == GameMode.Play) return;

        string key = obj.ToString();
        BuildingData data = buildingDatabaseSO.GetBuildingDataByKey(key);
        if (data == null) return;

        (string key, BuildingData data) oldData = placementSystem.GetKeyAndBuildingData();
        if (key.Equals(oldData.key) && data == oldData.data)
        {
            placementSystem.SetKeyAndBuildingData(null, null);
            placementSystem.StopPlacement();
            return;
        }
        placementSystem.SetKeyAndBuildingData(key, data);
        placementSystem.StartPlacemenent();
    }
    #endregion
}
