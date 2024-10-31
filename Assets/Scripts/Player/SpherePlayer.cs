using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class SpherePlayer : MonoBehaviour
{
    Rigidbody rb;

    float force = 100.0f;
    float torque = 10.0f;

    private bool isFadingOut = false;
    private float fadeOutSpeed = 0.1f; // フェードアウトの速度 param

    [SerializeField]
    private PlayerCamera playerCamera;
    Vector3 cameraForward;

    // ジャンプ可能かどうかを管理するフラグ
    bool canJump = true;

    [SerializeField]
    private IngameSceneController sc;

    private ParticleSystem[] psArray;
    private ParticleSystem psFire;
    private ParticleSystem psExplode;

    [SerializeField]
    private AudioSource fireAudio;
    [SerializeField]
    private AudioClip fireClip;

    [SerializeField]
    private AudioSource explodeAudio;
    [SerializeField]
    private AudioClip explodeClip;

    [SerializeField]
    private GameObject SpeedLines;
    private ParticleSystem speedLinesPS;

    private float currentFov;
    private float fovVelocity;

    private bool isKeyEnabled = true;

    [SerializeField] private Material defaultMaterial;

    [SerializeField] private Material draggedMaterial;

    [SerializeField] private Material undraggedMaterial;

    /// <summary>
    /// 空気抵抗の算出に使う前フレームの速度
    /// </summary>
    private Vector3 previousVelocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // ギャンブルシーン判定
        if (sc.isGamblingMode)
        {
            ChangeMaterial(undraggedMaterial);
        } else {
            ChangeMaterial(defaultMaterial);
        }

        rb = GetComponent<Rigidbody>(); //this.は省略可能
        rb.maxAngularVelocity = 100.0f;

        speedLinesPS = SpeedLines.GetComponent<ParticleSystem>();

        if (fireAudio != null)
        {
            fireAudio.loop = true;
            fireAudio.clip = fireClip;
            fireAudio.mute = false;
            fireAudio.volume = 0.1f; //param
            fireAudio.Stop(); // SEは最初は停止
        }

        if (explodeAudio != null)
        {
            explodeAudio.loop = false;
            explodeAudio.clip = explodeClip;
            explodeAudio.mute = false;
            explodeAudio.volume = 0.1f; //param
            explodeAudio.Stop(); // SEは最初は停止
        }

        // 各エフェクトを取得
        psArray = GetComponentsInChildren<ParticleSystem>();
        psFire = psArray.FirstOrDefault(ps => ps.transform.name == "FlamesBig");
        psFire.Stop(); // 炎エフェクトは最初は非表示
        psExplode = psArray.FirstOrDefault(ps => ps.transform.name == "BigExplosionEffect");
        psExplode.Stop(); // 爆発エフェクトは最初は非表示

        // SpeedLinesのParticle Systemを取得
        speedLinesPS = SpeedLines.GetComponent<ParticleSystem>();
        currentFov = playerCamera.GetFov();
    }

    void Update()
    {
        // Debug.Log(LatestVelocityDirection());

        //Playerカメラから進行方向を取得
        if (playerCamera != null)
        {
            cameraForward = playerCamera.GetForwardDirection();
        }

        //Spaceでジャンプ
        if (isKeyEnabled && !sc.isGamblingMode && Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
            canJump = false; // ジャンプ後にフラグをリセット
            sc.GivePlayerDamage(30.0f); //param
        }

        // Qキーでオプション画面を表示
        if (isKeyEnabled && Input.GetKeyDown(KeyCode.Q))
        {
            sc.Pause();
        }

        //後で，ダメージを受けてる時にパーティクルを出すように変更
        if (isKeyEnabled && !sc.isGamblingMode && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) 
        || sc.isGamblingMode && Input.GetKey(KeyCode.LeftShift))
        {
            if (!psFire.isPlaying) // パーティクルが再生されていないなら
            {
                psFire.Play(); // 再生開始
            }
            if (!fireAudio.isPlaying) // 音声が再生されていないなら
            {
                fireAudio.Play(); // 音声再生開始
            }
            fireAudio.volume = 0.1f; // 音量を最大に設定
            isFadingOut = false; // フェードアウトを停止
        }
        else
        {
            if (psFire.isPlaying) // パーティクルが再生されているなら
            {
                psFire.Stop(); // 再生停止
            }
            if (fireAudio.isPlaying && !isFadingOut) // 音声が再生されているなら
            {
                isFadingOut = true; // フェードアウトを開始
            }
        }

        // フェードアウト処理
        if (isFadingOut)
        {
            fireAudio.volume -= fadeOutSpeed * Time.deltaTime;
            if (fireAudio.volume <= 0)
            {
                fireAudio.Stop(); // 音量が0になったら停止
                fireAudio.volume = 0; // 音量を0に設定
                isFadingOut = false; // フェードアウトを停止
            }
        }

        // SpeedLinesパーティクルシステムの制御
        ControlSpeedLinesAndFov();

        LastUpdate();
    }

    void LastUpdate()
    {
        previousVelocity = rb.velocity;
    }

    private Vector3 playerForward;

    void FixedUpdate() //物理演算関連はできるだけこちらで処理。フレームレートに依存しない処理を目指す感じかな？
    {
        // Gamble Mode以外での移動処理
        playerForward = Vector3.zero;
        // WASDキーで方向を決定
        if (Input.GetKey(KeyCode.W))
        {
            playerForward += cameraForward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerForward -= cameraForward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerForward += Vector3.Cross(cameraForward, Vector3.up);
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerForward -= Vector3.Cross(cameraForward, Vector3.up);
        }

        // 決定した方向に力を加える
        if (isKeyEnabled && !sc.isGamblingMode && playerForward != Vector3.zero)
        {
            rb.AddForce(playerForward.normalized * force);
            sc.GivePlayerDamage(0.5f);
        }

        // Shiftでトルクを加える
        if (isKeyEnabled && !sc.isGamblingMode && Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 torqueDirection = Vector3.Cross(Vector3.up, playerForward);
            rb.AddTorque(torqueDirection.normalized * torque, ForceMode.Impulse);
            sc.GivePlayerDamage(1.0f); //param
        }

        float kDragForce = 0.5f;
        // Gamble Modeでの移動処理:SHIFTで空気抵抗追加
        if (isKeyEnabled && sc.isGamblingMode && Input.GetKey(KeyCode.LeftShift))
        {
            ChangeMaterial(draggedMaterial);
            kDragForce =10.0f;
            sc.GivePlayerDamage(0.1f);
        } else {
            ChangeMaterial(undraggedMaterial);
        }

        ApplyDragForce(kDragForce);
    }

    void ControlSpeedLinesAndFov()
    {
        // cameraForward方向への速度成分を計算
        float forwardSpeed = Vector3.Dot(rb.velocity, cameraForward);

        // Speed Linesの制御
        float filteredForwardSpeed = forwardSpeed > 5 ? forwardSpeed : 0;

        float baseSpeed = 10.0f;
        float baseStartSpeed = 10.0f;
        float baseRateOverTime = 50.0f;
        float newStartSpeed = baseStartSpeed * (filteredForwardSpeed / baseSpeed);
        float newRateOverTime = baseRateOverTime * (filteredForwardSpeed / baseSpeed);

        var main = speedLinesPS.main;
        main.startSpeed = newStartSpeed; // 動的に計算されたStart Speedを設定
        var emission = speedLinesPS.emission;
        emission.rateOverTime = newRateOverTime; // 動的に計算されたRate Over Timeを設定

        // Fovの制御
        float baseFov = 60.0f;
        float maxFov = 85.0f;
        float targetFov = Mathf.Clamp(baseFov + (forwardSpeed > 50 ? 2 * (forwardSpeed - 50) : 0), baseFov, maxFov);

        // Fovを滑らかに変更
        currentFov = Mathf.SmoothDamp(currentFov, targetFov, ref fovVelocity, 0.1f);
        playerCamera.SetFov(currentFov);
    }

    // オブジェクトに接触したときに呼ばれるメソッド
    void OnCollisionEnter(Collision collision)
    {
        // タグで接触したオブジェクトが地面かどうかをチェック
        if (collision.gameObject.tag == "Ground")
        {
            canJump = true; // ジャンプ可能にする
        }
        else if (collision.gameObject.tag == "Boss")
        {
            canJump = true;
        }

        ApplyBoundForce(collision);
    }

    //跳ね返り処理
    public float e; //param
    private float fixedE;
    private readonly float fixedEDeno = 0.959f; //param

    void ApplyBoundForce(Collision collision)
    {
        fixedE = e / fixedEDeno; //param
        if (collision.gameObject.tag == "Ground")
        {
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflect = Vector3.Reflect(previousVelocity, normal);

            // 法線方向の成分
            Vector3 normalComponent = Vector3.Project(reflect, normal);

            // 接面＝法線の垂直方向の成分。接面=tangent.
            Vector3 tangentComponent = Vector3.zero;

            // // 角度が30度以上の場合に垂直方向の成分を追加
            // float angle = Vector3.Angle(normal, Vector3.up);
            // if (angle >= 30.0f)
            // {
            //     float eTangent = 0.9f; //param
            //     tangentComponent = eTangent*(reflect - normalComponent); // 垂直方向の成分を少しだけ追加
            // }

            // 法線方向と垂直方向の成分を足し合わせる
            Vector3 finalComponent = normalComponent + tangentComponent;

            // 反射ベクトルに基づいて力を加える
            rb.AddForce(fixedE * finalComponent * rb.mass, ForceMode.Impulse);
        }
    }

    // 空気抵抗をシミュレート
    void ApplyDragForce(float kDrag = 0.5f) //param
    {
        Vector3 dragForce = -kDrag * rb.velocity; //param
        rb.AddForce(dragForce);
    }

    void OnTriggerEnter(Collider other)
    {
        // タグで接触したオブジェクトがキルボリュームかどうかをチェック
        if (other.gameObject.tag == "KillVolume")
        {
            sc.HandlePlayerDeath(); //プレイヤーのDeath処理
        }
    }

    void ChangeEmissionRate(float rate)
    {
        // パーティクルのエミッションレートを変更。e.g., プレイヤーの燃え具合を制御
        var emission = psFire.emission;

        if (psFire == null)
        {
            Debug.LogError("Emission module not found!");
            return;
        }

        emission.rateOverTime = new ParticleSystem.MinMaxCurve(rate); // rateを設定。直接数値の代入はできないらしい
    }

    public void ExplodeEffect()
    {
        // 爆発エフェクトを再生
        if (psExplode != null)
        {
            psExplode.Play();
            if (!explodeAudio.isPlaying)
            {
                explodeAudio.Play();
            }
        }
        else
        {
            Debug.LogError("BigExplosionEffect ParticleSystem not found!");
        }
    }

    public void SetInputEnabled(bool isEnabled)
    {
        // 入力を無効化
        isKeyEnabled = isEnabled;
    }

    /// <summary>
    /// ゲームオブジェクトのマテリアルを変更する
    /// </summary>
    private void ChangeMaterial(Material newMaterial)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && newMaterial != null)
        {
            renderer.material = newMaterial;
        }
        else
        {
            Debug.LogError("In inspector, renderer or newMaterial is not assigned.");
        }
    }
}
