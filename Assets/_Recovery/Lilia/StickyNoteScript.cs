using UnityEngine;
using TMPro;
using System;

public class StickyNoteScript : MonoBehaviour
{
  public TextMeshProUGUI text;
  private bool set;

  public void changeText(string text) {
    this.text.isRightToLeftText = true;
    this.text.text = text;
    this.text.ForceMeshUpdate();
    set = true;
  }

  public bool getSet() {
    return set;
  }
}
