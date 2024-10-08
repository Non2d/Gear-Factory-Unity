using UnityEngine;

public class GlobalGravityController : MonoBehaviour
{
    void Start()
    {
        // Find all objects with Rigidbody in the scene
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

        // Disable gravity for each Rigidbody
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.useGravity = false;
        }
    }
}