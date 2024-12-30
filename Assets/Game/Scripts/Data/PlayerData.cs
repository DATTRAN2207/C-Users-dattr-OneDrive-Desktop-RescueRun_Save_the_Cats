using System;

[Serializable]
public class PlayerData
{
    public int level;
    public float money;

    public float stamina;
    public float speed;
    public float income;

    public int staminaUpgradeCount;
    public int speedUpgradeCount;
    public int incomeUpgradeCount;

    public PlayerData(int currentLevel, float currentMoney, float initialStamina, float initialSpeed, float initialIncome)
    {
        level = currentLevel;
        money = currentMoney;

        stamina = initialStamina;
        speed = initialSpeed;
        income = initialIncome;

        staminaUpgradeCount = 0;
        speedUpgradeCount = 0;
        incomeUpgradeCount = 0;
    }
}