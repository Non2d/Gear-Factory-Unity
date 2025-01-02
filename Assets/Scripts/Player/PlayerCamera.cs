using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private SO_GearFactory gf;
    private GameObject player;
    private GameObject target;
    private Vector3 offset;
    private float pitch = -10.0f; //param
    private float yaw;
    private float roll;
    private float mouseSensitivity;
    private float zoomSpeed; // マウスホイールのズーム速度
    public float minZoom; // 最小ズーム距離
    public float maxZoom; // 最大ズーム距離
    public float defaultZoom; // デフォルトズーム距離
    public bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        zoomSpeed = gf.zoomSpeed;
        mouseSensitivity = gf.mouseSensitivity;
        player = GameObject.Find("SpherePlayer");
        target = player;
        if (player == null)
        {
            Debug.LogError("Player object not found!");
            return;
        }
        offset = transform.position - target.transform.position;

        offset = offset.normalized * defaultZoom; // デフォルトのズーム距離を15に設定
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen

        yaw = target.transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (canMove)
            {
                // Get mouse input
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                // Update yaw and pitch
                yaw += mouseX;
                pitch -= mouseY;
                pitch = Mathf.Clamp(pitch, -60f, 60f); // param。普通のステージでは-25~60, 宇宙ステージでは

                // Get mouse scroll input for zoom
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                float distance = offset.magnitude;
                distance -= scroll * zoomSpeed;
                distance = Mathf.Clamp(distance, minZoom, maxZoom);
                offset = offset.normalized * distance;

                // Calculate new camera position
                Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
                Vector3 newPosition = target.transform.position + rotation * offset;

                // Update camera position and rotation
                transform.position = newPosition;
                transform.LookAt(target.transform.position);
            }

        }
    }

    // Draw camera direction arrow in the scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 200);
        if (target != null)
        {
            //Playmode中のみ実行
        }
    }

    public Vector3 GetForwardDirection()
    {
        return transform.forward;
    }

    public float GetFov()
    {
        return Camera.main.fieldOfView;
    }

    public void SetFov(float fov)
    {
        Camera.main.fieldOfView = fov;
    }

    /// <summary>
    /// カメラのターゲットを動的に変更するメソッド
    /// </summary>
    /// <param name="newTarget">新しいターゲットとなるTransform</param>
    public void SetViewTarget(Transform newTarget)
    {
        transform.position = newTarget.position;
        transform.rotation = newTarget.rotation;
    }

    /// <summary>
    /// ターゲットをSpherePlayerに戻す
    /// </summary>
    public void SetSpherePlayerAsTarget()
    {
        target = player;
    }
}
