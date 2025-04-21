using UnityEngine;

public class InstructionInteractor : MonoBehaviour
{
    /// <summary>
    /// 指定されたキーが押されたときにオブジェクトの色を変更する
    /// </summary>
    [SerializeField] private KeyCode activationKey = KeyCode.R; // キーをInspectorで設定可能に

    private Renderer targetRenderer;

    void Start()
    {
        // 自身の Renderer を取得し、初期色を白に設定
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            targetRenderer.material.color = Color.white;
        }
    }

    void Update()
    {
        // 指定されたキーが押されたらオブジェクトを赤くする
        if (Input.GetKeyDown(activationKey))
        {
            if (targetRenderer != null)
            {
                targetRenderer.material.color = Color.red;
            }
        }

        // 指定されたキーが離されたらオブジェクトを白に戻す
        if (Input.GetKeyUp(activationKey))
        {
            if (targetRenderer != null)
            {
                targetRenderer.material.color = Color.white;
            }
        }
    }
}