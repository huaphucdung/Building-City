using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlacemendMode
{
    None,
    Placement,
    Edit
}


public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject girdShow;
    [SerializeField] private PhysicsRaycaster cameraRaycaster;
    public GameManager gameManager { get; private set; }

    private BuidingPreview preview;
    private PlacementGird placementGrid;

    public Vector3Int currentGridPositon { get; private set; } = Vector3Int.one;
    public PlacementDirection currentDirection { get; private set; } = PlacementDirection.Down;
    public PlacemendMode currentPlacementMode { get; private set; }
    
    public string currentKey { get; private set;} 
    public BuildingData currentBuildingData { get; private set; }

    private PlacementUnit _editUnit;

    #region Unity Methods
    private void Start()
    {
        preview = GetComponent<BuidingPreview>();
        preview.Intitialize(this);

        placementGrid = new PlacementGird(this);
    }

    public void DoLateUpdate()
    {
        foreach(KeyValuePair<Vector3Int, PlacementUnit> dictionary in placementGrid.dictionary)
        {
            dictionary.Value.DoUpdateModules();
        }
    }

    #endregion

    #region Initialize Methods
    public void Initilize(GameManager manager)
    {
        gameManager = manager;
    }

    public void SetKeyAndBuildingData(string key, BuildingData data)
    {
        currentKey = key;
        currentBuildingData = data;
    }

    public (string key, BuildingData data) GetKeyAndBuildingData()
    {
        return (currentKey, currentBuildingData);
    }

    private void AddInputAction()
    {
        InputManager.MouseLeftTapAction += OnMouseLeftClick;
        InputManager.RotationAction += OnRotation;
    }

    private void RemoveInputAction()
    {
        InputManager.MouseLeftTapAction -= OnMouseLeftClick;
        InputManager.RotationAction -= OnRotation;
    }

    #endregion

    #region Main Methods

    public void ReadCellPosotion(Vector3 worldPosition)
    {
        currentGridPositon = grid.WorldToCell(worldPosition);
    }

    public Vector3 GetCenterCellPosittion(Vector3Int cellPosition)
    {
        return grid.GetCellCenterWorld(cellPosition);
    }

    public void UpdatePreview()
    {
        if (currentPlacementMode == PlacemendMode.None) return;

        bool isCollider = IsCollider(currentGridPositon, currentBuildingData.Size, currentDirection);
        preview?.UpdatePosition(GetCenterCellPosittion(currentGridPositon), currentDirection, isCollider, gameManager.IsRayCastHit);
    }

    //Start Placement
    public void StartPlacemenent()
    {
        if (string.IsNullOrEmpty(currentKey) || currentBuildingData == null) return;
        //Hide current preview
        if (currentPlacementMode == PlacemendMode.Placement) StopPlacement();
        //Show new preview
        preview.StartShowingPlacementPreview(currentBuildingData);
        ChangePlacementMode(PlacemendMode.Placement);
        AddInputAction();
    }

    //Stop Placement
    public void StopPlacement()
    {
        preview.StopShowingPlacementPreview();
        ChangePlacementMode(PlacemendMode.None);
        RemoveInputAction();
    }

    //Start Edit
    public void StartEdit(Vector3Int gridPositon)
    {
        _editUnit = placementGrid.RemoveBuildingAt(gridPositon);
        if (_editUnit == null) return;

        _editUnit.DoConnectModuel();
        _editUnit.gameObject.SetActive(false);

        currentDirection = _editUnit.direction;
        SetKeyAndBuildingData(_editUnit.key, _editUnit.data);

        preview.StartShowingPlacementPreview(_editUnit.data);
        ChangePlacementMode(PlacemendMode.Edit);

        //Hide List
        gameManager.ui.HideList(false);

        AddInputAction();
    }

    //Stop Edit
    public void StopEdit(Vector3Int gridPositon)
    {
        //Check is Collider 
        if (IsCollider(gridPositon, currentBuildingData.Size, currentDirection)) return;
        placementGrid.EditPlacementUnitAt(gridPositon, _editUnit, currentDirection);
        SetKeyAndBuildingData(null, null);
        preview.StopShowingPlacementPreview();
        ChangePlacementMode(PlacemendMode.None);

        //Show List
        gameManager.ui.ShowList();

        RemoveInputAction();
    }

    private void ChangePlacementMode(PlacemendMode newMode)
    {
        currentPlacementMode = newMode;
        cameraRaycaster.enabled = (currentPlacementMode == PlacemendMode.None) ? true : false;
    }

    public void HidePreview()
    {
        preview.HidePreview();
    }
    
    //Add Building
    public bool AddBuildingUnit(Vector3Int gridPositon, string key, BuildingData data, PlacementDirection direction)
    {
        //Check is Collider 
        if (IsCollider(gridPositon, data.Size, direction)) return false;
        //Add placment unit
        placementGrid.AddPlacementUnitAt(gridPositon, key, data, direction);
        return true;
    }

    //Delete Building
    public void DeleteBuildingUnit(Vector3Int gridPositon)
    {
        PlacementUnit unit = placementGrid.RemoveBuildingAt(gridPositon);
        unit?.DestroyUnit();
    }

    private bool IsCollider(Vector3Int gridPositon, Vector2Int size, PlacementDirection direction)
    {
        List<Vector3Int> cellsWillUse = MathForGrid.CaculateCellUse(gridPositon, size, direction);
        List<Vector3Int> ColliderList = placementGrid.GetCellsNotEmpty();

      
        return ColliderList.Intersect(cellsWillUse).Count() > 0 || !CheckInBound(cellsWillUse) ? true : false;
    }

    private bool CheckInBound(List<Vector3Int> girdCellUse)
    {
        foreach(Vector3Int cell in girdCellUse)
        {
            if(!Physics.Raycast(grid.GetCellCenterWorld(cell), Vector3.down, 100f, gameManager.GetPlacementLayers))
            {
                return false;
            }
        }
        return true;
    }

    public void ChangePlacementSize(Vector3 size)
    {
        girdShow.transform.localScale = size;
    }
    #endregion

    #region Callback Methods
    private void OnMouseLeftClick()
    {
        //Check is pointer in UI
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (currentPlacementMode == PlacemendMode.Placement)
        {
            if (gameManager.currentData.Coin < currentBuildingData.price)
            {
                return;
            }
            if(AddBuildingUnit(currentGridPositon, currentKey, currentBuildingData, currentDirection))
            {
                //Decrease coint
                gameManager.ChangeCoin(-currentBuildingData.price);
            }
        }

        if (currentPlacementMode == PlacemendMode.Edit)
        {
            StopEdit(currentGridPositon);
        }
    }
    private void OnRotation()
    {
        switch (currentDirection)
        {
            case PlacementDirection.Top:
                currentDirection = PlacementDirection.Right;
                break;
            case PlacementDirection.Right:
                currentDirection = PlacementDirection.Down;
                break;
            case PlacementDirection.Down:
                currentDirection = PlacementDirection.Left;
                break;
            case PlacementDirection.Left:
                currentDirection = PlacementDirection.Top;
                break;
        }
        UpdatePreview();
    }

    public PlacementUnit GetPlacementUnit(Vector3Int gridPosition)
    {
        return placementGrid.GetPlacementUnit(gridPosition);
    }
    #endregion
}

