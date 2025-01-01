using System;
using UnityEngine;

public class UIRunScene : MonoBehaviour
{
    [SerializeField] private GameObject joystick;
    [SerializeField] private UIBoostSpeed uIBoostSpeed;

    public Action OnUnShowUIBoostSpeed;

    private void Awake()
    {
        uIBoostSpeed.gameObject.SetActive(false);
        joystick.SetActive(false);
    }

    public void ShowUIBoostSpeed()
    {
        //uIBoostSpeed.OnEndBoostTime -= UnShowUIBoostSpeed;
        //uIBoostSpeed.OnEndBoostTime += UnShowUIBoostSpeed;

        uIBoostSpeed.gameObject.SetActive(true);
    }

    private void UnShowUIBoostSpeed()
    {
        uIBoostSpeed.gameObject.SetActive(false);
        joystick.SetActive(true);
        OnUnShowUIBoostSpeed?.Invoke();
    }
}