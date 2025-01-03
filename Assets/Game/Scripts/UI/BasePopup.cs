using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BasePopup : MonoBehaviour
{
    protected Action<object> OnShowCallback;
    private Action OnHideCallback;
    protected Action OnShownCallback;
    private Action OnHiddenCallback;

    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        OnShowCallback += OnShow;
        OnHideCallback += OnHide;
        OnShownCallback += OnShown;
        OnHiddenCallback += OnHidden;
    }

    protected virtual void Start() { }

    protected virtual void OnDestroy()
    {
        OnShowCallback -= OnShow;
        OnHideCallback -= OnHide;
        OnShownCallback -= OnShown;
        OnHiddenCallback -= OnHidden;
    }

    public virtual void Show(object data = null)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        OnShowCallback?.Invoke(data);
        canvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            OnShownCallback.Invoke();
        });

        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.Linear);
    }

    public virtual void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        OnHideCallback?.Invoke();
        canvasGroup.DOFade(0f, 0.5f);
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            OnHiddenCallback.Invoke();
        });
    }

    protected virtual void OnShow(object data)
    {
    }

    protected virtual void OnHide()
    {
    }

    protected virtual void OnShown()
    {
    }

    protected virtual void OnHidden()
    {
    }
}