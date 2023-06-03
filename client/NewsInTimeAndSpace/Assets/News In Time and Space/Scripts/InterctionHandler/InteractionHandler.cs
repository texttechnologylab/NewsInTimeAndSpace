using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;

/// <summary>
/// InteractionHanlder
/// 
/// This class is responsible for registering interactions with the map or UI.
/// It is meant to be attatched to the root player gameobject.
/// </summary>
public class InteractionHandler : MonoBehaviour
{
    static float globeRadius = 150.0f;

    public Camera headCamera;
    public Transform scaleTarget;
    public HandRayCaster leftRaycaster;
    public HandRayCaster rightRaycaster;
    public LayerMask visualizerLayer;
    public LayerMask clickableUILayer;
    public LayerMask raycastBlocker;
    Transform scaleHandler;

    HandController[] handControllers;
    public float minDistance = 160.0f;
    public float maxDistance = 400.0f;

    // Globe interaction variables:
    bool rightMapInteraction;
    bool leftMapInteraction;
    Vector3 rightPosition;
    Vector3 leftPosition;
    float controllerDistance;
    float controllerAngle;
    InteractionType interaction;

    // Old = value from last frame, new = value from current frame
    Vector3 oldPosition;
    Vector3 newPosition;
    float oldDistance;
    float newDistance;
    float oldControllerAngle;
    float newControllerAngle;

    // Event & UI interaction variables:
    bool rightRaycastInteraction;
    bool leftRaycastInteraction;
    Transform rightInteractionTarget;
    Transform leftInteractionTarget;

    bool oldRaycastInteraction;
    Transform raycastinteractionTarget;

    private enum InteractionTargetType
    {
        None,
        UI,
        Visualizer
    }

    private void Awake()
    {
        interaction = InteractionType.None;
        handControllers = GetComponentsInChildren<HandController>();
        scaleHandler = new GameObject("PlayerScaleHandler").transform;

        // Debug
        transform.parent = scaleHandler;
        // debug end
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Update viewHandler.filterPool

        updateObjectInteraction();

        updateGlobeInteraction();
    }

    /// <summary>
    /// Handles player interaction with selecting events, groups and UI elements.
    /// </summary>
    void updateObjectInteraction()
    {
        updateObjectInput();

        bool raycastInteraction = leftRaycastInteraction || rightRaycastInteraction;
        //bool newRaycastInteraction = raycastInteraction && raycastinteractionTarget == null;
        Transform newTarget = null;

        if (raycastInteraction && !oldRaycastInteraction)
        {
            if (rightRaycastInteraction)
            {
                newTarget = rightInteractionTarget;
            }
            else if (leftRaycastInteraction)
            {
                newTarget = leftInteractionTarget;
            }

            if (newTarget != raycastinteractionTarget)
            {
                // Determine interaction target type
                InteractionTargetType newTargetType = InteractionTargetType.None;
                if (newTarget != null)
                {
                    if (((1 << newTarget.gameObject.layer) & visualizerLayer.value) != 0)
                    {
                        newTargetType = InteractionTargetType.Visualizer;
                    }
                    else if (((1 << newTarget.gameObject.layer) & clickableUILayer.value) != 0)
                    {
                        newTargetType = InteractionTargetType.UI;
                    }
                }

                // Perform deselect on last selected visualizer if not interacting with UI
                if (raycastinteractionTarget != null
                    && newTargetType != InteractionTargetType.UI
                    && ((1 << raycastinteractionTarget.gameObject.layer) & visualizerLayer.value) != 0)
                {
                    raycastinteractionTarget.GetComponent<Visualizer>().onRaycastDeselect();
                    raycastinteractionTarget = null;
                }

                // Start new raycast interaction
                switch (newTargetType)
                {
                    case InteractionTargetType.Visualizer:
                        newTarget.GetComponent<Visualizer>().onRaycastSelect();
                        raycastinteractionTarget = newTarget;
                        break;
                    case InteractionTargetType.UI:
                        newTarget.GetComponent<UIButton>().onRaycastInteraction();
                        break;
                    default:
                        raycastinteractionTarget = null;
                        break;
                }
            }
        }

        // TODO: what happens on aggregation level change? raycastInteractionTarget might be inactive

        oldRaycastInteraction = raycastInteraction;
    }

