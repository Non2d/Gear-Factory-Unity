using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CubeBossController : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Freeze
    };

    public EnemyState state;
    // Start is called before the first frame update
    private Transform targetTransform;
    private NavMeshAgent agent;
    [SerializeField]
    private GameObject player;
    

    void Start()
    {
        state = EnemyState.Idle;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        DebugSwitchState();

        switch (state)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Chase:
                targetTransform = player.transform;
                agent.SetDestination(targetTransform.position);
                break;

            case EnemyState.Attack:
                break;

            case EnemyState.Freeze:
                break;
        }
    }

    public void SetState(EnemyState newState, Transform targetObject = null) //切り替わった瞬間の処理
    {
        state = newState;

        switch (state)
        {
            case EnemyState.Idle:
                Debug.Log("Idle");
                agent.isStopped = true;
                break;

            case EnemyState.Chase:
                Debug.Log("Chase");
                if(targetObject == null)
                {
                    SetState(EnemyState.Idle);
                    Debug.LogError("targetObject is null during EnemyState.Chase");
                }
                else
                {
                    targetTransform = targetObject.transform;
                    agent.SetDestination(targetTransform.position);
                    agent.isStopped = false;
                }
                break;

            case EnemyState.Attack:
                Debug.Log("Attack");
                break;

            case EnemyState.Freeze:
                Debug.Log("Freeze");
                break;
        }
    }

    private void DebugSwitchState()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetState(EnemyState.Idle);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetState(EnemyState.Chase);
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetState(EnemyState.Attack);
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetState(EnemyState.Freeze);
        }
    }

    public EnemyState GetState()
    {
        return state;
    }
}
