using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GearControl : MonoBehaviour
{
    public float gearSpeed = 20.0f;

    private List<TextMeshPro> textComponents = new List<TextMeshPro>();

    private int divisions; //param
    private float originAngle; //param

    private IngameSceneController sc ;

    // Start is called before the first frame update
    void Start()
    {
        sc = FindObjectOfType<IngameSceneController>();
        transform.Rotate(Vector3.forward, 120.0f);

        divisions = 18;
        originAngle = 5.0f;

        Vector3 parentScale = transform.localScale;
        float radius = 10.0f; //param
        float height = 5.0f; //param

        for (int i = 1; i <= divisions; i++)
        {
            float angleDegree = 20.0f * (i - 1) + originAngle;
            float angle = Mathf.Deg2Rad * angleDegree;

            GameObject textObj = new GameObject($"TextMeshPro_{i}");
            textObj.transform.SetParent(transform);

            TextMeshPro text = textObj.AddComponent<TextMeshPro>();
            text.text = i.ToString();
            text.fontSize = 20;
            text.alignment = TextAlignmentOptions.Center;

            float xPosition = radius * Mathf.Cos(angle);
            float yPosition = radius * Mathf.Sin(angle);
            textObj.transform.localPosition = new Vector3(xPosition / parentScale.x, yPosition / parentScale.y, height / parentScale.z);
            textObj.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f) * Quaternion.AngleAxis(angleDegree - 90.0f, Vector3.up) * Quaternion.Euler(10.0f, 0.0f, 0.0f);

            textComponents.Add(text);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, gearSpeed * Time.deltaTime);

        // Debug.Log(GetLocalDirectionToPlayer());
    }

    float GetLocalDirectionToPlayer(GameObject player)
    {
        // 現在のオブジェクトの位置を基準とする
        Vector3 currentPosition = transform.position;

        // test_playerの位置を取得
        Vector3 playerPosition = player.transform.position;

        // 方向ベクトルを計算
        Vector3 direction = playerPosition - currentPosition;

        // 方向ベクトルを正規化
        Vector3 normalizedDirection = direction.normalized;

        // ローカル座標系に変換
        Vector3 localDirection = transform.InverseTransformDirection(normalizedDirection);

        // 角度を計算(-180~180)
        float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }

    /// <summary>
    /// プレイヤーがどのポケットに入ったかを計算する
    /// </summary>
    /// <param name="localDirection">ギアオブジェクトから見たプレイヤーの角度</param>
    /// <returns></returns>
    int GetPocketLanded(float localDirection)
    {
        // -10から始まる範囲にシフト
        float shiftedDirection = localDirection + 10;

        // divisionsの範囲でポケット番号を計算
        int pocketNumber = Mathf.FloorToInt(shiftedDirection / 20) + 1;

        return pocketNumber;
    }

    /// <summary>
    /// ポケット入賞時の処理。プレイヤーがChildTriggerに入ったときに発火する。
    /// </summary>
    /// <param name="other"></param>
    public void OnPlayerEnterChildTrigger(Collider other) //TagがPlayerのとき、ChildTriggerHandlerから呼び出される
    {
        Debug.Log(GetLocalDirectionToPlayer(other.gameObject));
        Debug.Log(GetPocketLanded(GetLocalDirectionToPlayer(other.gameObject)));

        int pocketNumber = GetPocketLanded(GetLocalDirectionToPlayer(other.gameObject));

        if (new List<int> { 2,4,6,8,10,12,14,16,18 }.Contains(pocketNumber))
        {
            Debug.Log("Bonus!");
        }
        else
        {
            // Set player's energy to 0
            sc.HandlePlayerDeath();
        }
    }
}