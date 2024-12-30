using UnityEngine;

public class EndPointBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.Instance.ChangeState(GameState.LevelComplete);
        }
    }
}