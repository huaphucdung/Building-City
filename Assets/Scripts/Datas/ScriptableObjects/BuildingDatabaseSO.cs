using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Databases/BuidingDatabaseSO", fileName = "BuildingDatabaseSO")]
public class BuildingDatabaseSO : ScriptableObject
{

    [SerializedDictionary("ID", "Building Data")]
    public SerializedDictionary<string, BuildingData> dictionaryData;

    public BuildingData GetBuildingDataByKey(string key)
    {
        if(dictionaryData.TryGetValue(key, out BuildingData data))
        {
            return data;
        }
        return null;
    }
}

[Serializable]
public class BuildingData
{
    public string Name;
    public Vector2Int Size = Vector2Int.one;
    public int price = 100;
    public BuildingType type = BuildingType.Shop;
    public GameObject Prefab;
}

public enum BuildingType
{
    Shop,
    House,
    Decorate
}