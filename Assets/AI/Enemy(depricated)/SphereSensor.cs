using UnityEngine;
using UnityEditor;

public class SphereSensor : MonoBehaviour
{
    [SerializeField]
    private SphereCollider searchArea = default;
    [SerializeField]
    private float searchAngle = 45f;
    [SerializeField]
    private LayerMask obstacleLayer = default;
    private BoxBossControl enemyMove = default;

    private void Start()
    {
        enemyMove = transform.parent.GetComponent<BoxBossControl>();
    }

    private void OnTriggerStay(Collider target)
    {
        if (target.tag == "Player")
        {
            var playerDirection = target.transform.position - transform.position;
            var angle = Vector3.Angle(transform.forward, playerDirection);

            if (angle <= searchAngle)
            {
                if (!Physics.Linecast(transform.position + Vector3.up, target.transform.position + Vector3.up, obstacleLayer)) // プレイヤーとの間に障害物がないとき
                {
                    float distance = Vector3.Distance(target.transform.position, transform.position);
                    if (distance <= searchArea.radius * 0.5f && distance >= searchArea.radius * 0.05f)
                    {
                        enemyMove.SetState(BoxBossControl.EnemyState.Attack);
                    }
                    else if (distance <= searchArea.radius && distance >= searchArea.radius * 0.5f && enemyMove.state == BoxBossControl.EnemyState.Idle)
                    {
                        enemyMove.SetState(BoxBossControl.EnemyState.Chase, target.transform); // センサーに入ったプレイヤーをターゲットに設定して、追跡状態に移行する。
                    }
                }
            }
            else if (angle > searchAngle)
            {
                enemyMove.SetState(BoxBossControl.EnemyState.Idle);
            }
        }
    }

#if UNITY_EDITOR
    // サーチする角度表示
    private void OnDrawGizmos()
    {
        if (searchArea != null)
        {
            Handles.color = Color.red;
            Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2f, searchArea.radius);
        }
    }
#endif
}