using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/PlayerDataSO", fileName = "PlayerDataSO")]
public class PlayerDataSO : ScriptableObject
{
    public int CityLevel = 1;
    public string CityName = "";
    public int Coin = 0;
    
    [SerializedDictionary("GridPosition", "Unit Data")]
    public SerializedDictionary<Vector3Int, UnitData> dictionaryUnitData;

    public void LoadData()
    {

    }

    public void SaveData()
    {

    }
}

[Serializable]
public struct UnitData
{
    public string key;
    public PlacementDirection direction;
}