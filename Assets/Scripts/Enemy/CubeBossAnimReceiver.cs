using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor.Callbacks;
using System.Reflection.Emit;

public class CubeBossAnimReceiver : MonoBehaviour
{
    [SerializeField] private IngameSceneController sc;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject CubeBossBase;
    [SerializeField] private GameObject BossHitPointGauge;
    [SerializeField] private SO_GearFactory gf;
    [SerializeField] private GameObject Goal;
    [SerializeField] private GameObject DeathCollision;

    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private AudioSource explodeAudio;
    [SerializeField] private AudioClip explodeClip;

    [SerializeField] private GameObject bossFaceObj;
    [SerializeField] private GameObject bossFacesObj; // 2つ目のボス顔オブジェクト

    [SerializeField] private ParticleSystem DamageEffect;

    private UIBossHitPoint UIbossHitPointGauge;
    private CubeBossController bossCtrl;
    private TextMeshPro bossFace;
    private Rigidbody rb;
    private float e;
    private int sinceCollideFrame = -1; // 衝突からのフレーム数をカウントする変数

    Vector3 worldNormal;
    Vector3 localNormal;
    FaceDirection contactFace;
    float speedBeforeCollision; // 衝突前の速さ
    Vector3 previousVelocity; // 1F前の速さを保存する変数

    Vector3 elasticCollisionForce;

