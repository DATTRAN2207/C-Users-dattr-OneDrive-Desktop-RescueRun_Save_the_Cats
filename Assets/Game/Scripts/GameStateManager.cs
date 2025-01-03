using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    CameraIntro,
    Preparing,
    Playing,
    Failed,
    LevelComplete
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private CinemachineCamera introCamera;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private TsunamiBehaviour tsunami;
    [SerializeField] private Transform endPoint;
    [SerializeField] private UIRunScene uIRunScene;
    [SerializeField] private UILoading uILoading;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ChangeState(GameState.CameraIntro);
    }

    public void ChangeState(GameState newState)
    {
        switch (newState)
        {
            case GameState.CameraIntro:
                SetupPhase();
                break;
            case GameState.Preparing:
                PreparingPhase();
                break;
            case GameState.Playing:
                GameplayPhase();
                break;
            case GameState.Failed:
                FailedPhase();
                break;
            case GameState.LevelComplete:
                CompletePhase();
                break;
        }
    }

    private void SetupPhase()
    {
        introCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        playerController.enabled = false;

        Invoke(nameof(StartPreparing), 5f);
    }

    private void StartPreparing()
    {
        ChangeState(GameState.Preparing);
    }

    private void PreparingPhase()
    {
        introCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        StartCoroutine(WaitForCinemachineBlend(() =>
        {
            uIRunScene.ShowUIBoostSpeed();
            uIRunScene.OnUnShowUIBoostSpeed += GameplayPhase;
        }));
    }

    private IEnumerator WaitForCinemachineBlend(Action onComplete)
    {
        yield return new WaitForSeconds(3f);

        onComplete?.Invoke();
    }

    private void GameplayPhase(float boostSpeed = 0f)
    {
        playerController.enabled = true;
        playerController.UpdateSpeed(boostSpeed);
        tsunami.StartMove(endPoint);
    }

    private void FailedPhase()
    {
        playerController.enabled = false;

        uILoading.gameObject.SetActive(true);
        uILoading.ShowUILoading(() =>
        {
            SceneManager.LoadScene("MenuScene");
        });
    }

    private void CompletePhase()
    {
        playerController.enabled = false;
        GameManager.Instance.PlayerData.level += 1;

        uILoading.gameObject.SetActive(true);
        uILoading.ShowUILoading(() =>
        {
            SceneManager.LoadScene("MenuScene");
        });
    }
}
