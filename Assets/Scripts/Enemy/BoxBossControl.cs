using UnityEngine;
using UnityEngine.AI; // NavMeshAgentを使うための宣言
using UnityEngine.Playables; // PlayableDirectorを使うための宣言

public class BoxBossControl : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Freeze
    };

    // パラメータ関数の定義
    public EnemyState state; // キャラの状態
    private Transform targetTransform; // ターゲットの情報
    private NavMeshAgent navMeshAgent; // NavMeshAgentコンポーネント
    public Animator animator; // Animatorコンポーネント
    [SerializeField]
    private PlayableDirector timeline; // PlayableDirectorコンポーネント
    private Vector3 destination; // 目的地の位置情報を格納するためのパラメータ

    void Start()
    {
        // キャラのNavMeshAgentコンポーネントとnavMeshAgentを関連付ける
        navMeshAgent = GetComponent<NavMeshAgent>();

        // キャラモデルのAnimatorコンポーネントとanimatorを関連付ける
        animator = this.gameObject.transform.GetChild(0).GetComponent<Animator>();

        SetState(EnemyState.Idle); // 初期状態をIdle状態に設定する
    }

    void Update()
    {
        // プレイヤーを目的地にして追跡する
        if (state == EnemyState.Chase)
        {
            if (targetTransform == null)
            {
                SetState(EnemyState.Idle);
            }
            else
            {
                SetDestination(targetTransform.position);
                navMeshAgent.SetDestination(GetDestination());
            }

            // 敵の向きをプレイヤーの方向に少しづつ変える
            var dir = (GetDestination() - transform.position).normalized;
            dir.y = 0;
            Quaternion setRotation = Quaternion.LookRotation(dir);

            // 算出した方向の角度を敵の角度に設定
            transform.rotation = Quaternion.Slerp(transform.rotation, setRotation, navMeshAgent.angularSpeed * 0.1f * Time.deltaTime);
        }
    }

    // 状態移行時に呼ばれる処理
    public void SetState(EnemyState tempState, Transform targetObject = null)
    {
        state = tempState;

        if (tempState == EnemyState.Idle)
        {
            navMeshAgent.isStopped = true; // キャラの移動を止める
            animator.SetBool("chase", false); // アニメーションコントローラーのフラグ切替（Chase⇒IdleもしくはFreeze⇒Idle）
        }
        else if (tempState == EnemyState.Chase)
        {
            targetTransform = targetObject; // ターゲットの情報を更新
            navMeshAgent.SetDestination(targetTransform.position); // 目的地をターゲットの位置に設定
            navMeshAgent.isStopped = false; // キャラを動けるようにする
            animator.SetBool("chase", true); // アニメーションコントローラーのフラグ切替（Idle⇒Chase）
        }
        else if (tempState == EnemyState.Attack)
        {
            navMeshAgent.isStopped = true; // キャラの移動を止める
            animator.SetBool("chase", false);
            timeline.Play(); // 攻撃用のタイムラインを再生する
        }
        else if (tempState == EnemyState.Freeze)
        {
            Invoke("ResetState", 2.0f);
        }
    }

    // 敵キャラクターの状態取得メソッド
    public EnemyState GetState()
    {
        return state;
    }

    // 目的地を設定する
    public void SetDestination(Vector3 position)
    {
        destination = position;
    }

    // 目的地を取得する
    public Vector3 GetDestination()
    {
        return destination;
    }

    public void FreezeState()
    {
        SetState(EnemyState.Freeze);
    }

    private void ResetState()
    {
        SetState(EnemyState.Idle);
    }
}