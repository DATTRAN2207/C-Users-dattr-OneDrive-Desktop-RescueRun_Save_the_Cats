using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIRunScene : MonoBehaviour
{
    [SerializeField] private GameObject joystick;
    [SerializeField] private GameObject halfSpin;
    [SerializeField] private TMP_Text countDownTimer;
    [SerializeField] private UIBoostSpeed uIBoostSpeed;

    public Action OnUnShowUIBoostSpeed;

    private Coroutine countdownCoroutine;

    private void Awake()
    {
        uIBoostSpeed.gameObject.SetActive(false);
        joystick.SetActive(false);
        halfSpin.SetActive(false);
        countDownTimer.gameObject.SetActive(false);
    }

    public void ShowUIBoostSpeed()
    {
        halfSpin.SetActive(true);
        uIBoostSpeed.gameObject.SetActive(true);
        countDownTimer.gameObject.SetActive(true);

        countdownCoroutine = StartCoroutine(Countdown(3, UnShowUIBoostSpeed));
    }

    private IEnumerator Countdown(int seconds, Action onComplete)
    {
        int remainingTime = seconds;

        while (remainingTime > 0)
        {
            countDownTimer.text = remainingTime.ToString();
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        countDownTimer.text = "0";
        yield return new WaitForSeconds(0.5f);

        onComplete?.Invoke();
    }

    private void UnShowUIBoostSpeed()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        uIBoostSpeed.gameObject.SetActive(false);
        countDownTimer.gameObject.SetActive(false);

        joystick.SetActive(true);

        OnUnShowUIBoostSpeed?.Invoke();
    }
}