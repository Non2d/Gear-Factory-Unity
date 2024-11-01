using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LeftRightObstacle : MonoBehaviour
{
    // 振幅（オブジェクトが移動する範囲の半分）
    public float amplitude = 2.0f;

    // 初期位相
    public float initialPhase = 0.0f; //PIをあとで掛ける

    // 周期（オブジェクトが1往復するのにかかる時間）
    public float period = 2.0f;

    // 初期位置を保存するための変数
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        // 初期位置を保存
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // sin関数を使用して左右に往復運動させる
        float x = startPosition.x + amplitude * Mathf.Sin(initialPhase * Mathf.PI + 2 * Mathf.PI * Time.time / period);
        transform.position = new Vector3(x, startPosition.y, startPosition.z);
    }
}