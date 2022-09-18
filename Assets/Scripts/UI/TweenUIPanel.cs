using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public abstract class TweenUIPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _panelRectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _popDuration=0.5f;
        [SerializeField] private Ease _showEase = Ease.OutFlash;
        [SerializeField] private Ease _hideEase = Ease.InFlash;

        protected virtual void Awake()
        {
            _panelRectTransform.localScale= Vector3.zero;
        }

        protected virtual void ShowUI()
        {
            //_canvasGroup.alpha = 1f;
            _panelRectTransform.DOScale(Vector3.one, _popDuration).SetEase(_showEase);
            //_canvasGroup.DOFade(1, _popDuration/2);
        }

        
        protected virtual void HideUI(Action callback)
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
}
