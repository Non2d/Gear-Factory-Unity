using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private ParticleSystem ps;

    [SerializeField]
    private AudioSource fireAudio;

    [SerializeField]
    private AudioClip fireClip;

    [SerializeField]
    private GameObject SpeedLines;
    private ParticleSystem speedLinesPS;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); //this.は省略可能
        rb.maxAngularVelocity = 100.0f;

        speedLinesPS = SpeedLines.GetComponent<ParticleSystem>();

        if (ps == null) // インスペクタで保存し忘れているときのための保険
        {
            ps = GetComponentInChildren<ParticleSystem>();
        }

        if (fireAudio != null)
        {
            fireAudio.loop = true;
            fireAudio.clip = fireClip;
            fireAudio.mute = false;
            fireAudio.volume = 0.1f; //param
        }

        // SpeedLinesのParticle Systemを取得
        speedLinesPS = SpeedLines.GetComponent<ParticleSystem>();

    }

    private Vector3 previousVelocity = Vector3.zero;
    void Update()
    {
        //Playerカメラから進行方向を取得
        if (playerCamera != null)
        {
            cameraForward = playerCamera.GetForwardDirection();
        }

        //Spaceでジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
            canJump = false; // ジャンプ後にフラグをリセット
            sc.GivePlayerDamage(30.0f); //param
        }

        // Qキーでオプション画面を表示
        if (Input.GetKeyDown(KeyCode.Q))
        {
            sc.Pause();
        }

        //
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (!ps.isPlaying) // パーティクルが再生されていないなら
            {
                ps.Play(); // 再生開始
            }
            if (!fireAudio.isPlaying) // 音声が再生されていないなら
            {
                fireAudio.Play(); // 音声再生開始
            }
            fireAudio.volume = 0.1f; // 音量を最大に設定 param
            isFadingOut = false; // フェードアウトを停止
        }
        else
        {
            if (ps.isPlaying) // パーティクルが再生されているなら
            {
                ps.Stop(); // 再生停止
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
        ControlSpeedLines();

        LastUpdate();
    }

    void LastUpdate()
    {
        previousVelocity = rb.velocity;
    }

    void ControlSpeedLines()
    {
        // cameraForward方向への速度成分を計算
        float forwardSpeed = Vector3.Dot(rb.velocity, cameraForward);

        // 速度が5以下の場合は0に設定
        float filteredForwardSpeed = forwardSpeed > 5 ? forwardSpeed : 0;

        // 基準となる速度と割合を設定
        float baseSpeed = 10.0f; // 基準となる速度
        float baseStartSpeed = 10.0f; // 基準となるStart Speed
        float baseRateOverTime = 50.0f; // 基準となるRate Over Time

        // 現在の速度に応じてStart SpeedとRate Over Timeを計算
        float newStartSpeed = baseStartSpeed * (filteredForwardSpeed / baseSpeed);
        float newRateOverTime = baseRateOverTime * (filteredForwardSpeed / baseSpeed);

        // ParticleSystemのStart SpeedとEmissionのRate Over Timeを設定
        var main = speedLinesPS.main;
        main.startSpeed = newStartSpeed; // 動的に計算されたStart Speedを設定

        var emission = speedLinesPS.emission;
        emission.rateOverTime = newRateOverTime; // 動的に計算されたRate Over Timeを設定
    }


    private Vector3 playerForward;

    void FixedUpdate() //物理演算関連はできるだけこちらで処理。フレームレートに依存しない処理を目指す感じかな？
    {
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
        if (playerForward != Vector3.zero)
        {
            rb.AddForce(playerForward.normalized * force);
            sc.GivePlayerDamage(0.5f);
        }

        //Shiftでトルクを加える
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 torqueDirection = Vector3.Cross(Vector3.up, playerForward);
            rb.AddTorque(torqueDirection.normalized * torque, ForceMode.Impulse);
            sc.GivePlayerDamage(1.0f); //param
        }


    }

    // オブジェクトに接触したときに呼ばれるメソッド
    void OnCollisionEnter(Collision collision)
    {
        // タグで接触したオブジェクトが地面かどうかをチェック
        if (collision.gameObject.tag == "Ground")
        {
            canJump = true; // ジャンプ可能にする
        }

        Bound(collision);
    }

    //跳ね返り処理
    public float e; //param
    private float fixedE;
    private readonly float fixedEDeno = 0.959f; //param

    void Bound(Collision collision)
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
        var emission = ps.emission;

        if (ps == null)
        {
            Debug.LogError("Emission module not found!");
            return;
        }

        emission.rateOverTime = new ParticleSystem.MinMaxCurve(rate); // rateを設定。直接数値の代入はできないらしい
    }
}
