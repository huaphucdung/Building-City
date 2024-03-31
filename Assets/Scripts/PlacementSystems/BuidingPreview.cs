using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class BuidingPreview : MonoBehaviour
{
    [SerializeField] private GameObject previewCell;

    [SerializeField] private float yOffset = 0.06f;
    [SerializeField] private Material previewMaterial;

    [SerializeField] private Color placementColor;
    [SerializeField] private Color colliderColor;

    private Material _previewMaterial;
    private PlacementUnit previewUnit;

    public PlacementSystem placementSystem { get; private set;}

    private void Start()
    {
        _previewMaterial = new Material(previewMaterial);
    }

    public void Intitialize(PlacementSystem system)
    {
        placementSystem = system;
    }

    public void StartShowingPlacementPreview(BuildingData data)
    {
        previewUnit = Instantiate(data.Prefab).GetComponent<PlacementUnit>();
        PrepareObjectPreview(data);

        ShowPreviewObject(false);
        ShowCursor(false);
    }

    public void StopShowingPlacementPreview()
    {
        if (previewUnit == null) return;
        Destroy(previewUnit.gameObject);
        previewUnit = null;
        ShowPreviewObject(false);
        ShowCursor(false);
    }

    private void ShowPreviewObject(bool value)
    {
        previewUnit?.gameObject.SetActive(value);
    }

    private void ShowCursor(bool value)
    {
        previewCell?.SetActive(value);
    }

    public void HidePreview()
    {
        ShowPreviewObject(false);
        ShowCursor(false);
    }

    public void UpdatePosition(Vector3 position, PlacementDirection direction, bool isCollider, bool IsRayCastHit)
    {
        ShowPreviewObject(IsRayCastHit);
        ShowCursor(IsRayCastHit);

        //Update Preivew Object
        UpdatePreviewObject(position, direction, isCollider);
        //Update Cursor
        UpdateCursor(position, direction, isCollider);
    }

    private void UpdatePreviewObject(Vector3 position, PlacementDirection direction, bool isCollider)
    {
        position.y = yOffset;
        if (previewUnit) previewUnit.transform.position = position;
        //Update Rotation by direction
        previewUnit.ChangeDirection(direction);
        //Change material color
        _previewMaterial.color = isCollider ? colliderColor : placementColor;
    }

    private void UpdateCursor(Vector3 position, PlacementDirection direction, bool isCollider)
    {
        previewCell.transform.position = position;
        Transform childTransform = previewCell.transform.GetChild(0);
        if (previewUnit)
        {
            childTransform.localPosition = MathForGrid.GetLocalPositionToCell(previewUnit.size, direction);
            childTransform.localScale = MathForGrid.GetLocalScaleToCell(previewUnit.size, direction);
        }

        //Change material color
        previewCell.GetComponentInChildren<Renderer>().material.color = isCollider ? colliderColor : placementColor;
    }

    private void PrepareObjectPreview(BuildingData data)
    {
        previewUnit.SetDataForPreview(placementSystem, data.Size, PlacementDirection.Down);

        //Change Render
        Renderer[] renderers = previewUnit.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = _previewMaterial;
            }
            renderer.materials = materials;
        }
    }
}
