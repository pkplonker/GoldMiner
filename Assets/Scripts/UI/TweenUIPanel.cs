using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public abstract class TweenUIPanel : MonoBehaviour, IShowHideUI
    {
        [SerializeField] private RectTransform panelRectTransform;
        protected CanvasGroup canvasGroup;
        [SerializeField] private float popDuration=0.5f;
        [SerializeField] private Ease showEase = Ease.OutFlash;
        [SerializeField] private Ease hideEase = Ease.InFlash;

        protected virtual void Awake()=>panelRectTransform.localScale= Vector3.zero;
        

        private void OnValidate()
        {
            if(canvasGroup==null) canvasGroup = GetComponent<CanvasGroup>();
        }

        protected  void ShowUI()
        {
            canvasGroup.alpha = 1f;
            panelRectTransform.DOScale(Vector3.one, popDuration).SetEase(showEase);
            canvasGroup.DOFade(1, popDuration/2);
        }

        
        protected void HideUI()
        {
            canvasGroup.alpha = 1f;
            panelRectTransform.DOScale(Vector3.zero, popDuration).SetEase(hideEase);
            canvasGroup.DOFade(0, popDuration);
        }

        
        public virtual void Show() => ShowUI();
        
        public virtual void Hide() => HideUI();
        public void Disable()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void Enable()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
