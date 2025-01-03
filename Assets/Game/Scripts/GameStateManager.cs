using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Watermelon;

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
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private UIRunScene uIRunScene;
    [SerializeField] private UILoading uILoading;
    [SerializeField] private PopupPlayResultFailed popupPlayResultFailed;
    [SerializeField] private PopupPlayResultFinished popupPlayResultFinished;

    private float gameplayStartTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        popupPlayResultFailed.OnBackToMenuButtonClicked += GotoMenuScene;
        popupPlayResultFinished.OnBackToMenuButtonClicked += GotoMenuScene;
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
        gameplayStartTime = Time.time;

        playerController.enabled = true;
        playerController.UpdateSpeed(boostSpeed);
        popupPlayResultFailed.UpdateTopSpeed(boostSpeed);
        tsunami.StartMove(endPoint);
    }

    private void FailedPhase()
    {
        playerController.UpdateSpeed(0f);
        tsunami.StopMove();

        popupPlayResultFailed.SetLevelData(LevelLoader.Instance.GetLevelData());

        float completedTime = Time.time - gameplayStartTime;
        popupPlayResultFailed.UpdateCompletedTime(completedTime);

        int catsRescued = playerController.GetComponent<PlayerBehaviour>().GetTotalRescuedCats();
        popupPlayResultFailed.UpdateRescuedCats(catsRescued, GameManager.Instance.PlayerData.level);

        float distance = Vector3.Distance(startPoint.position, playerController.transform.position);
        popupPlayResultFailed.UpdateDistance(distance, GameManager.Instance.PlayerData.level);

        var money = CalculateReward(catsRescued, distance, GameManager.Instance.PlayerData.level);
        popupPlayResultFailed.UpdateMoneyReward(money);

        popupPlayResultFailed.Show();
    }

    private void CompletePhase()
    {
        playerController.UpdateSpeed(0f);
        tsunami.StopMove();

        var levelData = LevelLoader.Instance.GetLevelData();
        popupPlayResultFinished.SetLevelData(levelData);

        float completedTime = Time.time - gameplayStartTime;
        popupPlayResultFinished.UpdateCompletedTime(completedTime);

        int catsRescued = playerController.GetComponent<PlayerBehaviour>().GetTotalRescuedCats();
        popupPlayResultFinished.UpdateRescuedCats(catsRescued, GameManager.Instance.PlayerData.level);

        popupPlayResultFinished.UpdateDistance();

        var money = CalculateReward(catsRescued, levelData.levels[GameManager.Instance.PlayerData.level].roadLength, GameManager.Instance.PlayerData.level);
        popupPlayResultFinished.UpdateMoneyReward(money);

        popupPlayResultFinished.Show();

        GameManager.Instance.UpdatePlayerLevel();
    }

    private void GotoMenuScene()
    {
        popupPlayResultFailed.Hide();
        popupPlayResultFinished.Hide();
        uILoading.gameObject.SetActive(true);
        uILoading.ShowUILoading(() =>
        {
            SceneManager.LoadScene("MenuScene");
        });
    }

    private float CalculateReward(int catsRescued, float distanceRan, int levelIndex)
    {
        float baseRewardPerCat = GameManager.Instance.PlayerData.income * levelIndex;
        float baseRewardPerMeter = GameManager.Instance.PlayerData.income;

        float catBonus = catsRescued * baseRewardPerCat;
        float distanceBonus = distanceRan * baseRewardPerMeter;

        GameManager.Instance.UpdatePlayerMoney(catBonus + distanceBonus);

        return catBonus + distanceBonus;
    }

}
