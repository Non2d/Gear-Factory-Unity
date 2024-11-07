using UnityEngine;

public class PlanetLocalGravityController : MonoBehaviour
{
    // public float gravityXZ = 9.81f; // XZ方向の重力
    // public float gravityY = 9.81f;  // Y方向の重力

    // public float kXZ = 9.81f; //XZ方向のバネ係数
    // public float kY = 9.81f; //Y方向のバネ係数

    public float k = 100.0f;

    void Start()
    {
        
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        transform.Rotate(Vector3.up, 0.5f);

        // Find all objects with Rigidbody in the scene
        Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

        // Apply custom gravity for each Rigidbody
        foreach (Rigidbody rb in rigidbodies)
        {
            // Calculate direction from the gravity center to the object
            Vector3 directionToCenter = -(rb.position - transform.position).normalized;

            // Calculate custom gravity based on direction
            float distance = (transform.position - rb.position).magnitude;

            Vector3 SpringForce = k * distance * directionToCenter;
            Vector3 GravityForce = new Vector3(SpringForce.x, 3*SpringForce.y, SpringForce.z);

            rb.AddForce(GravityForce, ForceMode.Force);
        }
    }
}