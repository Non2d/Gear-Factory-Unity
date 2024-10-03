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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); //this.は省略可能
        rb.maxAngularVelocity = 100.0f;

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
    }

    // Update is called once per frame
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
        }

        // Qキーでオプション画面を表示
        if (Input.GetKeyDown(KeyCode.Q))
        {
            sc.Pause();
        }

        // Eキーでパーティクルと音声を制御
        if (Input.GetKey(KeyCode.E))
        {
            if (!ps.isPlaying) // パーティクルが再生されていないなら
            {
                ps.Play(); // 再生開始
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
    }

    void FixedUpdate() //物理演算関連はできるだけこちらで処理。フレームレートに依存しない処理を目指す感じかな？
    {
        Vector3 playerForward = Vector3.zero;

        // WASDキーで方向を決定
        if (Input.GetKey(KeyCode.W))
        {
            playerForward += cameraForward;
            sc.GivePlayerDamage(1);
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
        }

        //Shiftでトルクを加える
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Vector3 torqueDirection = Vector3.Cross(Vector3.up, playerForward);
            rb.AddTorque(torqueDirection.normalized * torque, ForceMode.Impulse);
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
        // パーティクルのエミッションレートを変更
        var emission = ps.emission;

        if (ps == null)
        {
            Debug.LogError("Emission module not found!");
            return;
        }

        emission.rateOverTime = new ParticleSystem.MinMaxCurve(rate); // rateを設定。直接数値の代入はできないらしい
    }
}
