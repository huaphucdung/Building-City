using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private BuildingItem itemPrefab;
    [SerializeField] private Transform itemsTransform;

    [Header("TMPs:")]
    [SerializeField] private TMP_Text numberlevel;
    [SerializeField] private TMP_Text cityName;
    [SerializeField] private TMP_Text numberPeople;
    [SerializeField] private TMP_Text numberCoin;

    [Header("Buttons:")]
    [SerializeField] private Button levelBtn;
    [SerializeField] private Button QuitBtn;
    [SerializeField] private Button ShowBtn;
    [SerializeField] private Button HideBtn;

    [Header("Toggles:")]
    [SerializeField] private Toggle shopToggle;
    [SerializeField] private Toggle houseToggle;
    [SerializeField] private Toggle decorateToggle;

    [Header("DoTween Settings:")]
    [SerializeField] private RectTransform bottom;
    [SerializeField] private float duration;

    public event Action<bool> OnShowBuildingListAction;

    private Dictionary<string, BuildingItem> _itemDictionary;

    private Dictionary<BuildingType, List<GameObject>> _itemBuildingTypeDictionary;

    private GameManager gameManager;

    public void Initialize(GameManager manager)
    {
        _itemDictionary = new Dictionary<string, BuildingItem>();
        _itemBuildingTypeDictionary = new Dictionary<BuildingType, List<GameObject>>();
        gameManager = manager;
        AddAction();
    }

    public void SetData(int level, string name, int people, int coin)
    {
        numberlevel.text = $"{level}";
        cityName.text = name;
        numberPeople.text = $"{people}";
        numberCoin.text = $"{coin}";
    }
    
    private void AddAction()
    {
        ShowBtn.onClick.AddListener(OnShowBtnClicekd);
        HideBtn.onClick.AddListener(OnHideBtnCliked);
        levelBtn.onClick.AddListener(OnLevelBtnClicked);

        shopToggle.onValueChanged.AddListener(OnToggleChange);
        houseToggle.onValueChanged.AddListener(OnToggleChange);
        decorateToggle.onValueChanged.AddListener(OnToggleChange);
    }

 
    public void AddBuildingItem(string key, BuildingData data, Action<object> callbackAction)
    {
        BuildingItem newItem = Instantiate(itemPrefab, itemsTransform);
        GameObject newObjefct = newItem.gameObject;
        newItem.SetData(key, null, data.Name, data.price, callbackAction);

        _itemDictionary.Add(key, newItem);

        if(!_itemBuildingTypeDictionary.ContainsKey(data.type))
        {
            _itemBuildingTypeDictionary[data.type] = new List<GameObject>();
        }
        _itemBuildingTypeDictionary[data.type].Add(newObjefct);
    }

    
    private void OnShowBtnClicekd()
    {
        ShowList();
        OnShowBuildingListAction?.Invoke(true);
    }

    private void OnHideBtnCliked()
    {
        HideList();
        OnShowBuildingListAction?.Invoke(false);
    }

    private void OnLevelBtnClicked()
    {
        gameManager.ShowUpgradeUI();
    }

    private void OnToggleChange(bool value)
    {
        foreach(var obj in _itemBuildingTypeDictionary[BuildingType.Shop])
        {
            obj.SetActive(shopToggle.isOn);
        }

        foreach (var obj in _itemBuildingTypeDictionary[BuildingType.House])
        {
            obj.SetActive(houseToggle.isOn);
        }
        foreach (var obj in _itemBuildingTypeDictionary[BuildingType.Decorate])
        {
            obj.SetActive(decorateToggle.isOn);
        }
    }

    public void ShowList(bool showBtn = true)
    {
        ShowBtn.interactable = false;
        bottom.DOMoveY(0f, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            ShowBtn.gameObject.SetActive(false);

            if(showBtn)
            {
                HideBtn.interactable = true;
                HideBtn.gameObject.SetActive(true);
            }
        });
    }

    public void HideList(bool showBtn = true)
    {
        
        HideBtn.interactable = false;
        bottom.DOMoveY(-360f, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            HideBtn.gameObject.SetActive(false);
            if (showBtn)
            {
                ShowBtn.interactable = true;
                ShowBtn.gameObject.SetActive(true);
            }
        }); ;
    }
}
