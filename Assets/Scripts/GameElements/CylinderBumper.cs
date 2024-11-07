using System.Collections.Generic;
using UnityEngine;

public class CylinderBumper : MonoBehaviour
{
    public float bumperForce = 10.0f;
    private Vector3 previousVelocity;
    private float critRate = 1.0f;

    void Update()
    {
        transform.Rotate(Vector3.up, 50.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトの情報を取得
        GameObject other = collision.gameObject;

        // 衝突したオブジェクトのタグがPlayerかどうかを判定
        if (other.CompareTag("Player"))
        {
            // Playerタグを持つオブジェクトとの衝突時の処理をここに追加
            // Debug.Log("Playerと衝突しました: " + other.name);

            // 衝突したオブジェクトのRigidbodyを取得
            Rigidbody rb = other.GetComponent<Rigidbody>();

            // 自身のワールド座標を取得
            Vector3 bumperWorldPosition = transform.position;
            // Debug.Log("Bumperのワールド座標: " + bumperWorldPosition);

            // 衝突したオブジェクトのワールド座標を取得
            Vector3 otherWorldPosition = other.transform.position;
            // Debug.Log("Playerのワールド座標: " + otherWorldPosition);

            // バンパーからプレイヤーへの方向ベクトルを計算
            Vector3 direction = otherWorldPosition - bumperWorldPosition;

            Debug.Log(Mathf.Abs(Vector3.Dot(-direction, previousVelocity) / (direction.magnitude * previousVelocity.magnitude)));
            Debug.Log(Mathf.Abs(Vector3.Dot(-direction, previousVelocity) / (direction.magnitude * previousVelocity.magnitude))<0.5f);

            // 会心判定
            if (Mathf.Abs(Vector3.Dot(-direction, previousVelocity) / (direction.magnitude * previousVelocity.magnitude))<0.5f)
            {
                Debug.Log("CRITICAL HIT!");
                critRate = 2.0f;
            }

            //水平方向に弾く
            direction.y = 0; // 水平方向に限定
            direction = direction.normalized;
            rb.AddForce(critRate*bumperForce*direction, ForceMode.Impulse);

            //直前のフレームの速度
            previousVelocity = rb.velocity;
        }
    }
}