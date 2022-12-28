using UnityEngine;
using UnityEngine.AI;

public class BolaAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public GameObject[] players;

    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    // Patrolling
    public Vector3 walkPoint;
    public float walkPointRange;
    private bool WalkPointSet;

    // Attacc
    public float timeBetweenAttacks;
    private bool IsAlreadyAttacked;

    // State
    public float sightRange;
    public float attackRange;
    public bool playerInSightRange;
    public bool playerInAttackRange;

    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    private void Update()
    {
        // Check sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
    }
}
