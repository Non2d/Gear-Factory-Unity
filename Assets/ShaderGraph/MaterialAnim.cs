using UnityEngine;

public class MaterialAnim : MonoBehaviour
{
    public Material material;
    public float speed = 1.0f;

    void Update()
    {
        if (material != null)
        {
            material.SetFloat("_Speed", speed);
        }
    }
}