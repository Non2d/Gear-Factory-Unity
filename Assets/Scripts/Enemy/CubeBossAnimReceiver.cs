using UnityEngine;
using TMPro;
using System.Collections;

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

    [SerializeField] private GameObject bossFaceObj;
    [SerializeField] private GameObject bossFacesObj; // 2つ目のボス顔オブジェクト

    private UIBossHitPoint UIbossHitPointGauge;
    private CubeBossController bossCtrl;
    private TextMeshPro bossFace;
    private float previousSpeed = 0f; // 前回の速度を保存する変数

    void Start()
    {
        transform.position = new Vector3(0, 11, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        UIbossHitPointGauge = BossHitPointGauge.GetComponent<UIBossHitPoint>();
        UIbossHitPointGauge.UpdateGauge();
        bossCtrl = CubeBossBase.GetComponent<CubeBossController>();
        bossFace = bossFaceObj.GetComponent<TextMeshPro>();

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
                float reflectForce = 0.6f;

                if (rb != null)
                {
                    if (contactFace == FaceDirection.Back)
                    {
                        reflectForce = 0.2f;
                    }
                    else if (contactFace == FaceDirection.Forward)
                    {
                        reflectForce = 0.9f;
                    }
                    int damage = (int)(40 * (previousSpeed - rb.velocity.magnitude) / reflectForce);
                    GetDamage(damage);
                    Debug.Log($"Damage: {damage}, speed: {rb.velocity.magnitude}, previousSpeed: {previousSpeed}");
                    rb.AddForce(reflectForce * reflectVelocity, ForceMode.Impulse);
                    bossCtrl.SetState(CubeBossController.EnemyState.Chase);
                }
            }
            
            if (collision.relativeVelocity.magnitude > 0.01f)
            {
                previousSpeed = collision.relativeVelocity.magnitude; // 現在の速度を保存
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