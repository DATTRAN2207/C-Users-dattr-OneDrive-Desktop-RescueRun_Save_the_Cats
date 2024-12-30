using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    CameraIntro,
    Playing,
    Failed,
    LevelComplete
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public GameState currentState;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private CinemachineCamera introCamera;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private Transform rescueTarget;
    
    private float setupDuration = 1f;

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
        currentState = newState;

        switch (newState)
        {
            case GameState.CameraIntro:
                SetupPhase();
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

        Invoke(nameof(StartGameplay), setupDuration);
    }

    private void StartGameplay()
    {
        ChangeState(GameState.Playing);
    }

    private void GameplayPhase()
    {
        introCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        playerController.enabled = true;
    }

    private void FailedPhase()
    {
        playerController.enabled = false;


    }

    private void CompletePhase()
    {
        playerController.enabled = false;

        SceneManager.LoadScene("MenuScene");
    }
}
