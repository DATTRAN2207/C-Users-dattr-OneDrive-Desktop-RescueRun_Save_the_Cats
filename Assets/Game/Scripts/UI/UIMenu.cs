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
        GameManager.Instance.OnMoneyChanged += UpdateMoneyText;
        UpdateMoneyText(GameManager.Instance.PlayerData.money);

        staminaUpgradeButton.button.onClick.AddListener(UpgradeStamina);
        speedUpgradeButton.button.onClick.AddListener(UpgradeSpeed);
        incomeUpgradeButton.button.onClick.AddListener(UpgradeIncome);

        raceButton.onClick.AddListener(LoadRunScene);
    }

    private void LoadRunScene()
    {
        SceneManager.LoadScene("RunScene");
    }

    public void UpgradeStamina()
    {
        float upgradeCost = UpgradeManager.Instance.UpgradeStamina(GameManager.Instance.PlayerData);

        if (GameManager.Instance.PlayerData.money >= upgradeCost)
        {
            GameManager.Instance.UpdatePlayerMoney(upgradeCost);
        }
    }

    public void UpgradeSpeed()
    {
        var upgradeCost = UpgradeManager.Instance.UpgradeSpeed(GameManager.Instance.PlayerData);

        if (GameManager.Instance.PlayerData.money >= upgradeCost)
        {
            GameManager.Instance.UpdatePlayerMoney(upgradeCost);
        }
    }

    public void UpgradeIncome()
    {
        var upgradeCost = UpgradeManager.Instance.UpgradeIncome(GameManager.Instance.PlayerData);

        if (GameManager.Instance.PlayerData.money >= upgradeCost)
        {
            GameManager.Instance.UpdatePlayerMoney(upgradeCost);
        }
    }

    private void UpdateMoneyText(float playerMoney)
    {
        currentMoney.text = $"{playerMoney:F2}$";
    }
}