    void updateObjectInput()
    {
        foreach (HandController item in handControllers)
        {
            if (item.Right)
            {
                rightRaycastInteraction = item.PrimaryButtonState;
                rightInteractionTarget = rightRaycaster.RayHit.transform;
            }
            else if (item.Left)
            {
                leftRaycastInteraction = item.PrimaryButtonState;
                leftInteractionTarget = leftRaycaster.RayHit.transform;
            }
        }

        // Desktop compatibility
        if (Input.GetMouseButton(0))
        {
            RaycastHit rayHit;
            Physics.Raycast(headCamera.ScreenPointToRay(Input.mousePosition), out rayHit, 1000f, clickableUILayer | visualizerLayer);

            rightRaycastInteraction = true;
            rightInteractionTarget = rayHit.transform;
        }
    }

    /// <summary>
    /// Handles Player interaction with moving, zooming and rotating around the globe.
    /// 
    /// Grip & move 1 controller: move globe (rotates player around center)
    /// Grip & move 2 controllers in same direction: move globe
    /// Grip & move 2 controllers apart or together: zoom globe
    /// Grip & move 2 controllers (counter-)clockwise: rotate globe (rotate player around axis from center to camera)
    /// </summary>
    void updateGlobeInteraction()
    {
        updateGlobeControlInput();

        // Start new interaction or update new variables
        if (rightMapInteraction && leftMapInteraction)
        {
            if (interaction != InteractionType.Zoom)
            {
                // Start new Zoom interaction
                interaction = InteractionType.Zoom;
                //oldDistance = Vector3.Distance(rightPosition, leftPosition);
                oldDistance = controllerDistance;
                newDistance = oldDistance;
                oldPosition = Vector3.Lerp(rightPosition, leftPosition, 0.5f);
                newPosition = oldPosition;
                //oldAngle = Vector3.SignedAngle(transform.up, rightPosition - leftPosition, transform.position);
                oldControllerAngle = controllerAngle;
                newControllerAngle = oldControllerAngle;
            }
            else
            {
                // Update current interaction variable
                newDistance = controllerDistance;
                newPosition = Vector3.Lerp(rightPosition, leftPosition, 0.5f);
                newControllerAngle = Vector3.SignedAngle(transform.up, rightPosition - leftPosition, transform.position);
            }
        }
        else if (rightMapInteraction)
        {
            if (interaction != InteractionType.MoveRight)
            {
                // Start new move interaction with right controller
                interaction = InteractionType.MoveRight;
                oldPosition = rightPosition;
                newPosition = oldPosition;
            }
            else
            {
                // Update current interaction variable
                newPosition = rightPosition;
            }
        }
        else if (leftMapInteraction)
        {
            if (interaction != InteractionType.MoveLeft)
            {
                // Start new move interaction with left controller
                interaction = InteractionType.MoveLeft;
                oldPosition = leftPosition;
                newPosition = oldPosition;
            }
            else
            {
                // Update current interaction variable
                newPosition = leftPosition;
            }
        }
        else if (!rightMapInteraction && !leftMapInteraction)
        {
            // Reset interaction to None
            interaction = InteractionType.None;
        }

        // Handle interaction & update old variables
        switch (interaction)
        {
            case InteractionType.Zoom:
                zoomInteraction(oldDistance, newDistance);
                moveInteraction(oldPosition, newPosition);
                rotateInteraction(oldControllerAngle, newControllerAngle);

                // Update old variables with transforms applied
                updateGlobeControlInput();
                oldDistance = controllerDistance;
                oldPosition = Vector3.Lerp(rightPosition, leftPosition, 0.5f);
                oldControllerAngle = controllerAngle;
                break;
            case InteractionType.MoveRight:
                moveInteraction(oldPosition, newPosition);

                // Update old variables with transforms applied
                updateGlobeControlInput();
                oldPosition = rightPosition;
                break;
            case InteractionType.MoveLeft:
                moveInteraction(oldPosition, newPosition);

                // Update old variables with transforms applied
                updateGlobeControlInput();
                oldPosition = leftPosition;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// This method updates all variables used for globe interaction based on the current input.
    /// </summary>
    void updateGlobeControlInput()
    {
        // VR Input
        foreach (HandController item in handControllers)
        {
            if (item.Right)
            {
                rightMapInteraction = item.TriggerState;
                rightPosition = item.transform.position;
            }
            else if (item.Left)
            {
                leftMapInteraction = item.TriggerState;
                leftPosition = item.transform.position;
            }
        }

        controllerDistance = Vector3.Distance(rightPosition, leftPosition);
        controllerAngle = Vector3.SignedAngle(transform.up, rightPosition - leftPosition, transform.position);

        // Keyboard input: debug only
        if (Input.GetKey(KeyCode.W))
        {
            zoomInteraction(1, 1.01f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            zoomInteraction(1, 0.99f);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveInteraction(new Vector3(0, 0, 10), new Vector3(0, 10 * Time.deltaTime, 10));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveInteraction(new Vector3(0, 0, 10), new Vector3(0, -10 * Time.deltaTime, 10));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveInteraction(new Vector3(0, 0, 10), new Vector3(10 * Time.deltaTime, 0, 10));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveInteraction(new Vector3(0, 0, 10), new Vector3(-10 * Time.deltaTime, 0, 10));
        }
    }

    /// <summary>
    /// Rotates the player around the globe based on controller position.
    /// 
    /// This creates the visual effect of rotating the globe in front of the player.
    /// </summary>
    /// <param name="oldPos">Controller position from the last frame</param>
    /// <param name="newPos">Controller position from the current frame</param>
    void moveInteraction(Vector3 oldPos, Vector3 newPos)
    {
        Vector3 axis = Vector3.Cross(newPos - oldPos, oldPos);
        float angle = -Vector3.SignedAngle(oldPos, newPos, axis);

        transform.RotateAround(Vector3.zero, axis, angle);
    }

    /// <summary>
    /// Scales the player towards the closest point on the globe based on controller distance.
    /// 
    /// This creates the visual effect of zooming in & out.
    /// The player is temporarily parented to the scale handler object to allow scaling towards a point in space.
    /// </summary>
    /// <param name="oldDist"></param>
    /// <param name="newDist"></param>
    void zoomInteraction(float oldDist, float newDist)
    {
        // Move the scale handler to the closest point on the globe
        Vector3 positionCorrection = scaleHandler.transform.position; // Variable to move player back afer moving the scaleHandler
        scaleHandler.transform.position = scaleTarget.transform.position.normalized * globeRadius;
        positionCorrection -= scaleHandler.transform.position;
        transform.position += positionCorrection;

        // Scale the player towards the scale handler
        float zoomFactor = oldDist / newDist;
        float headDistance = Vector3.Distance(Vector3.zero, headCamera.transform.position);

        if (headDistance > minDistance && headDistance < maxDistance || headDistance <= minDistance && zoomFactor > 1 || headDistance >= maxDistance && zoomFactor < 1)
        {
            scaleHandler.localScale *= zoomFactor;
        }
    }

    /// <summary>
    /// Rotates the player around their forward axis to reorient north and south poles.
    /// </summary>
    /// <param name="oldAngle">Angle of both controllers relative to player.up from the last frame</param>
    /// <param name="newAngle">Angle of both controllers relative to player.up from the current frame</param>
    void rotateInteraction(float oldAngle, float newAngle)
    {
        transform.RotateAround(Vector3.zero, headCamera.transform.position, oldAngle - newAngle);
    }
}

/// <summary>
/// Possible types of interaction the player can perform.
/// </summary>
enum InteractionType
{
    None,
    Zoom,
    MoveRight,
    MoveLeft
}
