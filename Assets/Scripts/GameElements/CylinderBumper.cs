using System.Collections.Generic;
using UnityEngine;

public class CylinderBumper : MonoBehaviour
{
    public float springForce = 1000f; // バネの力
    public float springLength = 0.2f; // バネの自然長

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトの情報を取得
        GameObject other = collision.gameObject;

        // 衝突したオブジェクトのタグがPlayerかどうかを判定
        if (other.CompareTag("Player"))
        {
            // Playerタグを持つオブジェクトとの衝突時の処理をここに追加
            Debug.Log("Playerと衝突しました: " + other.name);

            // 衝突したオブジェクトのRigidbodyを取得
            Rigidbody rb = other.GetComponent<Rigidbody>();

            // 自身のワールド座標を取得
            Vector3 bumperWorldPosition = transform.position;
            Debug.Log("Bumperのワールド座標: " + bumperWorldPosition);

            // 衝突したオブジェクトのワールド座標を取得
            Vector3 otherWorldPosition = other.transform.position;
            Debug.Log("Playerのワールド座標: " + otherWorldPosition);

            // バネの自然長を超えた場合に内側に力を適用
            float distance = Vector3.Distance(bumperWorldPosition, otherWorldPosition);
            if (distance > springLength)
            {
                Vector3 direction = (bumperWorldPosition - otherWorldPosition).normalized;
                float forceMagnitude = springForce * (distance - springLength);
                Vector3 springForceVector = direction * forceMagnitude;
                rb.AddForce(springForceVector, ForceMode.Acceleration);
            }
        }
    }
}