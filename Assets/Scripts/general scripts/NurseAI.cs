using UnityEngine;

public class NurseAI : MonoBehaviour
{
    public BurnLevelManager manager;
    public Animator anim; // Drag the Animator component here

    public float moveSpeed = 4f;
    public float stopDist = 1.5f;

    private Transform target;
    private bool following = false;
    private bool sitting = false;

    // Manager calls this when you press E
    public void StartFollowing(Transform player)
    {
        following = true;
        target = player;
        if (anim) anim.SetBool("isWalking", true);
    }

    // Manager calls this when you arrive at teacher
    public void GoSit(Transform chairPos)
    {
        following = false;
        target = chairPos;
        // Keep walking animation on while moving to chair
        if (anim) anim.SetBool("isWalking", true);
    }

    void Update()
    {
        if (sitting || target == null) return;

        // Mode 1: Following Player (Stay behind)
        if (following)
        {
            Vector3 dest = target.position - (target.forward * 1.5f);
            dest.y = transform.position.y; // Keep feet on ground

            if (Vector3.Distance(transform.position, dest) > stopDist)
            {
                transform.position = Vector3.MoveTowards(transform.position, dest, moveSpeed * Time.deltaTime);
                transform.LookAt(target.position);
                if (anim) anim.SetBool("isWalking", true);
            }
            else
            {
                if (anim) anim.SetBool("isWalking", false);
            }
        }
        // Mode 2: Going to Chair
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            transform.LookAt(target.position);

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                sitting = true;
                if (anim)
                {
                    anim.SetBool("isWalking", false);
                    anim.SetTrigger("Sit");
                }
                // Tell manager the level is DONE
                manager.LevelComplete();
            }
        }
    }
}