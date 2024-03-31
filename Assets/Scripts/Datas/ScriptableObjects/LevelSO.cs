using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Game/LevelSO", fileName = "LevelSO")]
public class LevelSO : ScriptableObject
{
    [Header("Update Game Level Data")]
    [SerializedDictionary("Level", "Require Data")]
    public SerializedDictionary<int, RequireData> dictionaryRequire;

    [Header("Update Placement Size Data")]
    [SerializedDictionary("Level", "Placement Size")]
    public SerializedDictionary<int, Vector3> dictionaryPlacementSize;

    public RequireData GetRequireData(int level)
    {
        if(dictionaryRequire.TryGetValue(level, out RequireData data)){
            return data;
        }
        return null;
    }

    public Vector3 GetPlacementSize(int level)
    {
        while(level > 1)
        {
            if (dictionaryPlacementSize.TryGetValue(level, out Vector3 size))
            {
                return size;
            }
            level--;
        }
        return new Vector3(10,1,10);
    }

}

[Serializable]
public class RequireData
{
    public int free = 1000;
    public int people = 100;
}