using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBoostSpeedInRunScene : UIBoostSpeed
{
    private float staminaPerTab = 0f;
    public Action<float> onSpeedChanged;

    protected override void Start()
    {
        base.Start();
        currentSpeed = playerData.speed + 10f;
        staminaPerTab = playerData.stamina / 10f;
        UpdateMaxSpeedText(playerData.speed);
    }

    protected override void UpdateMaxSpeedText(float speed)
    {
        maxBoostSpeed.text = currentSpeed.ToString("F1");
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (currentStamina >= staminaPerTab)
        {
            currentStamina -= staminaPerTab;
            sliderStamina.value = currentStamina;
            currentSpeed += 1f;
            UpdateMaxSpeedText(currentSpeed);
        }
    }

    public override void UpdateNeedleRotation(float speed)
    {
        float normalizedSpeed = speed / currentSpeed;
        float targetAngle = Mathf.Lerp(-90f, 90f, normalizedSpeed);

        clockwise.transform.DORotateQuaternion(Quaternion.Euler(0, 0, targetAngle), 0.2f);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void DecaySpeed()
    {
        DOVirtual.DelayedCall(5f, () =>
        {
            currentSpeed = playerData.speed + 10f;
            UpdateMaxSpeedText(currentSpeed);
            onSpeedChanged?.Invoke(currentSpeed);
        });
    }
}