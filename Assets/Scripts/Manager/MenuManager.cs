using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera VCamera;
    [SerializeField] private MenuUI ui;
    [SerializeField] private float durationRotation = 5f;
    private void Start()
    {
        ui.Initialize(this);
        ui.AddAction();
        //Check has save data or not
        ui.SetCanContinue(false);

        Vector3 rotationTarget = VCamera.transform.rotation.eulerAngles;
        rotationTarget.y = 0;
        VCamera.transform.DORotate(rotationTarget, durationRotation).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
        ui.RemoveAction();
    }

    public void ContinueGame()
    {

    }

    public void StartNewGame(string cityName)
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
