using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private float baseStaminaUpgradeCost = 17f;
    private float baseSpeedUpgradeCost = 9f;
    private float baseIncomeUpgradeCost = 12f;

    public static UpgradeManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float GetStaminaUpgradeCost(int upgradeCount)
    {
        return Mathf.Ceil(baseStaminaUpgradeCost * Mathf.Pow(1.1f, upgradeCount));
    }

    public float UpgradeStamina(PlayerData playerData)
    {
        float upgradeCost = GetStaminaUpgradeCost(playerData.staminaUpgradeCount);

        if (playerData.money >= upgradeCost)
        {
            playerData.stamina += playerData.stamina * 5 / 100;
            playerData.staminaUpgradeCount++;
            return upgradeCost;
        }

        Debug.LogWarning("Not enough money to upgrade stamina!");
        return 0f;
    }

    public float GetSpeedUpgradeCost(int upgradeCount)
    {
        return baseSpeedUpgradeCost * Mathf.Pow(1.1f, upgradeCount);
    }

    public bool UpgradeSpeed(PlayerData playerData)
    {
        float upgradeCost = GetSpeedUpgradeCost(playerData.speedUpgradeCount);

        if (playerData.money >= upgradeCost)
        {
            playerData.money -= upgradeCost;
            playerData.speed += 0.1f;
            playerData.speedUpgradeCount++;
            return true;
        }

        Debug.LogWarning("Not enough money to upgrade speed!");
        return false;
    }

    public float GetIncomeUpgradeCost(int upgradeCount)
    {
        return baseIncomeUpgradeCost * Mathf.Pow(1.1f, upgradeCount);
    }

    public bool UpgradeIncome(PlayerData playerData)
    {
        float upgradeCost = GetIncomeUpgradeCost(playerData.incomeUpgradeCount);

        if (playerData.money >= upgradeCost)
        {
            playerData.money -= upgradeCost;
            playerData.income += 1;
            playerData.incomeUpgradeCount++;
            return true;
        }

        return false;
    }
}
