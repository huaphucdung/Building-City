using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class BuildingItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;

    public event Action<string> clickedAction;

    private string _key;

    public void SetData(string key, Sprite img, string name, int price, Action<object> callbackAction)
    {
        _key = key;
        image.sprite = img;
        nameText.text = name;
        priceText.text = $"{price}";

        clickedAction = callbackAction;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickedAction?.Invoke(_key);
    }
}
