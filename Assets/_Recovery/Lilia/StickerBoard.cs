using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StickerBoard : MonoBehaviour {
  [SerializeField] GameObject board;
  [SerializeField] GameObject backgroundPanel;
  [SerializeField] StickyNoteScript[] sticky;
  [SerializeField] DescriptionScript description;
  [SerializeField] Image[] stickers;

  public static StickerBoard Instance;

  private Coroutine placeSticker;


  private void Awake() {
    if (Instance == null) {
      Instance = GameObject.FindFirstObjectByType<StickerBoard>();

      for (int i = 0; i < stickers.Length; i++) {
        if (stickers[i] != null && stickers[i].material != null)
          stickers[i].material = new Material(stickers[i].material);
      }
    }
  }

  public void OpenBoard(BoxScript.ID id, string objName, string objDescription)
  {
    int r = Random.Range(0, sticky.Length);

    int attempts = 0;
    while (sticky[r].getSet() && attempts < sticky.Length)
    {
      r = (r + 1) % sticky.Length; 
      attempts++;
    }

    sticky[r].changeText(objName, objDescription);
    description.changeText(objName, objDescription);

    backgroundPanel.transform.DOLocalMoveY(0, 1);

    board.transform.DOLocalMoveY(-62, 1).SetEase(Ease.InOutSine).OnComplete(() =>
    {
      StartStickerPlace(id);
    });
  }

  private void StartStickerPlace(BoxScript.ID id) {
    if (placeSticker != null) StopCoroutine(placeSticker);
    placeSticker = StartCoroutine(StickerPlaceRoutine((int)id, 0.1f));
  }

  private IEnumerator StickerPlaceRoutine(int stickerIndex, float stepDelay) {
    if (stickerIndex < 0 || stickerIndex >= stickers.Length) yield break;
    var img = stickers[stickerIndex];
    if (img == null || img.material == null) yield break;

    float e = 0f;
    const float step = 0.05f;
    const float max = 1f;
    img.gameObject.SetActive(true);
    while (e < max) {
      e += step;
      img.material.SetFloat("_PageCurl_movement_1", e);
      yield return new WaitForSeconds(stepDelay);
    }
  }

  public void closeBoard() {
    board.transform.DOLocalMoveY(-1456, 1).SetEase(Ease.InOutSine);
    backgroundPanel.transform.DOLocalMoveY(-1523.8f, 1);
  }

  public void openTheBoard() {
    backgroundPanel.transform.DOLocalMoveY(0, 1);
    board.transform.DOLocalMoveY(-62, 1).SetEase(Ease.InOutSine);
  }

  public void UpdateDescription(string newName, string newDesc) {
    if (description != null) {
      description.changeText(newName, newDesc);
    }
  }

}