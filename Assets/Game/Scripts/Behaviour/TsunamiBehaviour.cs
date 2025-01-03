using DG.Tweening;
using UnityEngine;

public class TsunamiBehaviour : MonoBehaviour
{
    private Tween moveTween;

    public void StartMove(Transform endPoint)
    {
        moveTween = transform.DOMove(endPoint.position, 15f).SetEase(Ease.Linear).SetSpeedBased(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.Instance.ChangeState(GameState.Failed);
        }
    }

    public void StopMove()
    {
        moveTween?.Kill();
    }
}