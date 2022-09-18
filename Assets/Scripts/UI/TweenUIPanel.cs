using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class TweenUIPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _panelRectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _popDuration=0.5f;
    [SerializeField] private Ease _showEase = Ease.OutFlash;
    [SerializeField] private Ease _hideEase = Ease.InFlash;

    protected void ShowUI()
    {
        _canvasGroup.alpha = 0f;
        _panelRectTransform.DOScale(Vector3.one, _popDuration).SetEase(_showEase);
        _canvasGroup.DOFade(1, _popDuration);
    }

    protected void HideUI(Action callback)
    {
        _canvasGroup.alpha = 1f;
        _panelRectTransform.DOScale(Vector3.zero, _popDuration).SetEase(_hideEase).OnComplete(() =>
        {
            Callback(callback);
        });
        _canvasGroup.DOFade(0, _popDuration);
    }

    private void Callback(Action callback)
    {
        callback?.Invoke();
        Destroy(gameObject);
    }
}
