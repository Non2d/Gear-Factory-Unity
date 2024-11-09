using UnityEngine;
using UnityEngine.UI;

public class CubeBossAnimReceiver : MonoBehaviour
{
    [SerializeField] private IngameSceneController sc;
    [SerializeField] private GameObject CubeBossBase;
    [SerializeField] private GameObject BossHitPointGauge;
    [SerializeField] private SO_GearFactory gf;
    [SerializeField] private GameObject Goal;
    [SerializeField] private GameObject DeathCollision;

    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private AudioSource explodeAudio;
    [SerializeField] private AudioClip explodeClip;



    private UIBossHitPoint UIbossHitPointGauge;
    private CubeBossController bossCtrl;

    void Start()
    {
        transform.position = new Vector3(0, 11, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        UIbossHitPointGauge = BossHitPointGauge.GetComponent<UIBossHitPoint>();
        UIbossHitPointGauge.UpdateGauge();
        bossCtrl = CubeBossBase.GetComponent<CubeBossController>();

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

    void Update()
    {
    }

    public void GetDamage(int damage)
    {
        gf.cubeBossHp -= damage;
        UIbossHitPointGauge.UpdateGauge();

        if (gf.cubeBossHp <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        explodeAudio.Play();
        explosionEffect.Play();
        Goal.SetActive(true);
        DisableCubeBossModel();
        Destroy(BossHitPointGauge);
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
                Vector3 worldNormal = contact.normal;
                Vector3 localNormal = transform.InverseTransformDirection(worldNormal);
                FaceDirection contactFace = GetContactFace(localNormal);
                Debug.Log("PlayerがCubeBossの" + contactFace + "面に接触しました: " + collision.gameObject.name);
                if (contactFace == FaceDirection.Down)
                {
                    sc.GivePlayerDamage(10000);
                }

                // 反射ベクトルを計算
                Vector3 incomingVelocity = collision.relativeVelocity;
                Vector3 reflectVelocity = Vector3.Reflect(incomingVelocity, worldNormal);

                // 力を加える
                Rigidbody rb = collision.rigidbody;
                float reflectForce = 5;

                if (rb != null)
                {
                    if (contactFace == FaceDirection.Back)
                    {
                        reflectForce = 2;

                    }
                    else if (contactFace == FaceDirection.Up)
                    {
                        reflectForce = 10;
                    }
                    int damage = (int)(100 * rb.velocity.magnitude / reflectForce);
                    GetDamage(damage);
                    Debug.Log("Damage: " + (int)(rb.velocity.magnitude / reflectForce) + "Velocity: " + rb.velocity.magnitude + "ReflectForce: " + reflectForce);
                    rb.AddForce(reflectForce * reflectVelocity, ForceMode.Impulse);
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