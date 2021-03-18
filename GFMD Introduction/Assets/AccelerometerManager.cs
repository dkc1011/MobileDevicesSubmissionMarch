using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerometerManager : MonoBehaviour
{
    public bool phoneIsFlat = true;
    private Rigidbody cubeRB;

    // Start is called before the first frame update
    void Start()
    {
        cubeRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tilt = Input.acceleration;

        if(phoneIsFlat)
        {
            tilt = Quaternion.Euler(90, 0, 0) * tilt;
        }
        cubeRB.AddForce(tilt);
    }
}
