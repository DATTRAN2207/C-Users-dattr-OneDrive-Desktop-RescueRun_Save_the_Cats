using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBoostSpeedInMenuScene : UIBoostSpeed
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private MoneyProgress moneyProgress;

    protected override void Start()
    {
        base.Start();
        UpdateMaxSpeedText(playerData.speed);

        GameManager.Instance.OnStaminaChanged += UpdateMaxStaminaText;
        GameManager.Instance.OnSpeedChanged += UpdateMaxSpeedText;
        sliderStamina.onValueChanged.AddListener(UpdateStaminaText);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.Instance.OnStaminaChanged -= UpdateMaxStaminaText;
        GameManager.Instance.OnSpeedChanged -= UpdateMaxSpeedText;
        sliderStamina.onValueChanged.RemoveListener(UpdateStaminaText);
    }

    protected override void RecoverStamina()
    {
        base.RecoverStamina();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        float currentTime = Time.time;
        isQuickTouch = currentTime - lastTouchTime <= quickTouchThreshold;
        lastTouchTime = currentTime;

        speedTween?.Kill();

        if (isRecoveringStamina)
        {
            currentSpeed = playerData.speed;
            quickTouchCount = 0;
            MovePlayer(currentSpeed);
            return;
        }

        if (isQuickTouch)
        {
            if (!isAtMaxSpeed)
            {
                quickTouchCount++;
                currentSpeed = Mathf.Min(playerData.speed * quickTouchCount, playerData.speed * 10);

                if (quickTouchCount >= 10)
                {
                    isAtMaxSpeed = true;
                }
            }
        }
        else
        {
            currentSpeed = playerData.speed;
            quickTouchCount = 0;
            isAtMaxSpeed = false;
        }

        if (currentStamina >= playerData.speed)
        {
            currentStamina -= playerData.speed;
            sliderStamina.value = currentStamina;
            MovePlayer(currentSpeed);
        }
        else
        {
            isRecoveringStamina = true;
            RecoverStamina();
        }

        moneyProgress.AddMoney(1f);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        speedTween?.Kill();

        float decelerationTime = Mathf.Lerp(0.5f, 3f, currentSpeed / (playerData.speed * 10));

        speedTween = DOTween.To(() => currentSpeed, x => currentSpeed = x, 0, decelerationTime)
            .SetEase(Ease.Linear)
            .OnUpdate(() => { MovePlayer(currentSpeed); })
            .OnComplete(() =>
            {
                StopPlayer();
                RecoverStamina();
            });
    }

    public void MovePlayer(float speed)
    {
        if (playerAnimator == null || _playerRigidbody == null) return;

        if (speed <= playerData.speed * 5f)
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

    public override void UpdateNeedleRotation(float speed)
    {
        float normalizedSpeed = speed / (playerData.speed * 10);
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