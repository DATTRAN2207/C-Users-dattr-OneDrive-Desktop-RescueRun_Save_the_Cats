using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBoostSpeed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] protected Slider sliderStamina;
    [SerializeField] protected TMP_Text staminaText;
    [SerializeField] protected TMP_Text maxBoostSpeed;
    [SerializeField] protected GameObject clockwise;

    protected PlayerData playerData;
    protected float currentSpeed;
    protected float currentStamina = 0f;
    protected bool isRecoveringStamina = false;
    protected Tween speedTween;
    protected Tween staminaTween;
    protected float lastTouchTime = 0f;
    protected float quickTouchThreshold = 0.3f;
    protected bool isQuickTouch = false;
    protected int quickTouchCount = 0;
    protected bool isAtMaxSpeed = false;

    protected virtual void Start()
    {
        playerData = GameManager.Instance.PlayerData;

        currentStamina = playerData.stamina;

        UpdateMaxStaminaText(currentStamina);
        sliderStamina.value = currentStamina;
    }

    protected virtual void OnDestroy()
    {
    }

    protected void UpdateStaminaText(float stamina)
    {
        staminaText.text = $"{Mathf.CeilToInt(stamina)}/" + Mathf.CeilToInt(sliderStamina.maxValue).ToString("F0");
    }

    protected virtual void UpdateMaxStaminaText(float stamina)
    {
        sliderStamina.maxValue = stamina;
        staminaText.text = $"{Mathf.CeilToInt(currentStamina)}/" + Mathf.CeilToInt(stamina).ToString("F0");
    }

    protected virtual void UpdateMaxSpeedText(float speed)
    {
        maxBoostSpeed.text = (speed * 10f).ToString("F1");
    }

    protected virtual void RecoverStamina()
    {
        if (staminaTween != null && staminaTween.IsPlaying())
            return;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(2f);
        sequence.AppendCallback(() => { isRecoveringStamina = true; });

        float recoveryDuration = (playerData.stamina - currentStamina) * 0.1f;

        sequence.Append(DOTween.To(() => currentStamina, x =>
        {
            currentStamina = Mathf.Min(x, playerData.stamina);
            sliderStamina.value = Mathf.FloorToInt(currentStamina);
            UpdateStaminaText(currentStamina);
        }, playerData.stamina, recoveryDuration).SetEase(Ease.Linear));

        sequence.OnComplete(() =>
        {
            currentStamina = Mathf.FloorToInt(playerData.stamina);
            isRecoveringStamina = false;
            quickTouchCount = 0;
            isAtMaxSpeed = false;
            isQuickTouch = false;
        });

        staminaTween = sequence;
    }

    public virtual void UpdateNeedleRotation(float speed)
    {

    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        float currentTime = Time.time;
        isQuickTouch = currentTime - lastTouchTime <= quickTouchThreshold;
        lastTouchTime = currentTime;

        speedTween?.Kill();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
    }
}