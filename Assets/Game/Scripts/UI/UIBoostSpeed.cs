using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIBoostSpeed : MonoBehaviour
{
    [SerializeField] private Slider sliderStamina;
    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private TMP_Text maxBoostSpeed;
    [SerializeField] private GameObject clockwise;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody _playerRigidbody;

    private UIMenuInputAction uIMenuInputAction;
    private float currentStamina = 0f;
    private float speedOfPlayer = 0f;
    private bool isRecoveringStamina = false;

    private void Start()
    {
        var playerData = GameManager.Instance.PlayerData;

        currentStamina = playerData.stamina;
        speedOfPlayer = playerData.speed;
        SetupMaxStamina(currentStamina);
        UpdateSpeedText(speedOfPlayer);
        sliderStamina.value = currentStamina;

        sliderStamina.onValueChanged.AddListener(UpdateStaminaText);
        GameManager.Instance.OnStaminaChanged += SetupMaxStamina;
        GameManager.Instance.OnSpeedChanged += UpdateSpeedText;

        uIMenuInputAction = new UIMenuInputAction();
        uIMenuInputAction.Gameplay.Tap.performed += OnTapPerformed;
        uIMenuInputAction.Gameplay.Enable();
    }

    private void OnDestroy()
    {
        sliderStamina.onValueChanged.RemoveListener(UpdateStaminaText);
        GameManager.Instance.OnStaminaChanged -= SetupMaxStamina;
        GameManager.Instance.OnSpeedChanged -= UpdateSpeedText;

        uIMenuInputAction.Gameplay.Tap.performed -= OnTapPerformed;
        uIMenuInputAction.Gameplay.Disable();
    }

    private void Update()
    {
        HandleRecovery();
    }

    private void HandleRecovery()
    {
        if (isRecoveringStamina && currentStamina < GameManager.Instance.PlayerData.stamina)
        {
            currentStamina += 1f * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, GameManager.Instance.PlayerData.stamina);
            sliderStamina.value = currentStamina;

            if (currentStamina >= GameManager.Instance.PlayerData.stamina)
            {
                isRecoveringStamina = false;
                StopPlayer();
            }
        }
    }

    private void UpdateStaminaText(float currentStamina)
    {
        staminaText.text = $"{Mathf.CeilToInt(currentStamina)}/" + Mathf.CeilToInt(sliderStamina.maxValue).ToString("F0");
    }

    private void UpdateSpeedText(float speed)
    {
        speedOfPlayer = speed;
        maxBoostSpeed.text = (speed * 10f).ToString("F1");
    }

    private void SetupMaxStamina(float staminaOfPlayer)
    {
        sliderStamina.maxValue = staminaOfPlayer;
        staminaText.text = $"{Mathf.CeilToInt(currentStamina)}/" + Mathf.CeilToInt(staminaOfPlayer).ToString("F0");
    }

    private void OnTapPerformed(InputAction.CallbackContext context)
    {
        if (isRecoveringStamina)
        {
            playerAnimator.SetBool("isRunning", true);
            MovePlayerSlowly();
        }
        else
        {
            if (currentStamina >= speedOfPlayer)
            {
                currentStamina -= speedOfPlayer;
                sliderStamina.value = currentStamina;

                playerAnimator.SetBool("isRunning", true);
                MovePlayerQuickly();
            }
        }
    }

    private void MovePlayerSlowly()
    {
        Vector3 movement = Vector3.forward * GameManager.Instance.PlayerData.speed * 5f;
        _playerRigidbody.linearVelocity = new Vector3(movement.x, _playerRigidbody.linearVelocity.y, movement.z);
    }

    public void MovePlayerQuickly()
    {
        Vector3 movement = Vector3.forward * GameManager.Instance.PlayerData.speed * 10f;
        _playerRigidbody.linearVelocity = new Vector3(movement.x, _playerRigidbody.linearVelocity.y, movement.z);
    }

    private void StopPlayer()
    {
        _playerRigidbody.linearVelocity = Vector3.zero;
        playerAnimator.SetBool("isRunning", false);
    }
}