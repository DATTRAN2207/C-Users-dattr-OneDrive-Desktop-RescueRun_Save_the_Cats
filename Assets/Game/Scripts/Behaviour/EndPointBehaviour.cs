using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class EndPointBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject arrowIcon;

    private void Start()
    {
        AnimateArrow();
    }

    private void AnimateArrow()
    {
        if (arrowIcon != null)
        {
            Vector3 startPosition = arrowIcon.transform.localPosition;

            arrowIcon.transform.DOLocalMoveY(startPosition.y + 0.5f, 1f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName == "MenuScene")
            {
                InfiniteRoad.Instance.SwithRoad();
            }
            else if (currentSceneName == "RunScene")
            {
                GameStateManager.Instance.ChangeState(GameState.LevelComplete);
            }
        }
    }
}