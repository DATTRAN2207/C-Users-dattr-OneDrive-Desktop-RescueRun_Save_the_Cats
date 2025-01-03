using UnityEngine;

public class StaminaUpgradeButton : BaseButton
{
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.OnMoneyChanged += CheckStatusButton;
        GameManager.Instance.OnStaminaChanged += UpdateText;

        float currentMoney = GameManager.Instance.PlayerData.money;
        CheckStatusButton(currentMoney);

        UpdateText(GameManager.Instance.PlayerData.stamina);
    }

    protected override void OnDestroy()
    {
        GameManager.Instance.OnMoneyChanged -= CheckStatusButton;
        GameManager.Instance.OnStaminaChanged -= UpdateText;
    }

    protected override void UpdateText(float playerStasValue)
    {
        currentValue.text = $"{Mathf.CeilToInt(playerStasValue)}";
        upgradeCost.text = UpgradeManager.Instance.GetStaminaUpgradeCost(GameManager.Instance.PlayerData.staminaUpgradeCount).ToString();
    }

    private void CheckStatusButton(float money)
    {
        float staminaUpgradeCost = UpgradeManager.Instance.GetStaminaUpgradeCost(GameManager.Instance.PlayerData.staminaUpgradeCount);

        if (money < staminaUpgradeCost)
        {
            SetStatusOfButton(false);
        }
        else
        {
            SetStatusOfButton(true);
        }
    }
}