public class PlacementGird
{
    public Dictionary<Vector3Int, PlacementUnit> dictionary = new Dictionary<Vector3Int, PlacementUnit>();

    private PlacementSystem placementSystem;
    
    public PlacementGird(PlacementSystem system)
    {
        placementSystem = system;
    }

    //Add placement unit
    public void AddPlacementUnitAt(Vector3Int gridPosition, string key, BuildingData data, PlacementDirection direction)
    {
        //Instance unit at Grid Position
        GameObject newGameObject = GameObject.Instantiate(data.Prefab);
        Vector3 position = placementSystem.GetCenterCellPosittion(gridPosition);
        position.y = 0.05f;
        newGameObject.transform.position = position;

        //Updatdata
        PlacementUnit placmentUnit = newGameObject.GetComponent<PlacementUnit>();
        
        placmentUnit.SetData(placementSystem, gridPosition, key, data, data.Size, direction);
        
        //Add to dictionary
        dictionary.Add(gridPosition, placmentUnit);
        placmentUnit.DoInitializeUpdateModules();
        placmentUnit.DoConnectModuel();
    }

    //Edit placemet unit
    public void EditPlacementUnitAt(Vector3Int gridPosition, PlacementUnit placementUnit, PlacementDirection direction)
    {
        Vector3 position = placementSystem.GetCenterCellPosittion(gridPosition);
        position.y = 0.05f;
        placementUnit.transform.position = position;
        
        placementUnit.SetData(placementSystem, gridPosition, placementUnit.key, placementUnit.data, placementUnit.data.Size, direction);
        placementUnit.gameObject.SetActive(true);
        
        dictionary.Add(gridPosition, placementUnit);
        placementUnit.DoConnectModuel();
    }

