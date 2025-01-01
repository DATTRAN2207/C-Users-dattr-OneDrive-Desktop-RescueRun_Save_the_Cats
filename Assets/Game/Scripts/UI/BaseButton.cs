using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BaseButton : MonoBehaviour
{
    [SerializeField] protected TMP_Text currentValue;
    [SerializeField] protected TMP_Text upgradeCost;
    [SerializeField] protected Image buttonBackground;

    public Button button;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
    }

    protected virtual void SetStatusOfButton(bool status)
    {
        if (status)
        {
            buttonBackground.color = new Color32(0, 127, 255, 255);
            button.interactable = true;
        }
        else
        {
            buttonBackground.color = new Color32(150, 150, 150, 255);
            button.interactable = false;
        }
    }

    protected virtual void UpdateText(float playerStasValue)
    {
    }
}