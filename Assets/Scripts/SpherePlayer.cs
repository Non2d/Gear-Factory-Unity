using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePlayer : MonoBehaviour
{
    Rigidbody rb;

    float force = 100.0f;
    float torque = 10.0f;
    PlayerCamera playerCamera;
    Vector3 cameraForward;

    // ジャンプ可能かどうかを管理するフラグ
    bool canJump = true;

    // Start is called before the first frame update
    void Start()
    {
        GameObject cameraObject = GameObject.Find("PlayerCamera"); 
        if (cameraObject != null)
        {
            playerCamera = cameraObject.GetComponent<PlayerCamera>();
        }
        rb = this.GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Platyerカメラから進行方向を取得
        if (playerCamera != null)
        {
            cameraForward = playerCamera.GetForwardDirection();
        }

        

        //Spaceでジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && this.canJump)
        {
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
            this.canJump = false; // ジャンプ後にフラグをリセット
        }

    }

    void FixedUpdate() //物理演算関連はできるだけこちらで処理。フレームレートに依存しない処理を目指す感じかな？
    {
        //WASDで前後左右に力を加える
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(force * cameraForward);
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
        // 接触したオブジェクトが地面かどうかをチェック（タグを使用）
        this.canJump = true; // ジャンプ可能にする
    }
}
