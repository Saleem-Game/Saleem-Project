using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StickerBoard : MonoBehaviour {
  [SerializeField] GameObject board;
  [SerializeField] GameObject backgroundPanel;
  [SerializeField] StickyNoteScript [] sticky;
  [SerializeField] Image m;
  float e = 0;

  public static StickerBoard Instance;

  public StickerBoard instance;

  private void Start() {
    if (Instance == null) {
      instance = GameObject.FindFirstObjectByType<StickerBoard>();
      Instance = instance;
    }
  }

  public void OpenBoard(BoxScript.ID id, string objName, string objDescription) {
    backgroundPanel.transform.DOLocalMoveY(0, 1);
    board.transform.DOLocalMoveY(0, 1).SetEase(Ease.InOutSine).OnComplete(() => {
      int r =Random.Range(0,sticky.Length);
      
      while (sticky[r].getSet()) {
        r = Random.Range(0, sticky.Length);
      }

      string temp = objName + " " + objDescription;

      sticky[r].changeText(objName);
      Invoke(nameof(open), 0.1f);
    });

  }

  public void closeBoard()
  {
    board.transform.DOLocalMoveY(-1300, 1).SetEase(Ease.InOutSine);
    backgroundPanel.transform.DOLocalMoveY(-1523.8f, 1);
  }

  public void openTheBoard()
  {
    backgroundPanel.transform.DOLocalMoveY(0, 1);
    board.transform.DOLocalMoveY(0, 1).SetEase(Ease.InOutSine);
  }

  void open() {
    e+=0.05f;
    m.material.SetFloat("_PageCurl_movement_1", e);
    Invoke(nameof(open), 0.1f);
  }

}