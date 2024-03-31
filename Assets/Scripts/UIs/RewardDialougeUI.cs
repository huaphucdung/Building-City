using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RewardDialougeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text numberCoinReward;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private float yOffset = 8f;

    public void SetData(Vector3 position, int coin)
    {
        position.y = yOffset;
        transform.position = position;
        numberCoinReward.text = $"{coin}";
    }

    public void UpdateTime(float percent, float time)
    {
       
        timeSlider.value = Mathf.Clamp(percent, 0, 1);

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        if (minutes < 0) minutes = 0;
        if (seconds < 0) seconds = 0;

        timeText.text = $"{minutes.ToString("00")}:{seconds.ToString("00")}";
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
