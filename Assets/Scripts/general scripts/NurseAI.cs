using UnityEngine;

public class NurseAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 2f;
    public float rotationSpeed = 5f;

    private Transform targetToFollow;
    private bool isFollowing = false;
    private Animator animator;
    private string currentAnimState;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!isFollowing || targetToFollow == null) return;

        float distance = Vector3.Distance(transform.position, targetToFollow.position);

        if (distance > stoppingDistance)
        {
            Vector3 lookPosition = targetToFollow.position;
            lookPosition.y = transform.position.y;
            Vector3 direction = (lookPosition - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            transform.position = Vector3.MoveTowards(transform.position, lookPosition, moveSpeed * Time.deltaTime);
            ChangeAnimationState("Walk_N");
        }
        else
        {
            ChangeAnimationState("Idle");
        }
    }

    public void StartFollowing(Transform player)
    {
        targetToFollow = player;
        isFollowing = true;
    }

    public void StopFollowing()
    {
        isFollowing = false;
        targetToFollow = null;
        ChangeAnimationState("Idle");
    }

    private void ChangeAnimationState(string newState)
    {
        if (animator == null || currentAnimState == newState) return;
        animator.Play(newState);
        currentAnimState = newState;
    }

    // --- FIX: This stops the annoying orange errors in the console! ---
    public void OnFootstep()
    {
        // Do nothing! Just catch the animation event so it doesn't crash.
    }
}