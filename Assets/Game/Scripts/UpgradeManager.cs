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
            GameManager.Instance.UpdatePlayerStamina(playerData.stamina * 5 / 100);

            return upgradeCost;
        }

        return 0f;
    }

    public float GetSpeedUpgradeCost(int upgradeCount)
    {
        return Mathf.Ceil(baseSpeedUpgradeCost * Mathf.Pow(1.1f, upgradeCount));
    }

    public float UpgradeSpeed(PlayerData playerData)
    {
        float upgradeCost = GetSpeedUpgradeCost(playerData.speedUpgradeCount);

        if (playerData.money >= upgradeCost)
        {
            GameManager.Instance.UpdatePlayerSpeed(0.1f);

            return upgradeCost;
        }

        return 0f;
    }

    public float GetIncomeUpgradeCost(int upgradeCount)
    {
        return Mathf.Ceil(baseIncomeUpgradeCost * Mathf.Pow(1.1f, upgradeCount));
    }

    public float UpgradeIncome(PlayerData playerData)
    {
        float upgradeCost = GetIncomeUpgradeCost(playerData.incomeUpgradeCount);

        if (playerData.money >= upgradeCost)
        {
            GameManager.Instance.UpdatePlayerIncome(1f);

            return upgradeCost;
        }

        return 0f;
    }
}
