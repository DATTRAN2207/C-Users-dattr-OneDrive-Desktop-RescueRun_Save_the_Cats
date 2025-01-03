using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MoneyProgress : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    private float targetAmount = 2f;
    private float currentAmount = 0f;
    private float fillSpeed = 0.3f;

    private Queue<float> moneyQueue = new Queue<float>();
    private bool isProcessing = false;

    private void Start()
    {
        UpdateProgressUI();
    }

    public void AddMoney(float amount)
    {
        moneyQueue.Enqueue(amount);

        if (!isProcessing)
        {
            ProcessQueue();
        }
    }

    private void ProcessQueue()
    {
        if (moneyQueue.Count > 0)
        {
            isProcessing = true;

            float nextAmount = moneyQueue.Dequeue();

            float newAmount = Mathf.Min(currentAmount + nextAmount, targetAmount);

            DOTween.To(() => currentAmount, x =>
            {
                currentAmount = x;
                UpdateProgressUI();
            }, newAmount, fillSpeed).OnComplete(() =>
            {
                if (currentAmount >= targetAmount)
                {
                    OnTargetReached();
                }
                ProcessQueue();
            });
        }
        else
        {
            isProcessing = false;
        }
    }

    private void UpdateProgressUI()
    {
        float progress = currentAmount / targetAmount;
        progressBar.fillAmount = progress;
    }

    private void OnTargetReached()
    {
        GameManager.Instance.UpdatePlayerMoney(GameManager.Instance.PlayerData.income);
        ResetProgress();
    }

    private void ResetProgress()
    {
        currentAmount = 0f;
        progressBar.fillAmount = 0f;
    }
}