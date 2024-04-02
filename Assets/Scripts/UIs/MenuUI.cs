using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button quitBtn;

    [SerializeField] private GameObject gameName;
    [SerializeField] private GameObject menuGroup;

    [Header("Name Dialouge:")]
    [SerializeField] private GameObject cityNameDialouge;
    [SerializeField] private TMP_InputField cityNameField;
    [SerializeField] private Button createBtn;
    [SerializeField] private Button exitBtn;


    private MenuManager menuManger;
    public void Initialize(MenuManager manager)
    {
        menuManger = manager;
    }

    public void SetCanContinue(bool value)
    {
        continueBtn.interactable = value;
    }

    public void AddAction()
    {
        continueBtn.onClick.AddListener(OnContinueBtnClicked);
        newGameBtn.onClick.AddListener(OnNewGameBtnClicked);
        quitBtn.onClick.AddListener(OnQuitBtnClicked);
        createBtn.onClick.AddListener(OnCreateBtnClicked);
        exitBtn.onClick.AddListener(OnExitBtnCliked);
    }

    public void RemoveAction()
    {
        continueBtn.onClick.RemoveAllListeners();
        newGameBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
        createBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();
    }

    private void OnContinueBtnClicked()
    {
        menuManger.ContinueGame();
    }

    private void OnNewGameBtnClicked()
    {
        gameName.SetActive(false);
        menuGroup.SetActive(false);
        //Show Name Dilouage
        cityNameField.text = "";
        cityNameDialouge.SetActive(true);
    }

    private void OnCreateBtnClicked()
    {
        menuManger.StartNewGame(cityNameField.text);
    }

    private void OnExitBtnCliked()
    {
        cityNameDialouge.SetActive(false);
        gameName.SetActive(true);
        menuGroup.SetActive(true);  
    }

    private void OnQuitBtnClicked()
    {
        menuManger.Quit();
    }
}
