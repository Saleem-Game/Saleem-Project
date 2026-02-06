using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    public PlayableDirector director;
    public GameObject interactPrompt;
    public float interactDistance = 3f;

    [Header("Module References")]
    public PlayerCutsceneHandler playerHandler;
    public CharacterResetManager npcManager;

    private bool isPlayerClose = false;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, playerHandler.transform.position);

        if (distance <= interactDistance)
        {
            if (!isPlayerClose) ShowUI(true);
            if (Input.GetKeyDown(KeyCode.E)) StartCutscene();
        }
        else if (isPlayerClose)
        {
            ShowUI(false);
        }
    }

    void StartCutscene()
    {
        ShowUI(false);
        playerHandler.PrepareForCutscene();
        director.Play();
        director.stopped += Finish;
    }

    void Finish(PlayableDirector obj)
    {
        playerHandler.ResumeGameplay();
        if (npcManager != null) npcManager.ResetAll();
        director.stopped -= Finish;
    }

    void ShowUI(bool show)
    {
        isPlayerClose = show;
        if (interactPrompt != null) interactPrompt.SetActive(show);
    }
}