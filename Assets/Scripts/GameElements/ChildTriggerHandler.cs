using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTriggerHandler : MonoBehaviour
{
    private GearControl parentScript;
    private MeshRenderer meshRenderer;

    void Start()
    {
        Debug.Log("ChildTriggerHandler: Start()");

        // 親オブジェクトのスクリプトを取得
        parentScript = GetComponentInParent<GearControl>();

        // MeshRendererコンポーネントを取得
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer not found!");
        }

        // Playモードではメッシュを非表示
        if (Application.isPlaying)
        {
            // Debug.Log("Running in Play mode");
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
                Debug.Log("MeshRenderer disabled");
            }
        }
        else
        {
            // Debug.Log("Running in Edit mode");
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
                Debug.Log("MeshRenderer enabled");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("ChildTriggerHandler: OnTriggerEnter()");
        if (other.CompareTag("Player"))
        {
            // 親の関数を呼び出す
            parentScript.OnPlayerEnterChildTrigger(other);
        }
    }
}