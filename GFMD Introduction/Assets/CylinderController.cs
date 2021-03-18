using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderController : MonoBehaviour, IControllable
{
    private Vector3 _iPos;
    private Quaternion _iRot;
    private Vector3 _iScal;
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

    public void HandleTap()
    {
        ToggleSelected(true);
    }

    public void MoveTo(Touch touch)
    {
        Camera myCamera = Camera.main;
        Vector3 newPos = myCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, _iDist));

        transform.position = newPos;
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

}