    void Start()
    {
        transform.position = new Vector3(0, 11, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        UIbossHitPointGauge = BossHitPointGauge.GetComponent<UIBossHitPoint>();
        UIbossHitPointGauge.UpdateGauge();
        bossCtrl = CubeBossBase.GetComponent<CubeBossController>();
        bossFace = bossFaceObj.GetComponent<TextMeshPro>();
        rb = player.GetComponent<Rigidbody>();

        if (explodeAudio != null)
        {
            explodeAudio.loop = false;
            explodeAudio.clip = explodeClip;
            explodeAudio.mute = false;
            explodeAudio.volume = 0.1f; //param
            explodeAudio.Stop(); // SEは最初は停止
        }

        explosionEffect.Stop(); // 爆発エフェクトは最初は非表示
        DamageEffect.Stop();
    }

    private void FixedUpdate()
    {
        if (sinceCollideFrame >= 0)
        {
            sinceCollideFrame++;
        }

        if (sinceCollideFrame == 1)
        {
            speedBeforeCollision = previousVelocity.magnitude; // 衝突前の速さを保存
            if (contactFace == FaceDirection.Down)
            {
                sc.GivePlayerDamage(10000);
            }

            // e の値を条件によって変更する例（必要ならコメントを外す）
            e = 0.9f;
            if (contactFace == FaceDirection.Left || contactFace == FaceDirection.Right || contactFace == FaceDirection.Up)
            {
                e = 5.0f;
            }
            else if (contactFace == FaceDirection.Forward)
            {
                e = 10.0f;
            }
            else if (contactFace == FaceDirection.Back)
            {
                e = 0.0f;
            }

            // 衝突面の実際の法線を使用して反射ベクトルを計算する
            Vector3 reflectVelocity = Vector3.Reflect(previousVelocity, worldNormal);
            Vector3 muki = reflectVelocity.normalized; // 反射ベクトルの向き
            float ookisa = 2 * previousVelocity.magnitude; // 反射ベクトルの大きさ
            elasticCollisionForce = muki * ookisa; // 反射力

            // 反射後、垂直方向の成分を取り除く（水平のみの反射とする）
            elasticCollisionForce.y = 0;

            // 計算した反射ベクトルに反発係数 e をかけ、力を加える
            // rb.AddForce(e * elasticCollisionForce, ForceMode.Force);
            rb.velocity += e * elasticCollisionForce / rb.mass;

            Debug.Log("ForceAdded: " + e * elasticCollisionForce + "base:" + previousVelocity);
        }
        else if (sinceCollideFrame == 2)
        {
            // 力を加えた後の速さを取得
            float SpeedAfterForced = rb.velocity.magnitude;
            float vDifferential = Mathf.Max(0, speedBeforeCollision - SpeedAfterForced);
            float damage = rb.mass * vDifferential;
            GetDamage(damage);
            Debug.Log("2-damage: " + damage + "e: " + e + " SpeedBeforeCollision: " + speedBeforeCollision + " SpeedAfterForced: " + SpeedAfterForced);
        }

        previousVelocity = rb.velocity; // 1F前の速さを保存
    }

    public void GetDamage(float damage)
    {
        gf.cubeBossHp -= damage;
        UIbossHitPointGauge.UpdateGauge();

        if (gf.cubeBossHp <= 500)
        {
            if (bossFace != null)
            {
                bossFace.text = ":(";
            }
        }

        if (gf.cubeBossHp <= 0)
        {
            Death();
        }
        else if (damage > 300)
        {
            DamageEffect.transform.position = player.transform.position;
            DamageEffect.Play();
            if (bossFace != null)
            {
                bossFace.text = ":o";
            }
            explodeAudio.Play();

            // コルーチンを開始してエフェクトをリセット
            StartCoroutine(ResetAfterEffect(explodeAudio, DamageEffect));
        }
    }

    private IEnumerator ResetAfterEffect(AudioSource audio, ParticleSystem effect)
    {
        // AudioSource の再生終了を待機
        yield return new WaitWhile(() => audio.isPlaying);

        // ボスの顔を戻す
        if (bossFace != null)
        {
            bossFace.text = gf.cubeBossHp > 500 ? ":)" : ":(";
        }

        // エフェクトを停止
        effect.Stop();
    }

    void Death()
    {
        DisableCubeBossModel();
        StartCoroutine(WaitForAllEnd(explodeAudio, explosionEffect));
        if (bossFace != null)
        {
            bossFace.text = "x(";
        }
        Destroy(BossHitPointGauge);
    }

    IEnumerator WaitForAllEnd(AudioSource audio, ParticleSystem ps)
    {
        audio.Play();
        ps.Play();

        while (audio.isPlaying || ps.IsAlive())
        {
            yield return null;
        }

        Goal.SetActive(true);
        CubeBossBase.SetActive(false);
    }

    public void OnAttackAnimEnd()
    {
        bossCtrl.SetState(CubeBossController.EnemyState.Idle);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // 接触点ごとに実際の衝突面の法線を取得
                Vector3 worldNormal = contact.normal;

                // ワールド空間の法線をローカル座標系に変換（GetContactFace用）
                Vector3 localNormal = transform.InverseTransformDirection(worldNormal);
                FaceDirection contactFace = GetContactFace(localNormal);
                Debug.Log("PlayerがCubeBossの" + contactFace + "面に接触しました: " + collision.gameObject.name);

                Rigidbody rb = collision.rigidbody;
                if (rb != null)
                {



                    // 次の FixedUpdate でダメージ計算を行うためのフレームカウンタをリセットする
                    sinceCollideFrame = 0;

                    // ボスがプレイヤーに気づく状態に変更
                    bossCtrl.SetState(CubeBossController.EnemyState.Chase);
                }
            }
        }
    }

    private enum FaceDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back,
        Unknown
    }

    /// <summary>
    /// 法線ベクトルから接触面を判定します。
    /// </summary>
    /// <param name="normal">法線ベクトル</param>
    /// <returns>接触面の方向を示すFaceDirection列挙型</returns>
    private FaceDirection GetContactFace(Vector3 normal)
    {
        FaceDirection contactFaceDirection;

        if (Vector3.Dot(normal, Vector3.up) > 0.5f)
        {
            contactFaceDirection = FaceDirection.Down;
        }
        else if (Vector3.Dot(normal, Vector3.down) > 0.5f)
        {
            contactFaceDirection = FaceDirection.Up;
        }
        else if (Vector3.Dot(normal, Vector3.left) > 0.5f)
        {
            contactFaceDirection = FaceDirection.Left;
        }
        else if (Vector3.Dot(normal, Vector3.right) > 0.5f)
        {
            contactFaceDirection = FaceDirection.Right;
        }
        else if (Vector3.Dot(normal, Vector3.forward) > 0.5f)
        {
            contactFaceDirection = FaceDirection.Back;
        }
        else if (Vector3.Dot(normal, Vector3.back) > 0.5f)
        {
            contactFaceDirection = FaceDirection.Forward;
        }
        else
        {
            contactFaceDirection = FaceDirection.Unknown;
        }

        Debug.Log($"GetContactFace: Normal = {normal}, Contact Face = {contactFaceDirection}");
        return contactFaceDirection;
    }

    private void DisableCubeBossModel()
    {
        DeathCollision.SetActive(false);
        Transform cubeBossModelTransform = CubeBossBase.transform.Find("CubeBossModel");
        if (cubeBossModelTransform != null)
        {
            GameObject cubeBossModel = cubeBossModelTransform.gameObject;

            // Rendererを無効化
            bossFacesObj.SetActive(false);
            var renderers = cubeBossModel.GetComponents<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }

            // Colliderを無効化
            var colliders = cubeBossModel.GetComponents<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
        }
        else
        {
            Debug.LogError("CubeBossModel not found as a child of CubeBossBase.");
        }
    }
}