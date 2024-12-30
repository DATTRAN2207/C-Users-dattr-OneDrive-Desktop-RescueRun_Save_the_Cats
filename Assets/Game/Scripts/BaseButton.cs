using TMPro;
using UnityEngine;

public class BaseButton : MonoBehaviour
{
    [SerializeField] protected TMP_Text currentValue;
    [SerializeField] protected TMP_Text upgradeCost;

    public virtual void SetupUI(PlayerData playerData)
    {

    }
}