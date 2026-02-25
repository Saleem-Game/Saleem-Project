using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;
using DG.Tweening; 

public class StickyNoteScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
  public TextMeshProUGUI text;
  private bool set;

  private string myName;
  private string myDescription;

  [SerializeField] private float hoverScale = 1.1f;
  [SerializeField] private float animationDuration = 0.2f;

  public void changeText(string name, string desc)
  {
    this.myName = name;
    this.myDescription = desc;
    this.text.isRightToLeftText = true;
    this.text.text = name;
    this.text.ForceMeshUpdate();
    set = true;
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    if (set)
    {
      StickerBoard.Instance.UpdateDescription(myName, myDescription);
    }
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (set) 
    {
      transform.DOScale(Vector3.one * hoverScale, animationDuration).SetEase(Ease.OutBack);
    }
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (set)
    {
      transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutSine);
    }
  }

  public bool getSet() {
    return set;
  }
}
