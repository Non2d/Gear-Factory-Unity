using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpherePlayer : MonoBehaviour
{
    Rigidbody rb;

    float force = 100.0f;
    float torque = 10.0f;

    [SerializeField]
    private PlayerCamera playerCamera;
    Vector3 cameraForward;

    // ジャンプ可能かどうかを管理するフラグ
    bool canJump = true;

    [SerializeField]
    private IngameSceneController sc;

    [SerializeField]
    private ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); //this.は省略可能
        rb.maxAngularVelocity = 100.0f;

        ps = GetComponentInChildren<ParticleSystem>();
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

        // テスト：Eキーでパーティクルのエミッションレートを変更
        if (Input.GetKey(KeyCode.E))
        {
            if (!ps.isPlaying) // パーティクルが再生されていないなら
            {
                ps.Play(); // 再生開始
            }
        }
        else
        {
            if (ps.isPlaying) // パーティクルが再生されているなら
            {
                ps.Stop(); // 再生停止
            }
        }
    }

    void FixedUpdate() //物理演算関連はできるだけこちらで処理。フレームレートに依存しない処理を目指す感じかな？
    {
        //WASDで前後左右に力を加える
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(force * cameraForward);
            sc.GivePlayerDamage(1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-force * cameraForward);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector3.Cross(cameraForward, Vector3.up) * force);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(-Vector3.Cross(cameraForward, Vector3.up) * force);
        }

        //Shiftでトルクを加える
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddTorque(-Vector3.Cross(cameraForward, Vector3.up) * torque, ForceMode.Impulse);
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

        if(ps==null)
        {
            Debug.LogError("Emission module not found!");
            return;
        }

        emission.rateOverTime = new ParticleSystem.MinMaxCurve(rate);  // rateを設定。直接数値の代入はできないらしい
    }
}
