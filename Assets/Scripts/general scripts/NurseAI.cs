using UnityEngine;

public class NurseAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    [Tooltip("How close she gets to the player before stopping")]
    public float stoppingDistance = 2f;
    public float rotationSpeed = 5f;

    private Transform targetToFollow;
    private bool isFollowing = false;
    private Animator animator;

    // --- FIX: This variable remembers what animation is playing so it doesn't restart! ---
    private string currentAnimState;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isFollowing || targetToFollow == null)
        {
            return; // Only move if she is currently following
        }

        float distance = Vector3.Distance(transform.position, targetToFollow.position);

        if (distance > stoppingDistance)
        {
            // Calculate direction to look (ignoring Y axis so she doesn't tilt up/down)
            Vector3 lookPosition = targetToFollow.position;
            lookPosition.y = transform.position.y;
            Vector3 direction = (lookPosition - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            // Move towards the player
            transform.position = Vector3.MoveTowards(transform.position, lookPosition, moveSpeed * Time.deltaTime);

            // FIX: Safely switch to Walk
            ChangeAnimationState("Walk_N");
        }
        else
        {
            // FIX: Safely switch to Idle
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

    public void GoSit(Transform seatTransform)
    {
        StopFollowing();

        if (seatTransform != null)
        {
            transform.position = seatTransform.position;
            transform.rotation = seatTransform.rotation;
        }

        // FIX: Safely switch to Sitting
        ChangeAnimationState("Sitting");
    }

    public void GoSit()
    {
        StopFollowing();
        ChangeAnimationState("Sitting");
    }

    // ==========================================
    // THE ULTIMATE FIX: The Animation State Manager
    // ==========================================
    private void ChangeAnimationState(string newState)
    {
        // Safety check to make sure she has an animator
        if (animator == null) return;

        // If she is ALREADY playing this animation, stop and do nothing!
        if (currentAnimState == newState) return;

        // Otherwise, play the new animation and update our tracker
        animator.Play(newState);
        currentAnimState = newState;
    }
}