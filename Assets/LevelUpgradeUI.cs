using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpgradeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text levelUpgradeText;
    [SerializeField] private TMP_Text residentText;
    [SerializeField] private TMP_Text freeText;

    [SerializeField] private Button upgradeBtn;

    private event Action upgradeBtnAction;

    private void Awake()
    {
        upgradeBtn.onClick.AddListener(OnUpgradeBtnClicked);
    }

    public void SetData(int nextLevel, RequireData data, Action action)
    {
        levelUpgradeText.text = $"{nextLevel}";
        residentText.text = $"{data.people}";
        freeText.text = $"{data.free}";

        upgradeBtnAction = action;
    }

    public void SetCanUpgrade(bool value)
    {
        upgradeBtn.interactable = value;
    }

    private void OnUpgradeBtnClicked()
    {
        upgradeBtnAction?.Invoke();
    }
}
