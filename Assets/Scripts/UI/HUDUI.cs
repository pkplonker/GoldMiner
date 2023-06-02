using System;
using Player;
using TerrainGeneration;
using UnityEngine;

namespace UI
{
  public class HUDUI :  MonoBehaviour, IShowHideUI
  {
    private CanvasGroup _canvasGroup;
    private void Awake()
    {
      _canvasGroup = GetComponent<CanvasGroup>();
      Hide();
    }

    private void OnEnable() => MapGenerator.OnMapGenerated += StartGame;

   
    private void OnDisable()=> MapGenerator.OnMapGenerated  -= StartGame;

    private void StartGame(float f) => Show();


    public void Show()
    {
      _canvasGroup.alpha = 1;
      _canvasGroup.interactable = true;
      _canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
      _canvasGroup.alpha = 0;
      _canvasGroup.interactable = false;
      _canvasGroup.blocksRaycasts = false;
    }

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
