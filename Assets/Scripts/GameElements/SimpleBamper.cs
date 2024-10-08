using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBamper : MonoBehaviour
{
    float bamperSpeed = 500.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update(){
        transform.Rotate(Vector3.up, bamperSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {

    }
}