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
            PlayerData = new PlayerData(currentLevel: 0, currentMoney: 50f, initialStamina: 60f, initialSpeed: 1f, initialIncome: 3f);
            SavePlayerData();
            PlayerPrefs.SetInt(PlayerInitializedKey, 1);
        }
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("Level", PlayerData.level);
        PlayerPrefs.SetFloat("Money", PlayerData.money);
        PlayerPrefs.SetFloat("Stamina", PlayerData.stamina);
        PlayerPrefs.SetFloat("Speed", PlayerData.speed);
        PlayerPrefs.SetFloat("Income", PlayerData.income);

        PlayerPrefs.SetInt("StaminaUpgradeCount", PlayerData.staminaUpgradeCount);
        PlayerPrefs.SetInt("SpeedUpgradeCount", PlayerData.speedUpgradeCount);
        PlayerPrefs.SetInt("IncomeUpgradeCount", PlayerData.incomeUpgradeCount);

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

        PlayerData = new PlayerData(level, money, stamina, speed, income)
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

    public void UpdatePlayerSpeed()
    {

    }

    public void UpdatePlayerIncome()
    {

    }

    public void UpdatePlayerMoney(float upgradeCost)
    {
        playerData.money -= upgradeCost;
        OnMoneyChanged?.Invoke(playerData.money);
    }
}