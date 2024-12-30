using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private StaminaUpgradeButton staminaUpgradeButton;
    [SerializeField] private SpeedUpgradeButton speedUpgradeButton;
    [SerializeField] private IncomeUpgradeButton incomeUpgradeButton;
    [SerializeField] private Button raceButton;

    [SerializeField] protected TMP_Text currentMoney;

    private void Start()
    {
        SetupUI();

        staminaUpgradeButton.GetComponent<Button>().onClick.AddListener(UpgradeStamina);
        speedUpgradeButton.GetComponent<Button>().onClick.AddListener(UpgradeSpeed);
        incomeUpgradeButton.GetComponent<Button>().onClick.AddListener(UpgradeIncome);

        raceButton.onClick.AddListener(LoadRunScene);
    }

    private void LoadRunScene()
    {
        SceneManager.LoadScene("RunScene");
    }

    private void SetupUI()
    {
        var playerData = GameManager.Instance.playerData;
        staminaUpgradeButton.SetupUI(playerData);
        speedUpgradeButton.SetupUI(playerData);
        incomeUpgradeButton.SetupUI(playerData);

        SetupMoneyUI(playerData);
    }

    public void UpgradeStamina()
    {
        var playerData = GameManager.Instance.playerData;
        var upgradeCost = UpgradeManager.Instance.UpgradeStamina(playerData);

        if (playerData.money >= upgradeCost)
        {
            playerData.money -= upgradeCost;
            staminaUpgradeButton.SetupUI(playerData);
            SetupMoneyUI(playerData);
        }
    }

    public void UpgradeSpeed()
    {
        var playerData = GameManager.Instance.playerData;
        var upgradeCost = UpgradeManager.Instance.UpgradeSpeed(playerData);

        if (playerData.money >= upgradeCost)
        {
            playerData.money -= upgradeCost;
            speedUpgradeButton.SetupUI(playerData);
            SetupMoneyUI(playerData);
        }
    }

    public void UpgradeIncome()
    {
        var playerData = GameManager.Instance.playerData;
        var upgradeCost = UpgradeManager.Instance.UpgradeIncome(playerData);

        if (playerData.money >= upgradeCost)
        {
            playerData.money -= upgradeCost;
            incomeUpgradeButton.SetupUI(playerData);
            SetupMoneyUI(playerData);
        }
    }

    public void SetupMoneyUI(PlayerData playerData)
    {
        currentMoney.text = $"{playerData.money:F2}$";
    }
}