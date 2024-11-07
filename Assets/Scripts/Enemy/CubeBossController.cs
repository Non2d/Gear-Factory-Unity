using UnityEngine;
using UnityEngine.AI;

public class CubeBossController : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack }

    [SerializeField] private GameObject player;
    [SerializeField] private SO_GearFactory gf;

    private EnemyState state = EnemyState.Idle;
    private NavMeshAgent agent;
    private Animator animator;
    

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        switch (state)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chase:
                HandleChaseState();
                break;
            case EnemyState.Attack:
                break;
        }
    }

    private void HandleChaseState()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            Debug.LogError("Player reference is missing.");
            SetState(EnemyState.Idle);
        }
    }

    public void SetState(EnemyState newState)
    {
        state = newState;

        switch (state)
        {
            case EnemyState.Idle:
                animator.SetBool(gf.AnimConditionAttack, false);
                break;
            case EnemyState.Chase:
                animator.CrossFadeInFixedTime(gf.AnimStateChase, 0.25f);
                break;
            case EnemyState.Attack:
                animator.SetBool(gf.AnimConditionAttack, true);
                break;
        }
    }

    public EnemyState GetState()
    {
        return state;
    }
}