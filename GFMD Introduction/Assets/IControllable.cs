using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IControllable
{
    Vector3 initialPosition
    {
        get;
        set;
    }

    Quaternion initialRotation
    {
        get;
        set;
    }

    Vector3 initialScale
    {
        get;
        set;
    }

    float startDistanceToScreen
    {
        get;
        set;
    }

    void ToggleSelected(bool isSelected);
    void HandleTap();

    void HandleScale(Vector3 value);

    void HandleRotate(float value);

    void MoveTo(Touch touch);

    void DragRotate(float value, bool Vertical);

    void SetInitialState();

    void ResetObject();
}
