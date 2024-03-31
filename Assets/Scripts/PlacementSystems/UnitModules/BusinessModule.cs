using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BusinessModule : UnitModule, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int coinReward;
    [SerializeField] private int timeWait;

    private PlacementUnit unit;
    private DateTime _endTime;
    private float _timeCheck;

    private bool _canGetReward;

    private RewardDialougeUI t_rewardDialougeUI;
    private void Awake()
    {
        unit = GetComponent<PlacementUnit>();
    }

    public override void DoInitialize()
    {
        _endTime = DateTime.Now.AddSeconds(timeWait);
        unit.placementSystem.gameManager.ChangeModeAction += OnGameChangeMode;
    }

    public override void DoDestroy()
    {
        unit.placementSystem.gameManager.ChangeModeAction -= OnGameChangeMode;
        if (t_rewardDialougeUI == null) return;
        unit.placementSystem.gameManager.poolManger.ReleaseDialougeUI(t_rewardDialougeUI);
    }

    public override void DoModule()
    {
        if (_canGetReward) return;
        
        TimeSpan difference = _endTime - DateTime.Now;
        float floatDifference = (float)difference.TotalSeconds;

        t_rewardDialougeUI?.UpdateTime(RemapToPercent(floatDifference, 0, timeWait), floatDifference);

        if (floatDifference <= 0 && unit.placementSystem.gameManager.currentGameMode == GameMode.Play)
        {
            _canGetReward = true;
            if (t_rewardDialougeUI == null)
            {
                t_rewardDialougeUI = unit.placementSystem.gameManager.poolManger.GetDialougeUI();
                t_rewardDialougeUI.SetData(unit.worldPositon, coinReward);
                t_rewardDialougeUI.UpdateTime(1, 0);
            }
        }

        
    }

    private float RemapToPercent(float value, float fromMin, float fromMax, float toMin  = 0, float toMax = 1)
    {
        // Clamp value to the original range
        value = Mathf.Clamp(value, fromMin, fromMax);

        // Calculate the normalized value in the original range
        float normalizedValue = (value - fromMin) / (fromMax - fromMin);

        // Map the normalized value to the new range
        return 1 - Mathf.Lerp(toMin, toMax, normalizedValue);
    }

    #region Callback Methods
    public void OnPointerDown(PointerEventData eventData)
    {
        _timeCheck = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.time - _timeCheck > 0.16f || !_canGetReward) return;
        if (unit.placementSystem.gameManager.currentGameMode != GameMode.Play) return;

        //Update new Endtime and Get Reward
        _endTime = DateTime.Now.AddSeconds(timeWait);
        unit.placementSystem.gameManager.ChangeCoin(coinReward);
        _canGetReward = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_canGetReward) return;
        if (unit.placementSystem.gameManager.currentGameMode != GameMode.Play) return;
        //Set World Space UI
        t_rewardDialougeUI = unit.placementSystem.gameManager.poolManger.GetDialougeUI();
        t_rewardDialougeUI.SetData(unit.worldPositon, coinReward);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_canGetReward) return;
        if (unit.placementSystem.gameManager.currentGameMode != GameMode.Play) return;
        //Remove World Space UI
        unit.placementSystem.gameManager.poolManger.ReleaseDialougeUI(t_rewardDialougeUI);
        t_rewardDialougeUI = null;
    }

    private void OnGameChangeMode(bool value)
    {
        if (value)
        {
            if (t_rewardDialougeUI)
            {
                unit.placementSystem.gameManager.poolManger.ReleaseDialougeUI(t_rewardDialougeUI);
                t_rewardDialougeUI = null;
            }
        }
        else
        {
            if (_canGetReward)
            {
                t_rewardDialougeUI = unit.placementSystem.gameManager.poolManger.GetDialougeUI();
                t_rewardDialougeUI.SetData(unit.worldPositon, coinReward);
                t_rewardDialougeUI.UpdateTime(1, 0);
            }
        }
    }
    #endregion

}
