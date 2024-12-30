using UnityEngine;

public class SpeedUpgradeButton : BaseButton
{
    public override void SetupUI(PlayerData playerData)
    {
        currentValue.text = $"{playerData.speed:F1}";
        upgradeCost.text = UpgradeManager.Instance.GetSpeedUpgradeCost(playerData.speedUpgradeCount).ToString();
    }
}
