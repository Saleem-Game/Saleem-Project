using UnityEngine;
using StarterAssets; // Required for the Starter Assets package

public class PlayerCutsceneHandler : MonoBehaviour
{
    private ThirdPersonController moveScript;
    private Animator anim;

    void Awake()
    {
        moveScript = GetComponent<ThirdPersonController>();
        anim = GetComponent<Animator>();
    }

    public void PrepareForCutscene()
    {
        if (moveScript != null) moveScript.enabled = false;
        // Optionally trigger an Idle animation here
    }

    public void ResumeGameplay()
    {
        if (moveScript != null) moveScript.enabled = true;

        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }
    }
}