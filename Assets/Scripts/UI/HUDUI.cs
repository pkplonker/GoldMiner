using UnityEngine;

namespace UI
{
  public class HUDUI :  MonoBehaviour
  {
    private CanvasGroup _canvasGroup;
    private void Awake()
    {
      _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(float s = 0)
    {
      _canvasGroup.alpha = 1;
      _canvasGroup.interactable = true;
      _canvasGroup.blocksRaycasts = true;
    }

    public void Hide(float s = 0)
    {
      _canvasGroup.alpha = 0;
      _canvasGroup.interactable = false;
      _canvasGroup.blocksRaycasts = false;
    }
  }
}