    //Detete placement unit
    public PlacementUnit RemoveBuildingAt(Vector3Int gridPosition)
    {
        if(dictionary.TryGetValue(gridPosition, out PlacementUnit unit)){
            dictionary.Remove(gridPosition);
            return unit;
        }
        return null;
    }

    //Get List Cell
    public List<Vector3Int> GetCellsNotEmpty()
    {
        List<Vector3Int> colliderList = new List<Vector3Int>();
        foreach (var item in dictionary)
        {
            Vector3Int girdPositon = item.Key;
            colliderList.AddRange(MathForGrid.CaculateCellUse(girdPositon, item.Value.size, item.Value.direction));
        }
        return colliderList;
    }

    //Get Placement Unit in dictionary
    public PlacementUnit GetPlacementUnit(Vector3Int girdPosition)
    {
        if (dictionary.TryGetValue(girdPosition, out PlacementUnit unit))
        {
            return unit;
        }
        return null;
    }
}
public enum PlacementDirection
{
    Top,
    Left,
    Down,
    Right
}

public static class MathForGrid
{
    private const float X_OFFSET_DEFAULT = 5f;
    private const float Z_OFFSET_DEFAULT = 5f;

    public static List<Vector3Int> CaculateCellUse(Vector3Int gridPosition, Vector2Int size, PlacementDirection direction)
    {
        List<Vector3Int> colliderList = new List<Vector3Int>();
        int sizeHorizontal = (direction == PlacementDirection.Down || direction == PlacementDirection.Top) ? size.x : size.y;
        int sizeVertical = (direction == PlacementDirection.Down || direction == PlacementDirection.Top) ? size.y : size.x;

        for (int i = 0; i < sizeHorizontal; i++)
        {
            for (int j = 0; j < sizeVertical; j++)
            {
                colliderList.Add(gridPosition + new Vector3Int(i, 0, j));
            }
        }
        return colliderList;
    }

    public static Vector3 GetLocalPositionToCell(Vector2Int size, PlacementDirection direction)
    {
        float xOffset = X_OFFSET_DEFAULT * ((direction == PlacementDirection.Down || direction == PlacementDirection.Top) ? (size.x - 1) : (size.y - 1));
        float zOffset = Z_OFFSET_DEFAULT * ((direction == PlacementDirection.Down || direction == PlacementDirection.Top) ? (size.y - 1) : (size.x - 1));

        return new Vector3(xOffset, 0, zOffset);
    }

    public static Vector3 GetLocalScaleToCell(Vector2Int size, PlacementDirection direction)
    {
        float xOffset = (direction == PlacementDirection.Down || direction == PlacementDirection.Top) ? size.x : size.y;
        float zOffset = (direction == PlacementDirection.Down || direction == PlacementDirection.Top) ? size.y : size.x;
        return new Vector3(xOffset, 0, zOffset);
    }

} 