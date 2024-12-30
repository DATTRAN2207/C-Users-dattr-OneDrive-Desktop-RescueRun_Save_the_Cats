using UnityEngine;

public class TsunamiBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.Instance.ChangeState(GameState.Failed);
        }
    }
}