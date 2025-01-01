using UnityEngine;

public class SpeedUpgradeButton : BaseButton
{
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.OnMoneyChanged += CheckStatusButton;
        GameManager.Instance.OnSpeedChanged += UpdateText;

        float currentMoney = GameManager.Instance.PlayerData.money;
        CheckStatusButton(currentMoney);

        UpdateText(GameManager.Instance.PlayerData.speed);
    }

    protected override void UpdateText(float playerStasValue)
    {
        currentValue.text = $"{playerStasValue:F1}";
        upgradeCost.text = UpgradeManager.Instance.GetSpeedUpgradeCost(GameManager.Instance.PlayerData.speedUpgradeCount).ToString();
    }

    private void CheckStatusButton(float money)
    {
        float speedUpgradeCost = UpgradeManager.Instance.GetSpeedUpgradeCost(GameManager.Instance.PlayerData.speedUpgradeCount);

        if (money < speedUpgradeCost)
        {
            SetStatusOfButton(false);
        }
        else
        {
            SetStatusOfButton(true);
        }
    }
}
