using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class NurseAI : MonoBehaviour
{
    public Animator anim;
    private NavMeshAgent _agent;
    private Transform _target;
    private bool _isFollowing = false;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        // Physics Setup (Logic maintained)
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void StartFollowing(Transform player)
    {
        _isFollowing = true;
        _target = player;
        anim.SetBool("isWalking", true);
    }

    public void GoSit(Transform chair)
    {
        _isFollowing = false;
        _target = null;
        _agent.enabled = false; // Disable NavMesh to snap position

        transform.position = chair.position;
        transform.rotation = chair.rotation;

        anim.SetBool("isWalking", false);
        anim.SetTrigger("Sit");
    }

    void Update()
    {
        if (_isFollowing && _target != null)
        {
            _agent.SetDestination(_target.position);

            // Optional: Manual distance check if NavMesh stopping distance isn't enough
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                anim.SetBool("isWalking", false);
            }
            else
            {
                anim.SetBool("isWalking", true);
            }
        }
    }

    // Event Handler
    public void OnFootstep() { }
}