using UnityEngine;

public class StaminaUpgradeButton : BaseButton
{
    public override void SetupUI(PlayerData playerData)
    {
        currentValue.text = $"{Mathf.CeilToInt(playerData.stamina)}";
        upgradeCost.text = UpgradeManager.Instance.GetStaminaUpgradeCost(playerData.staminaUpgradeCount).ToString();
    }
}