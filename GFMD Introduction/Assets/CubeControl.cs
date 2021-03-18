using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : MonoBehaviour, IControllable
{
    private Vector3 _iPos;
    private Quaternion _iRot;
    private Vector3 _iScal;
    private GameObject[] allPlanesInScene;
    private float _iDist;
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
    public Vector3 initialScale 
    { 
        get => _iScal; 
        set => _iScal = value; 
    }

    public float startDistanceToScreen
    {
        get => _iDist;
        set => _iDist = value;
    }

    void Start()
    {
        allPlanesInScene = GameObject.FindGameObjectsWithTag("Plane");
    }

    public void HandleTap()
    {
        ToggleSelected(true);
    }

    public void MoveTo(Touch touch)
    {
        float cubeRadius = transform.localScale.y / 2;

        Camera myCamera = Camera.main;
        Ray ray = myCamera.ScreenPointToRay(touch.position);
        RaycastHit info;

        GameObject floorPlane = GameObject.Find("FloorPlane");

        Collider planeCollider = floorPlane.GetComponent<Collider>();

        Debug.DrawRay(ray.origin, ray.direction, Color.green, 20f);


        if (planeCollider.Raycast(ray, out info, 100f))
        {
            Vector3 rayPoint = ray.GetPoint(Vector3.Distance(myCamera.transform.position, info.point));
            Vector3 newPos = new Vector3(rayPoint.x, rayPoint.y + cubeRadius, rayPoint.z);

            transform.position = newPos;
        }

    }

    public void ToggleSelected(bool isSelected)
    {
        Renderer objectRenderer = GetComponent<Renderer>();

        if (isSelected)
        {
            objectRenderer.material.SetColor("_Color", Color.green);
        }
        else
        {
            objectRenderer.material.SetColor("_Color", Color.white);
        }
    }

    public void HandleScale(Vector3 value)
    {
        transform.localScale = value;
    }

    public void HandleRotate(float value)
    {
        transform.Rotate(transform.forward, value);
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

    public void ResetObject()
    {
        transform.position = _iPos;
        transform.rotation = _iRot;
        transform.localScale = _iScal;
    }

    public void SetInitialState()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }
}
