using UnityEngine;
using UnityEngine.EventSystems;

public class UIFeatureBase : MonoBehaviour, IPointerClickHandler
{
    protected float lastTapTime;
    private float cooldown = 0.1f;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastTapTime < cooldown)
        {
            return;
        }

        lastTapTime = Time.time;
    }
}