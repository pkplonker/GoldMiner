using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public abstract class TweenUIPanel : MonoBehaviour, IShowHideUI
    {
        [SerializeField] private RectTransform _panelRectTransform;
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] private float _popDuration=0.5f;
        [SerializeField] private Ease _showEase = Ease.OutFlash;
        [SerializeField] private Ease _hideEase = Ease.InFlash;

        protected virtual void Awake()
        {
            if(_canvasGroup==null) _canvasGroup = GetComponent<CanvasGroup>();
            _panelRectTransform.localScale= Vector3.zero;
        }

        protected  void ShowUI()
        {
            _canvasGroup.alpha = 1f;
            _panelRectTransform.DOScale(Vector3.one, _popDuration).SetEase(_showEase);
            _canvasGroup.DOFade(1, _popDuration/2);
        }

        
        protected void HideUI()
        {
            _canvasGroup.alpha = 1f;
            _panelRectTransform.DOScale(Vector3.zero, _popDuration).SetEase(_hideEase);
            _canvasGroup.DOFade(0, _popDuration);
        }

        
        public virtual void Show() => ShowUI();
        
        public virtual void Hide() => HideUI();
        public void Disable()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void Enable()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
