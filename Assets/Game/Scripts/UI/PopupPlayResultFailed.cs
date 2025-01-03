using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PopupPlayResultFailed : BasePopup
{
    [Header("UI Elements")]
    [SerializeField] private Slider distanceSlider;
    [SerializeField] private TMP_Text percentageText;
    [SerializeField] private TMP_Text catsRescuedText;
    [SerializeField] private TMP_Text completedTimeText;
    [SerializeField] private TMP_Text topSpeedText;
    [SerializeField] private TMP_Text moneyRewardText;
    [SerializeField] private Button backToMenuButton;

    public Action OnBackToMenuButtonClicked;
    private LevelData levelData;

    protected override void Start()
    {
        backToMenuButton.onClick.AddListener(() =>
        {
            OnBackToMenuButtonClicked?.Invoke();
        });
    }

    public void SetLevelData(LevelData levelData)
    {
        this.levelData = levelData;
    }

    public void UpdateDistance(float distance, int levelIndex)
    {
        float completionPercentage = Mathf.Clamp01(distance / levelData.levels[levelIndex].roadLength) * 100f;
        distanceSlider.value = completionPercentage;
        percentageText.text = $"{completionPercentage:F1}%";
    }

    public void UpdateRescuedCats(int rescued, int levelIndex)
    {
        int totalCats = levelData.levels[levelIndex].catCount;
        int rescuedCats = Mathf.Clamp(rescued, 0, totalCats);
        catsRescuedText.text = $"Cats Rescued: {rescuedCats}/{totalCats}";
    }

    public void UpdateCompletedTime(float completedTime)
    {
        completedTimeText.text = $"Completed Time: {FormatTime(completedTime)}";
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds % 1f) * 100f);

        return $"{minutes:D2}:{seconds:D2}:{milliseconds:D2}s";
    }

    public void UpdateTopSpeed(float topSpeed)
    {
        topSpeedText.text = $"Top Speed: {topSpeed:F1} m/s";
    }

    public void UpdateMoneyReward(float money)
    {
        moneyRewardText.text = $"{money:F1}$";
    }
}