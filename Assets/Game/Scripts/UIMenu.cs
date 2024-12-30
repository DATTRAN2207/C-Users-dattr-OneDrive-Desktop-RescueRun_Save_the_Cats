using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private StaminaUpgradeButton staminaUpgradeButton;
    [SerializeField] private Button speedUpgradeButton;
    [SerializeField] private Button incomeUpgradeButton;
    [SerializeField] private Button raceButton;

    [SerializeField] protected TMP_Text currentMoney;

    private void Start()
    {
        SetupUI();

        staminaUpgradeButton.GetComponent<Button>().onClick.AddListener(UpgradeStamina);
        //speedUpgradeButton.onClick.AddListener(UpgradeSpeed);
        //incomeUpgradeButton.onClick.AddListener(UpgradeIncome);
    }

    private void SetupUI()
    {
        var playerData = GameManager.Instance.playerData;
        staminaUpgradeButton.SetupUI(playerData);

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
            GameManager.Instance.SavePlayerData();
        }
    }

    public void SetupMoneyUI(PlayerData playerData)
    {
        currentMoney.text = $"{playerData.money:F2}$";
    }
}