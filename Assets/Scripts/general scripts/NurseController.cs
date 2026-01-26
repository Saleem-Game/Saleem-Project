using UnityEngine;

public class NurseController : MonoBehaviour
{
    public BurnLevelManager manager;
    public Animator anim; // Drag Animator here

    [Header("Settings")]
    public float moveSpeed = 4f;
    public float stopDistance = 1.5f;

    private Transform target;
    private bool isFollowing = false;
    private bool isSitting = false;

    // Called when Player presses E
    public void StartFollowing(Transform player)
    {
        isFollowing = true;
        target = player;
        if (anim) anim.SetBool("isWalking", true);
    }

    // Called when Player reaches Teacher
    public void GoToChairAndSit(Transform chairLocation)
    {
        isFollowing = false;
        target = chairLocation;
    }

    void Update()
    {
        if (isSitting) return;

        if (isFollowing && target != null)
        {
            // Move to a point BEHIND the player
            Vector3 destination = target.position - (target.forward * 1.5f);
            destination.y = transform.position.y; // Keep on ground

            float dist = Vector3.Distance(transform.position, destination);

            if (dist > stopDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
                transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                if (anim) anim.SetBool("isWalking", true);
            }
            else
            {
                if (anim) anim.SetBool("isWalking", false);
            }
        }
        else if (!isFollowing && target != null) // Moving to Chair mode
        {
            // Move directly to chair
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            transform.LookAt(target.position);

            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                // SIT DOWN
                isSitting = true;
                if (anim)
                {
                    anim.SetBool("isWalking", false);
                    anim.SetTrigger("Sit");
                }
                manager.LevelComplete(); // Tell manager we won!
            }
        }
    }
}