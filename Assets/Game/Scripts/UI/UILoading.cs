using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UILoading : MonoBehaviour
{
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Slider loadingSlider;
    private float transitionSpeed = 5f;

    private void Start()
    {
        loadingSlider.value = 0;
    }

    public void ShowUILoading(Action onLoadingComplete)
    {
        loadingSlider.value = 0;

        loadingSlider.DOValue(loadingSlider.maxValue, transitionSpeed)
            .OnUpdate(() =>
            {
                progressText.text = $"{Mathf.CeilToInt(loadingSlider.value * 100 / loadingSlider.maxValue)}%";
            })
            .OnComplete(() =>
            {
                onLoadingComplete?.Invoke();
            });
    }
}