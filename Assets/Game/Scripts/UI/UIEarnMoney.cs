using UnityEngine;
using UnityEngine.EventSystems;

public class UIEarnMoney : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UIMenu uIMenu;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody _playerRigidbody;

    private float lastTapTime;
    private float cooldown = 0.2f;
    private float stopDelay = 1.0f;

    private void Update()
    {
        if (Time.time - lastTapTime > stopDelay)
        {
            StopPlayer();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastTapTime < cooldown)
        {
            return;
        }

        lastTapTime = Time.time;

        GameManager.Instance.playerData.money += GameManager.Instance.playerData.income;
        uIMenu.SetupMoneyUI(GameManager.Instance.playerData);

        playerAnimator.SetBool("isRunning", true);
        MovePlayerInSceneMenu();
    }

    public void MovePlayerInSceneMenu()
    {
        Vector3 movement = Vector3.forward * GameManager.Instance.playerData.speed * 10f;
        _playerRigidbody.velocity = new Vector3(movement.x, _playerRigidbody.velocity.y, movement.z);
    }

    private void StopPlayer()
    {
        _playerRigidbody.velocity = Vector3.zero;
        playerAnimator.SetBool("isRunning", false);
    }
}