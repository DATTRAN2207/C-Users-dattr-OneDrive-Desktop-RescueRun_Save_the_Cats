using DG.Tweening;
using UnityEngine;

public class TsunamiBehaviour : MonoBehaviour
{
    public void StartMove(Transform endPoint)
    {
        transform.DOMove(endPoint.position, 15f)
                       .SetEase(Ease.Linear).SetSpeedBased(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.Instance.ChangeState(GameState.Failed);
        }
    }
}