using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementUnit : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private UnitModule connectModule;
    [SerializeField] private List<UnitModule> updateModules;
    public PlacementSystem placementSystem { get; private set; }
    public string key { get; private set; }
    public BuildingData data { get; private set; }
    public PlacementDirection direction { get; private set; } = PlacementDirection.Down;
    public Vector2Int size { get; private set; } = Vector2Int.one;
    public Vector3Int position { get; private set; }

    public Vector3 worldPositon { get; private set; }
    private float _timeCheck;
    
    public void SetData(PlacementSystem system, Vector3Int gridPositon, string key, BuildingData data, Vector2Int size, PlacementDirection direction)
    {
        placementSystem = system;
        position = gridPositon;
        this.key = key;
        this.data = data;
        this.size = size;
        ChangeDirection(direction);
    }

    public void SetDataForPreview(PlacementSystem system, Vector2Int size, PlacementDirection direction)
    {
        placementSystem = system;
        this.size = size;
        ChangeDirection(direction);
    }

    public void DestroyUnit()
    {
        DoConnectModuel();
        DoDestoryUPdateModule();
        placementSystem.gameManager.ChangeCoin(data.price / 2);
        Destroy(gameObject);
    }

    public void ChangeDirection(PlacementDirection newDirecion)
    {
        direction = newDirecion;
        ChangeLocalPositionAndRotation();
    }

    public void DoConnectModuel()
    {
        connectModule?.DoModule();
    }

    public void DoInitializeUpdateModules()
    {
        foreach(var module in updateModules)
        {
            module?.DoInitialize();
        }
    }

    public void DoUpdateModules()
    {
        foreach (var module in updateModules)
        {
            module?.DoModule();
        }
    }

    private void DoDestoryUPdateModule()
    {
        foreach (var module in updateModules)
        {
            module?.DoDestroy();
        }
    }

    private void ChangeLocalPositionAndRotation()
    {
        Vector3 newPosiotn = MathForGrid.GetLocalPositionToCell(size, direction);
       
        Transform[] childs = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in childs)
        {
            if (child == transform) continue;
            child.transform.localPosition = newPosiotn;

            switch (direction)
            {
                case PlacementDirection.Top:
                    child.transform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                    break;
                case PlacementDirection.Left:
                    child.transform.localRotation = Quaternion.LookRotation(-Vector3.right, Vector3.up);
                    break;
                case PlacementDirection.Down:
                    child.transform.localRotation = Quaternion.LookRotation(-Vector3.forward, Vector3.up);
                    break;
                case PlacementDirection.Right:
                    child.transform.localRotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                    break;
            }

        }
        worldPositon = childs[1].transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _timeCheck = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.time - _timeCheck > 0.16f) return;  
        
        if (placementSystem.gameManager.currentGameMode == GameMode.Play) return;
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (placementSystem.currentPlacementMode == PlacemendMode.Placement) return;
            placementSystem.StartEdit(position);
            placementSystem.UpdatePreview();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (placementSystem.currentPlacementMode == PlacemendMode.Placement) return;
            placementSystem.DeleteBuildingUnit(position);
        }
    }

    
}
