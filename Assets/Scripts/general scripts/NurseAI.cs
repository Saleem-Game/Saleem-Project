using UnityEngine;

public class NurseAI : MonoBehaviour
{
    public BurnLevelManager manager;
    public Animator anim;

    public float moveSpeed = 4f;
    public float stopDist = 1.5f;

    private Transform target;
    private bool following = false;
    private bool sitting = false;

    void Start()
    {
        // 1. PHYSICS FIX:
        // Force the Rigidbody to be kinematic so she doesn't fall over.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    // 2. ANIMATION ERROR FIX:
    // This function "catches" the event from the animation so the error stops.
    public void OnFootstep()
    {
        // You can add audio here later if you want, otherwise leave empty.
    }

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

                // Fix LookAt to stay level (so she doesn't tilt up/down)
                Vector3 lookPos = target.position;
                lookPos.y = transform.position.y;
                transform.LookAt(lookPos);

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

            // Fix LookAt
            Vector3 lookPos = target.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);

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