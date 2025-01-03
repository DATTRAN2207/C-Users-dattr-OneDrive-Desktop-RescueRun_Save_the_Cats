using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private PlayerData playerData;

    public PlayerData PlayerData
    {
        get => playerData;
        private set => playerData = value;
    }

    private const string PlayerInitializedKey = "PlayerInitialized";

    public Action<float> OnStaminaChanged;
    public Action<float> OnSpeedChanged;
    public Action<float> OnIncomeChanged;
    public Action<float> OnMoneyChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePlayerData()
    {
        if (PlayerPrefs.HasKey(PlayerInitializedKey) && PlayerPrefs.GetInt(PlayerInitializedKey) == 1)
        {
            LoadPlayerData();
        }
        else
        {
            playerData = new PlayerData(currentLevel: 0, currentMoney: 50f, initialStamina: 60f, initialSpeed: 1f, initialIncome: 3f);
            SavePlayerData();
            PlayerPrefs.SetInt(PlayerInitializedKey, 1);
        }
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("Level", playerData.level);
        PlayerPrefs.SetFloat("Money", playerData.money);
        PlayerPrefs.SetFloat("Stamina", playerData.stamina);
        PlayerPrefs.SetFloat("Speed", playerData.speed);
        PlayerPrefs.SetFloat("Income", playerData.income);

        PlayerPrefs.SetInt("StaminaUpgradeCount", playerData.staminaUpgradeCount);
        PlayerPrefs.SetInt("SpeedUpgradeCount", playerData.speedUpgradeCount);
        PlayerPrefs.SetInt("IncomeUpgradeCount", playerData.incomeUpgradeCount);

        PlayerPrefs.Save();
    }

    public void LoadPlayerData()
    {
        int level = PlayerPrefs.GetInt("Level", 0);
        float money = PlayerPrefs.GetFloat("Money", 100f);
        float stamina = PlayerPrefs.GetFloat("Stamina", 100f);
        float speed = PlayerPrefs.GetFloat("Speed", 5f);
        float income = PlayerPrefs.GetFloat("Income", 10f);

        int staminaUpgradeCount = PlayerPrefs.GetInt("StaminaUpgradeCount", 0);
        int speedUpgradeCount = PlayerPrefs.GetInt("SpeedUpgradeCount", 0);
        int incomeUpgradeCount = PlayerPrefs.GetInt("IncomeUpgradeCount", 0);

        playerData = new PlayerData(level, money, stamina, speed, income)
        {
            staminaUpgradeCount = staminaUpgradeCount,
            speedUpgradeCount = speedUpgradeCount,
            incomeUpgradeCount = incomeUpgradeCount
        };
    }

    private void OnApplicationQuit()
    {
        SavePlayerData();
    }

    public void UpdatePlayerStamina(float value)
    {
        playerData.stamina += value;
        playerData.staminaUpgradeCount++;
        OnStaminaChanged?.Invoke(playerData.stamina);
    }

    public void UpdatePlayerSpeed(float value)
    {
        playerData.speed += value;
        playerData.speedUpgradeCount++;
        OnSpeedChanged?.Invoke(playerData.speed);
    }

    public void UpdatePlayerIncome(float value)
    {
        playerData.income += value;
        playerData.incomeUpgradeCount++;
        OnIncomeChanged?.Invoke(playerData.income);
    }

    public void UpdatePlayerMoney(float upgradeCost)
    {
        playerData.money += upgradeCost;
        OnMoneyChanged?.Invoke(playerData.money);
    }

    public void UpdatePlayerLevel()
    {
        playerData.level++;
    }
}