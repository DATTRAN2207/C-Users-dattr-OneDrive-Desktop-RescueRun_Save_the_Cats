using UnityEngine;

public class IncomeUpgradeButton : BaseButton
{
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.OnMoneyChanged += CheckStatusButton;
        GameManager.Instance.OnIncomeChanged += UpdateText;

        float currentMoney = GameManager.Instance.PlayerData.money;
        CheckStatusButton(currentMoney);

        UpdateText(GameManager.Instance.PlayerData.income);
    }

    protected override void UpdateText(float playerStasValue)
    {
        currentValue.text = $"{Mathf.CeilToInt(playerStasValue)}";
        upgradeCost.text = UpgradeManager.Instance.GetIncomeUpgradeCost(GameManager.Instance.PlayerData.incomeUpgradeCount).ToString();
    }

    private void CheckStatusButton(float money)
    {
        float incomeUpgradeCost = UpgradeManager.Instance.GetIncomeUpgradeCost(GameManager.Instance.PlayerData.incomeUpgradeCount);

        if (money < incomeUpgradeCost)
        {
            SetStatusOfButton(false);
        }
        else
        {
            SetStatusOfButton(true);
        }
    }
}