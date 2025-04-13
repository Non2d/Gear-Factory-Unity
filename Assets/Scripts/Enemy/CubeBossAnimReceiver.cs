using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor.Callbacks;

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

    private UIBossHitPoint UIbossHitPointGauge;
    private CubeBossController bossCtrl;
    private TextMeshPro bossFace;
    private Rigidbody rb;
    private float reflectForce;
    private int sinceCollideFrame = -1; // 衝突からのフレーム数をカウントする変数

    Vector3 worldNormal;
    Vector3 localNormal;
    FaceDirection contactFace;
    float SpeedBeforeCollision = 0.0f; // 衝突前の速さ

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
    }

    float previousSpeed = 0.0f; // 衝突前の速さ
    private void FixedUpdate()
    {

        Debug.Log("SPEED: " + rb.velocity.magnitude);
        if (sinceCollideFrame >= 0)
        {
            sinceCollideFrame++;
        }

        if (sinceCollideFrame == 1)
        {
            SpeedBeforeCollision = previousSpeed;
            if (contactFace == FaceDirection.Down)
            {
                sc.GivePlayerDamage(10000);
            }

            reflectForce = 0.6f;
            if(contactFace == FaceDirection.Left || contactFace == FaceDirection.Right || contactFace == FaceDirection.Up)
            {
                reflectForce = 0.4f;
            }
            else if (contactFace == FaceDirection.Forward)
            {
                reflectForce = 0.8f;
            } else if (contactFace == FaceDirection.Back)
            {
                reflectForce = 0.2f;
            }

            // 反射ベクトルを計算して力を加える．OnCollisionEnterで処理しても，結局sinceCollideFrame==2にならないと反映されない
            Vector3 reflectVelocity = Vector3.Reflect(rb.velocity, Vector3.up);
            reflectVelocity.y = 0; // 縦方向の成分をゼロにする
            rb.AddForce(1 * reflectForce * reflectVelocity, ForceMode.Impulse);

            // float SpeedAfterForced = rb.velocity.magnitude;
            // float vDifferential = Mathf.Max(0, SpeedBeforeCollision - SpeedAfterForced);
            // float damage = 2 * vDifferential; //param:m=2
            // Debug.Log("1-damage: "+ damage + "reflectForce: " + reflectForce + " SpeedBeforeCollision: " + SpeedBeforeCollision + " SpeedAfterForced: " + SpeedAfterForced);
        } else if (sinceCollideFrame == 2)
        {
            // 力を加えた後の速さを取得
            float SpeedAfterForced = rb.velocity.magnitude;
            float vDifferential = Mathf.Max(0, SpeedBeforeCollision - SpeedAfterForced);
            float damage = 2 * vDifferential; //param:m=2
            GetDamage(damage); //param:m=5
            Debug.Log("2-damage: "+ damage + "reflectForce: " + reflectForce + " SpeedBeforeCollision: " + SpeedBeforeCollision + " SpeedAfterForced: " + SpeedAfterForced);
        }

        previousSpeed = rb.velocity.magnitude; // 1F前の速さを保存
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
                worldNormal = contact.normal;
                localNormal = transform.InverseTransformDirection(worldNormal);
                contactFace = GetContactFace(localNormal);
                Debug.Log("PlayerがCubeBossの" + contactFace + "面に接触しました: " + collision.gameObject.name);

                Rigidbody rb = collision.rigidbody;
                if (rb != null)
                {
                    // ダメージ計算を次の FixedUpdate で実行するためのフレームカウンタをリセット
                    sinceCollideFrame = 0;

                    // ボスがプレイヤーに気づく
                    bossCtrl.SetState(CubeBossController.EnemyState.Chase);
                }

                SpeedBeforeCollision =  previousSpeed;
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