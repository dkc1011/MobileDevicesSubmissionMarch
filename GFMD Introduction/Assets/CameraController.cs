using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 _iPos;
    private Quaternion _iRot;
    public Vector3 initialPosition
    {
        get => _iPos;
        set => _iPos = value;
    }

    public Quaternion initialRotation
    {
        get => _iRot;
        set => _iRot = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MoveTo(Vector3 destination)
    {
        transform.Translate(destination * -0.005f);
    }

    public void Zoom(float distance, bool zoomingIn)
    {
        if(zoomingIn)
        {
            transform.position += (distance / 20) * transform.forward.normalized * Time.deltaTime;
        }
        else
        {
            transform.position += (distance / 20) * -transform.forward.normalized * Time.deltaTime;
        }
    }

    public void Rotate(float value, bool clockwise)
    {
        transform.Rotate(transform.forward, value);
        if (!clockwise)
        {
            
        }
        else
        {
            transform.Rotate(transform.forward, value);
        }   
    }

    public void DragRotate(float value, bool Vertical)
    {
        if (Vertical)
        {
            transform.RotateAround(transform.position, transform.right, value * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(transform.position, transform.up, value * Time.deltaTime);
        }
    }
    public void ResetCamera()
    {
        transform.position = _iPos;
        transform.rotation = _iRot;
    }

    public void SetInitialState()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

    }
}
