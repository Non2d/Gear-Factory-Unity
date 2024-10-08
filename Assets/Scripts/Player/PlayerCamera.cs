using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private GameObject player;
    private Vector3 offset;
    public float mouseSensitivity = 100.0f;
    private float pitch = 30.0f;
    private float yaw = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.Find("SpherePlayer");
        if (this.player == null)
        {
            Debug.LogError("Player object not found!");
            return;
        }
        offset = transform.position - this.player.transform.position;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
    }

    // Update is called once per frame
    void Update()
    {
        if (this.player != null)
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Update yaw and pitch
            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -90f, 90f); // Limit pitch to avoid flipping

            // Calculate new camera position
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            Vector3 newPosition = this.player.transform.position + rotation * offset*2;

            // Update camera position and rotation
            transform.position = newPosition;
            transform.LookAt(this.player.transform.position);
        }
    }

    // Draw camera direction arrow in the scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 200);
        if (this.player != null)
        {
            //Playmode中のみ実行
        }
    }

    public Vector3 GetForwardDirection()
    {
        return transform.forward;
    }
}