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

    void Start()
    {
        // Automatically grab the Animator if it's on the Nurse
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // If she is not told to follow, ensure she stands still
        if (!isFollowing || targetToFollow == null)
        {
            if (animator != null) animator.SetFloat("Speed", 0f);
            return;
        }

        // Check how far away the player is
        float distance = Vector3.Distance(transform.position, targetToFollow.position);

        if (distance > stoppingDistance)
        {
            // 1. Calculate direction to look (ignoring Y axis so she doesn't tilt up/down)
            Vector3 lookPosition = targetToFollow.position;
            lookPosition.y = transform.position.y;

            Vector3 direction = (lookPosition - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            // 2. Move towards the player
            transform.position = Vector3.MoveTowards(transform.position, lookPosition, moveSpeed * Time.deltaTime);

            // 3. Play Walk Animation
            if (animator != null) animator.SetFloat("Speed", 1f);
        }
        else
        {
            // She reached the player, stop walking and play Idle animation
            if (animator != null) animator.SetFloat("Speed", 0f);
        }
    }

    // Called by CafeteriaLevel.cs when Dialogue finishes
    public void StartFollowing(Transform player)
    {
        targetToFollow = player;
        isFollowing = true;
    }

    // Called by CafeteriaLevel.cs when arriving at the empty chair
    public void StopFollowing()
    {
        isFollowing = false;
        targetToFollow = null;

        // Force her back to Idle animation
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    // If your BurnLevelManager calls: nurse.GoSit(chairTransform);
    public void GoSit(Transform seatTransform)
    {
        // 1. Stop following the player
        StopFollowing();

        // 2. Snap the nurse's position and rotation to exactly match the chair
        if (seatTransform != null)
        {
            transform.position = seatTransform.position;
            transform.rotation = seatTransform.rotation;
        }

        // 3. Trigger the sitting animation
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f); // Stop walking

            // If you have a sitting animation, you can trigger it like this:
            // animator.SetBool("IsSitting", true); 
        }
    }

    // Just in case your BurnLevelManager calls it WITHOUT a Transform like: nurse.GoSit();
    public void GoSit()
    {
        StopFollowing();

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            // animator.SetBool("IsSitting", true); 
        }
    }
}