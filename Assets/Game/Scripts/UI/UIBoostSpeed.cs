using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

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
    private bool isRecoveringStamina = false;
    private float currentSpeed;
    private Tween speedTween;
    private Tween staminaTween;
    private float lastTouchTime = 0f;
    private float quickTouchThreshold = 3f;
    private bool isQuickTouch = false;
    private int quickTouchCount = 0;
    private bool isAtMaxSpeed = false;

    private void Start()
    {
        var playerData = GameManager.Instance.PlayerData;

        currentStamina = playerData.stamina;
        UpdateMaxStaminaText(currentStamina);
        UpdateMaxSpeedText(playerData.speed);
        sliderStamina.value = currentStamina;

        sliderStamina.onValueChanged.AddListener(UpdateStaminaText);
        GameManager.Instance.OnStaminaChanged += UpdateMaxStaminaText;
        GameManager.Instance.OnSpeedChanged += UpdateMaxSpeedText;

        uIMenuInputAction = new UIMenuInputAction();
        uIMenuInputAction.Gameplay.Tap.performed += OnTapPerformed;
        uIMenuInputAction.Gameplay.Tap.canceled += OnTapCanceled;
        uIMenuInputAction.Gameplay.Enable();
    }

    private void OnDestroy()
    {
        sliderStamina.onValueChanged.RemoveListener(UpdateStaminaText);
        GameManager.Instance.OnStaminaChanged -= UpdateMaxStaminaText;
        GameManager.Instance.OnSpeedChanged -= UpdateMaxSpeedText;

        uIMenuInputAction.Gameplay.Tap.performed -= OnTapPerformed;
        uIMenuInputAction.Gameplay.Tap.canceled -= OnTapCanceled;
        uIMenuInputAction.Gameplay.Disable();
    }

    private void RecoverStamina()
    {
        if (staminaTween != null && staminaTween.IsPlaying())
            return;

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(2f);

        sequence.AppendCallback(() =>
        {
            isRecoveringStamina = true;
        });

        float recoveryDuration = (GameManager.Instance.PlayerData.stamina - currentStamina) * 0.1f;

        sequence.Append(DOTween.To(
            () => currentStamina,
            x =>
            {
                currentStamina = Mathf.Min(x, GameManager.Instance.PlayerData.stamina);
                sliderStamina.value = Mathf.FloorToInt(currentStamina);
                UpdateStaminaText(currentStamina);
            },
            GameManager.Instance.PlayerData.stamina,
            recoveryDuration
        ).SetEase(Ease.Linear));

        sequence.OnComplete(() =>
        {
            currentStamina = Mathf.FloorToInt(GameManager.Instance.PlayerData.stamina);
            isRecoveringStamina = false;

            quickTouchCount = 0;
            isAtMaxSpeed = false;
            isQuickTouch = false;
        });


        staminaTween = sequence;
    }

    private void UpdateStaminaText(float currentStamina)
    {
        staminaText.text = $"{Mathf.CeilToInt(currentStamina)}/" + Mathf.CeilToInt(sliderStamina.maxValue).ToString("F0");
    }

    private void UpdateMaxSpeedText(float speed)
    {
        maxBoostSpeed.text = (speed * 10f).ToString("F1");
    }

    private void UpdateMaxStaminaText(float staminaOfPlayer)
    {
        sliderStamina.maxValue = staminaOfPlayer;
        staminaText.text = $"{Mathf.CeilToInt(currentStamina)}/" + Mathf.CeilToInt(staminaOfPlayer).ToString("F0");
    }

    private void OnTapPerformed(CallbackContext context)
    {
        speedTween?.Kill();

        float currentTime = Time.time;
        isQuickTouch = (currentTime - lastTouchTime <= quickTouchThreshold);
        lastTouchTime = currentTime;

        if (isRecoveringStamina)
        {
            currentSpeed = GameManager.Instance.PlayerData.speed;
            quickTouchCount = 0;
            MovePlayer(currentSpeed);
            return;
        }

        if (isQuickTouch)
        {
            if (!isAtMaxSpeed)
            {
                quickTouchCount++;
                currentSpeed = Mathf.Min(GameManager.Instance.PlayerData.speed * quickTouchCount,
                                         GameManager.Instance.PlayerData.speed * 10);

                if (quickTouchCount >= 10)
                {
                    isAtMaxSpeed = true;
                }
            }
        }
        else
        {
            currentSpeed = GameManager.Instance.PlayerData.speed;
            quickTouchCount = 0;
            isAtMaxSpeed = false;
        }

        if (currentStamina >= GameManager.Instance.PlayerData.speed)
        {
            currentStamina -= GameManager.Instance.PlayerData.speed;
            sliderStamina.value = currentStamina;
            MovePlayer(currentSpeed);
        }
        else
        {
            isRecoveringStamina = true;
            RecoverStamina();
        }
    }

    private void OnTapCanceled(CallbackContext context)
    {
        speedTween?.Kill();

        float decelerationTime = Mathf.Lerp(0.5f, 3f, currentSpeed / (GameManager.Instance.PlayerData.speed * 10));

        speedTween = DOTween.To(() => currentSpeed, x => currentSpeed = x, 0, decelerationTime)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                MovePlayer(currentSpeed);
            })
            .OnComplete(() =>
            {
                StopPlayer();
                RecoverStamina();
            });
    }

    public void MovePlayer(float speed)
    {
        if (playerAnimator == null || _playerRigidbody == null) return;
        if (speed <= GameManager.Instance.PlayerData.speed * 5f)
        {
            playerAnimator.SetBool("isWalking", true);
            playerAnimator.SetBool("isRunning", false);
        }
        else
        {
            playerAnimator.SetBool("isWalking", false);
            playerAnimator.SetBool("isRunning", true);
        }

        Vector3 movement = Vector3.forward * speed;
        _playerRigidbody.velocity = new Vector3(movement.x, _playerRigidbody.velocity.y, movement.z);

        UpdateNeedleRotation(speed);
    }

    private void UpdateNeedleRotation(float speed)
    {
        float normalizedSpeed = speed / (GameManager.Instance.PlayerData.speed * 10);
        float targetAngle = Mathf.Lerp(-90f, 90f, normalizedSpeed);

        clockwise.transform.DORotateQuaternion(Quaternion.Euler(0, 0, targetAngle), 0.2f);
    }

    private void StopPlayer()
    {
        _playerRigidbody.velocity = Vector3.zero;
        playerAnimator.SetBool("isWalking", false);
        playerAnimator.SetBool("isRunning", false);
    }
}