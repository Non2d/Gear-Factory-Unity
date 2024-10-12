using UnityEngine;
using UnityEditor;

public class CubeBossSensor : MonoBehaviour
{
    [SerializeField]
    private SphereCollider searchArea = default;
    [SerializeField]
    private float searchAngle = 45f;
    [SerializeField]
    private LayerMask obstacleLayer = default;
    [SerializeField]
    private CubeBossController bossController = default;

    private void Start()
    {
        bossController = transform.parent.GetComponent<CubeBossController>();
    }

    private void OnTriggerStay(Collider target)
    {
        if (target.tag == "Player")
        {
            Vector3 playerDirection = target.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, playerDirection);

            if (angle <= searchAngle)
            {
                if (!Physics.Linecast(transform.position + Vector3.up, target.transform.position + Vector3.up, obstacleLayer)) // プレイヤーとの間に障害物がないとき
                {
                    float distance = Vector3.Distance(target.transform.position, transform.position);
                    if (distance <= searchArea.radius * 0.5f && distance >= searchArea.radius * 0.05f)
                    {
                        bossController.SetState(CubeBossController.EnemyState.Attack);
                    }
                    else if (distance <= searchArea.radius && distance >= searchArea.radius * 0.5f && bossController.state == CubeBossController.EnemyState.Idle)
                    {
                        bossController.SetState(CubeBossController.EnemyState.Chase, target.transform); // センサーに入ったプレイヤーをターゲットに設定して、追跡状態に移行する。
                    }
                }
            }
            else if (angle > searchAngle)
            {
                bossController.SetState(CubeBossController.EnemyState.Idle);
            }
        }
        Debug.Log(bossController.GetState());
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