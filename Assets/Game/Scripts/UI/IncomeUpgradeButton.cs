using UnityEngine;

public class IncomeUpgradeButton : BaseButton
{
    public override void SetupUI(PlayerData playerData)
    {
        currentValue.text = $"{Mathf.CeilToInt(playerData.income)}";
        upgradeCost.text = UpgradeManager.Instance.GetIncomeUpgradeCost(playerData.incomeUpgradeCount).ToString();
    }
}