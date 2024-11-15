using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEventArea : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject playerCamera;
    private PlayerCamera playerCameraCtrl;

    [SerializeField] private Transform target;

    [SerializeField] private Transform SpherePlayerTarget;

    void Start()
    {
        playerCameraCtrl = playerCamera.GetComponent<PlayerCamera>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerCameraCtrl.canMove = false;
            playerCameraCtrl.SetViewTarget(target);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerCameraCtrl.canMove = true;
            playerCameraCtrl.SetSpherePlayerAsTarget();
        }
    }
}
