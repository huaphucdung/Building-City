using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoadType
{
    Default,
    Strainght,
    Corner,
    ThreeWay,
    Intersection,
}


public class RoadConnectModule : UnitModule
{
    [SerializeField] private GameObject roadDefault;
    [SerializeField] private GameObject roadStraight;
    [SerializeField] private GameObject roadCorner;
    [SerializeField] private GameObject roadThreeWay;
    [SerializeField] private GameObject roadIntersection;

    private PlacementUnit unit;
    private RoadType type;

    private GameObject _selectedObject;

    private void Awake()
    {
        unit = GetComponent<PlacementUnit>();
        type = RoadType.Default;

        ShowRoadGameObject(roadDefault);
    }

    public override void DoModule()
    {
        UpdateRoad();
    }

    public void UpdateRoad(bool updateNeighbour = true)
    {
        PlacementDirection newDirection = unit.direction;

        PlacementSystem system = unit.placementSystem;
        List<RoadConnectModule> listNeightRoad = new List<RoadConnectModule>();

        RoadConnectModule topUnit = system.GetPlacementUnit(unit.position + new Vector3Int(0, 0, 1))?.GetComponent<RoadConnectModule>(); //Top
        listNeightRoad.Add(topUnit);

        RoadConnectModule downUnit = system.GetPlacementUnit(unit.position + new Vector3Int(0, 0, -1))?.GetComponent<RoadConnectModule>(); //Down
        listNeightRoad.Add(downUnit);

        RoadConnectModule rightUnit = system.GetPlacementUnit(unit.position + new Vector3Int(1, 0, 0))?.GetComponent<RoadConnectModule>(); //Right
        listNeightRoad.Add(rightUnit);

        RoadConnectModule leftUnit = system.GetPlacementUnit(unit.position + new Vector3Int(-1, 0, 0))?.GetComponent<RoadConnectModule>(); //Left
        listNeightRoad.Add(leftUnit);

        listNeightRoad.RemoveAll(item => item == null);

        switch (listNeightRoad.Count)
        {
            case 4:
                //Intersection
                ChangRoadType(RoadType.Intersection, newDirection);
                break;
            case 3:
                //Three way
                if (!topUnit)
                {
                    newDirection = PlacementDirection.Left;
                }
                else if (!downUnit)
                {
                    newDirection = PlacementDirection.Right;
                }
                else if (!rightUnit)
                {
                    newDirection = PlacementDirection.Top;
                }
                else
                {
                    newDirection = PlacementDirection.Down;
                }
                ChangRoadType(RoadType.ThreeWay, newDirection);
                break;
            case 2:
                //Straight
                if ((topUnit && downUnit) || (leftUnit && rightUnit))
                {
                    if (topUnit)
                    {
                        newDirection = PlacementDirection.Down;
                    }
                    else
                    {
                        newDirection = PlacementDirection.Right;
                    }
                    ChangRoadType(RoadType.Strainght, newDirection);
                }

                //Corner
                else
                {
                    if (topUnit)
                    {
                        if (rightUnit)
                        {
                            newDirection = PlacementDirection.Right;
                        }
                        else
                        {
                            newDirection = PlacementDirection.Top;
                        }
                    }
                    else if (downUnit)
                    {
                        if (rightUnit)
                        {
                            newDirection = PlacementDirection.Down;
                        }
                        else
                        {
                            newDirection = PlacementDirection.Left;
                        }
                    }

                    ChangRoadType(RoadType.Corner, newDirection);
                }
                break;
            default:
                //Default
                if (topUnit)
                {
                    newDirection = PlacementDirection.Down;
                }
                else if (downUnit)
                {
                    newDirection = PlacementDirection.Top;
                }
                else if (rightUnit)
                {
                    newDirection = PlacementDirection.Left;
                }
                else if (leftUnit)
                {
                    newDirection = PlacementDirection.Right;
                }
                ChangRoadType(RoadType.Default, newDirection);
                break;
        }

        if (!updateNeighbour) return;
        //Update at neighbour unit
        topUnit?.UpdateRoad(false);
        downUnit?.UpdateRoad(false);
        rightUnit?.UpdateRoad(false);
        leftUnit?.UpdateRoad(false);
    }

    private void ChangRoadType(RoadType newType, PlacementDirection direction)
    {
        type = newType;

        switch (type)
        {
            case RoadType.Default:
                ShowRoadGameObject(roadDefault);
                break;
            case RoadType.Strainght:
                ShowRoadGameObject(roadStraight);
                break;
            case RoadType.Corner:
                ShowRoadGameObject(roadCorner);
                break;
            case RoadType.ThreeWay:
                ShowRoadGameObject(roadThreeWay);
                break;
            case RoadType.Intersection:
                ShowRoadGameObject(roadIntersection);
                break;
        }
        unit.ChangeDirection(direction);
    }

    private void ShowRoadGameObject(GameObject gameObject)
    {
        _selectedObject?.SetActive(false);
        _selectedObject = gameObject;
        _selectedObject.SetActive(true);
    }
}
