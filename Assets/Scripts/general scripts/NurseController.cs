using UnityEngine;
using UnityEngine.AI; // Make sure your Nurse has a NavMeshAgent!

public class NurseController : MonoBehaviour
{
    public BurnLevelManager manager;
    private NavMeshAgent agent;
    private Transform targetPlayer;
    private bool isFollowing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // 1. Follow Player
        if (isFollowing && targetPlayer != null)
        {
            agent.SetDestination(targetPlayer.position);

            // 2. Check Win Condition (Distance to destination)
            // We assume the destination is set in the manager
            float distToFinal = Vector3.Distance(transform.position, manager.finalDestination.position);
            if (distToFinal < 3.0f) // If close to the injured kid
            {
                isFollowing = false;
                agent.isStopped = true;
                manager.MissionComplete();
            }
        }

        // 3. Interact Check (Player presses E near Nurse)
        if (!isFollowing && Input.GetKeyDown(KeyCode.E))
        {
            // Simple distance check to player
            float dist = Vector3.Distance(transform.position, manager.saleemPlayer.transform.position);
            if (dist < 3.0f)
            {
                manager.NurseFound();
            }
        }
    }

    public void StartFollowing(Transform player)
    {
        targetPlayer = player;
        isFollowing = true;
    }
}