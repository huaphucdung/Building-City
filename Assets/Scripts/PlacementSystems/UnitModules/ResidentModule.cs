using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidentModule : UnitModule
{
    [SerializeField] private int numberResidents;

    private PlacementUnit unit;
    private void Awake()
    {
        unit = GetComponent<PlacementUnit>();
    }

    public override void DoInitialize()
    {
        unit.placementSystem.gameManager.ChangeNumberPeople(numberResidents);
    }


    public override void DoDestroy()
    {
        unit.placementSystem.gameManager.ChangeNumberPeople(-numberResidents);
    }
}
