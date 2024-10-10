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
    private NavMeshAgent agent;
    [SerializeField]
    private GameObject player;

    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            agent.SetDestination(player.transform.position);
        }
    }
}
