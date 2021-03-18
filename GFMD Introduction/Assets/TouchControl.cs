using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    Camera myCamera;
    float touchTime = 0;
    IControllable selectedObject;
    private bool touchMoved;
    private List<IControllable> controllableObjects;
    private CameraController selectedCameraController;
    private float startingDistance, startingAngle, distanceThreshold = 150f, angleThreshold = 0.2f;
    private Vector2 startingPosition;
    private Rigidbody sphereRB;
    enum Gesture {  Tap, Drag, Rotate, Zoom, Determining, None }
    Gesture currentGesture;

    void Start()
    {
        //Assign class properties
        currentGesture = Gesture.None;
        myCamera = Camera.main;
        selectedCameraController = myCamera.GetComponent<CameraController>();

        //Assign the cube rigidbody to a variable
        sphereRB = FindObjectOfType<Rigidbody>();
        
        //Create a list of all objects in the scene, then list IControllable objects in the scene for "Select Object" capabilities
        var sceneObjects = FindObjectsOfType(typeof(MonoBehaviour));
        controllableObjects = InitializeIControllableList(sceneObjects);

        //Assign initial Position, Rotate and Scale to each IControllable Object / Position and Rotation to Camera;
        SetDefaultObjectAndCameraPositions();

    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch firstTouch = Input.touches[0];

            if(currentGesture == Gesture.None)
            {
                currentGesture = Gesture.Determining;
            }

            switch(currentGesture)
            {
                case Gesture.Determining:
                    DetermineGesture(Input.touches);
                    break;
                case Gesture.Tap:
                    ManageTap(firstTouch);
                    break;
                case Gesture.Drag:
                    if(Input.touchCount == 1)
                    {
                        ManageDrag(firstTouch);
                    }
                    else
                    {   
                        if(Input.touchCount > 1)
                        {
                            ManageDrag(firstTouch, Input.touches[1]);
                        }     
                    }
                    break;
                case Gesture.Rotate:
                    ManageRotate(Input.touches);
                    break;
                case Gesture.Zoom:
                    ManageZoom(Input.touches);
                    break;

            }

            if(currentGesture != Gesture.Determining)
            {
                sphereRB.Sleep();
            }
            else
            {
                sphereRB.WakeUp();
            }
           
        }
    }

    /*
    +------------------------------------------------------+ 

                     Initialization

    +------------------------------------------------------+
    */

    private List<IControllable> InitializeIControllableList(object[] objectsInScene)
    {
        List<IControllable> IControllableList = new List<IControllable>();

        foreach (MonoBehaviour gObject in objectsInScene)
        {
            if (gObject is IControllable)
            {
                IControllableList.Add(gObject.GetComponent<IControllable>());
            }
        }

        return IControllableList;
    }

    private void SetDefaultObjectAndCameraPositions()
    {
        foreach (IControllable obj in controllableObjects)
        {
            obj.SetInitialState();
        }

        selectedCameraController.SetInitialState();
    }

    /*
    +------------------------------------------------------+ 
    
                     Gesture = Determining

    +------------------------------------------------------+
    */

    private void DetermineGesture(Touch[] touches)
    {
        Touch firstTouch = touches[0];

        if(Input.touchCount == 1)
        {
            DetermineGesturePhases(firstTouch);
        }
        else if(Input.touchCount == 2)
        {
            Touch secondTouch = touches[1];

            DetermineGesturePhases(firstTouch, secondTouch);
        }
        else
        {
            currentGesture = Gesture.None;
        }

    }

    //Overload - If given one touch as an argument, the Gesture is considered a one-touch gesture
    private void DetermineGesturePhases(Touch firstTouch)
    {
        switch (firstTouch.phase)
        {
            case TouchPhase.Began:
                touchTime = Time.time;
                touchMoved = false;
                break;
            case TouchPhase.Moved:
                currentGesture = Gesture.Drag;
                touchMoved = true;
                break;
            case TouchPhase.Ended:
                if (IsTap(touchTime))
                {
                    currentGesture = Gesture.Tap;
                }

                break;

        }

        
    }

    //Overload - If given two touches as arguments, the Gesture is considered a two-touch gesture
    private void DetermineGesturePhases(Touch firstTouch, Touch secondTouch)
    {
        switch (secondTouch.phase)
        {
            case TouchPhase.Began:
                touchTime = Time.time;
                touchMoved = false;
                startingDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
                startingAngle = Mathf.Atan2(secondTouch.position.y - firstTouch.position.y, secondTouch.position.x - firstTouch.position.x);
                startingPosition = secondTouch.position;
                break;
            case TouchPhase.Moved:

                if (IsZoom(firstTouch, secondTouch))
                {
                    currentGesture = Gesture.Zoom;
                }
                else if (IsRotation(firstTouch, secondTouch))
                {
                    currentGesture = Gesture.Rotate;
                }
                else
                {
                    currentGesture = Gesture.Drag;
                }
                
                touchMoved = true;
                break;
            case TouchPhase.Ended:
                currentGesture = Gesture.None;
                break;

        }

        
    }

    /*
    +------------------------------------------------------+ 

                     Gesture = Tap

    +------------------------------------------------------+
    */

    private Ray ShootRayFromScreen(Vector3 TouchPos)
    {
        Ray ray = myCamera.ScreenPointToRay(TouchPos);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        return ray;

    }

    private bool IsTap(float timeStarted)
    {
        float tapThreshold = 0.2f;
        float timeDifference = Time.time - timeStarted;

        if (timeDifference < tapThreshold && !touchMoved)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private void ManageTap(Touch firstTouch)
    {
        Ray touchRay = ShootRayFromScreen(firstTouch.position);
        RaycastHit touchInfo;

        if (Physics.Raycast(touchRay, out touchInfo))
        {
            IControllable object_tapped = touchInfo.transform.GetComponent<IControllable>();

            if (object_tapped != null)
            {
                object_tapped.HandleTap();
                selectedObject = object_tapped;
                object_tapped.startDistanceToScreen = Vector3.Distance(myCamera.transform.position, touchInfo.transform.position);

                foreach (IControllable obj in controllableObjects)
                {
                    if (obj != selectedObject)
                    {
                        obj.ToggleSelected(false);
                    }
                }
            }
            else
            {
                selectedObject = null;
                foreach (IControllable obj in controllableObjects)
                {
                    obj.ToggleSelected(false);
                }
            }
        }
        

        currentGesture = Gesture.None;
    }

    /*
    +------------------------------------------------------+ 

                     Gesture = Drag

    +------------------------------------------------------+
    */
    private void ManageDrag(Touch firstTouch)
    {
        if (selectedObject != null)
        {
            selectedObject.MoveTo(firstTouch);
        }
        else
        {
            selectedCameraController.MoveTo(firstTouch.deltaPosition);
        }

        currentGesture = Gesture.None;
    }

    private bool DragIsVertical(Touch secondTouch)
    {
        float horizontalDifference = Mathf.Abs(secondTouch.position.x - startingPosition.x);
        float verticalDifference = Mathf.Abs(secondTouch.position.y - startingPosition.y);

        if(verticalDifference > horizontalDifference)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool DragIsPositive(Touch secondTouch, bool isVertical)
    {
        if(isVertical)
        {
            if (secondTouch.position.x > startingPosition.x)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if(secondTouch.position.y > startingPosition.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    //If it is a two-touch drag, it is handled by this overload
    private void ManageDrag(Touch firstTouch, Touch secondTouch)
    {
        float rotateDistance = Vector2.Distance(secondTouch.position, startingPosition);

        if (selectedObject != null)
        {
            if(DragIsVertical(secondTouch))
            {
                if(DragIsPositive(secondTouch, true))
                {
                    selectedObject.DragRotate(rotateDistance, true);
                }
                else
                {
                    selectedObject.DragRotate(-rotateDistance, true);
                }
                
            }
            else
            {
                if (DragIsPositive(secondTouch, true))
                {
                    selectedObject.DragRotate(rotateDistance, false);
                }
                else
                {
                    selectedObject.DragRotate(-rotateDistance, false);
                }
            }
        }
        else
        {
            if (DragIsVertical(secondTouch))
            {
                if (DragIsPositive(secondTouch, true))
                {
                    selectedCameraController.DragRotate(rotateDistance, true);
                }
                else
                {
                    selectedCameraController.DragRotate(-rotateDistance, true);
                }

            }
            else
            {
                if (DragIsPositive(secondTouch, true))
                {
                    selectedCameraController.DragRotate(rotateDistance, false);
                }
                else
                {
                    selectedCameraController.DragRotate(-rotateDistance, false);
                }
            }
        }

        currentGesture = Gesture.None;
    }

    /*
    +------------------------------------------------------+ 

                     Gesture = Zoom

    +------------------------------------------------------+
    */

    private bool IsZoom(Touch firstTouch, Touch secondTouch)
    {
        return Mathf.Abs(Vector2.Distance(firstTouch.position, secondTouch.position) - startingDistance) > distanceThreshold;
    }

    private void ManageZoom(Touch[] touches)
    {
        if(touches.Length > 1)
        {
            Touch firstTouch = touches[0];
            Touch secondTouch = touches[1];

            if (selectedObject == null)
            {
                ZoomCamera(firstTouch, secondTouch);
            }
            else
            {
                Vector3 scaleFactor = CalculateScaleFactor(firstTouch, secondTouch);

                if (IsZoomingIn(firstTouch, secondTouch))
                {
                    selectedObject.HandleScale(scaleFactor);
                }
                else
                {
                    selectedObject.HandleScale(-scaleFactor);
                }
            }
        }


        currentGesture = Gesture.None;
    }

    private Vector3 CalculateScaleFactor(Touch firstTouch, Touch secondTouch)
    {
        float distanceZoomed1 = (Vector3.Distance(firstTouch.position, firstTouch.deltaPosition));
        float distanceZoomed2 = (Vector3.Distance(secondTouch.position, secondTouch.deltaPosition));

        if (distanceZoomed1 > distanceZoomed2)
        {
            return (distanceZoomed1 / distanceZoomed2) * Vector3.one;
        }
        else
        {
            return (distanceZoomed2 / distanceZoomed1) * Vector3.one;
        }
    }

    private void ZoomCamera(Touch firstTouch, Touch secondTouch)
    {
        if (IsZoomingIn(firstTouch, secondTouch))
        {
            selectedCameraController.Zoom(Vector3.Distance(firstTouch.position, secondTouch.position), true);
        }
        else
        {
            selectedCameraController.Zoom(Vector3.Distance(firstTouch.position, secondTouch.position), false);
        }
    }

    private bool IsZoomingIn(Touch firstTouch, Touch secondTouch)
    {
        return Mathf.Abs(Vector2.Distance(firstTouch.position, secondTouch.position)) > startingDistance;
    }

    /*
    +------------------------------------------------------+ 

                     Gesture = Rotation

    +------------------------------------------------------+
    */

    private bool IsRotation(Touch firstTouch, Touch secondTouch)
    {
        return Mathf.Abs(Mathf.Atan2(secondTouch.position.y - firstTouch.position.y, secondTouch.position.x - firstTouch.position.x) - startingAngle) > angleThreshold; 
    }

    private void ManageRotate(Touch[] touches)
    {
        if(touches.Length > 1)
        {
            Touch firstTouch = touches[0];
            Touch secondTouch = touches[1];

            float angle = Mathf.Atan2(secondTouch.position.y - firstTouch.position.y, secondTouch.position.x - firstTouch.position.x);

            if(selectedObject == null)
            {
                selectedCameraController.Rotate(angle, IsClockwise(angle));
            }
            else
            {
                selectedObject.HandleRotate(angle);
            }
        }

        currentGesture = Gesture.None;
    }

    private bool IsClockwise(float angle)
    {
        return angle > startingAngle;
    }

    /*
    +------------------------------------------------------+ 

                     Reset Button Pressed

    +------------------------------------------------------+
    */

    public void ResetAllControllableObjects()
    {
        foreach(IControllable obj in controllableObjects)
        {
            obj.ResetObject();
            obj.ToggleSelected(false);
        }

        selectedCameraController.ResetCamera();
        selectedObject = null;
    }
}
