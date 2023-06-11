using System;
using Player;
using TerrainGeneration;
using UnityEngine;

namespace UI
{
  public class HUDUI :  MonoBehaviour, IShowHideUI
  {
    private CanvasGroup canvasGroup;
    private void Awake()
    {
      canvasGroup = GetComponent<CanvasGroup>();
      Hide();
    }

    private void OnEnable() => MapGenerator.MapGenerated += StartGame;

   
    private void OnDisable()=> MapGenerator.MapGenerated  -= StartGame;

    private void StartGame(float f) => Show();


    public void Show()
    {
      canvasGroup.alpha = 1;
      canvasGroup.interactable = true;
      canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
      canvasGroup.alpha = 0;
      canvasGroup.interactable = false;
      canvasGroup.blocksRaycasts = false;
    }

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
