using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolingManager : MonoBehaviour
{
    [SerializeField] private RewardDialougeUI rewardUIPrefab;
    [SerializeField] private Transform parentRewardUIPool;
    private ObjectPool<RewardDialougeUI> rewardDialougeUIPool;

    public void Initialzie()
    {
        rewardDialougeUIPool = new ObjectPool<RewardDialougeUI>(CreateDialouge, OntakeDialougeFromPool, OnReturnDialougeFormPool, null);
    }

    #region RewardPool Method
    private RewardDialougeUI CreateDialouge()
    {
        return Instantiate(rewardUIPrefab, parentRewardUIPool);
    }

    private void OntakeDialougeFromPool(RewardDialougeUI obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void OnReturnDialougeFormPool(RewardDialougeUI obj)
    {
        obj.gameObject.SetActive(false);
    }

    public RewardDialougeUI GetDialougeUI()
    {
        return rewardDialougeUIPool.Get();
    }

    public void ReleaseDialougeUI(RewardDialougeUI obj)
    {
        rewardDialougeUIPool.Release(obj);
    } 
    #endregion
}
