using UnityEngine;

public class SlowDownEventArea : MonoBehaviour
{
    [SerializeField] private GameObject gear;
    private GearControl gearCtrl;
    // Start is called before the first frame update
    void Start()
    {
        gearCtrl = gear.GetComponent<GearControl>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gearCtrl.SetGearSpeed(10.0f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gearCtrl.SetGearSpeed(20.0f);
        }
    }
}
