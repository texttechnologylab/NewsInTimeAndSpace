using System.Collections;
using System.Collections.Generic;
using Ubiq.XR;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Class to handle rigidbody gameobject used as filter object.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FilterBallObject : MonoBehaviour, IGraspable
{
    public FilterHandler.FilterType filterType;
    public string value;
    public string display;
    public FilterMenu3D filterMenu;
    public Transform player;

    Vector3 oldPosition;
    float oldPositionTime;

    Rigidbody rb;

    private FilterHandler myfilterHandler;
    bool isCombined;

    public enum FilterBallLocation
    {
        OnUi,
        InHand,
        Free,
        InFilterPool,
        Combined
    };
    public FilterBallLocation location = FilterBallLocation.OnUi;

    public FilterBallLocation Location
    {
        get { return location; }
        set
        {
            location = value;
            updateRigidbody();
        }
    }
    public FilterHandler MyfilterHandler { get => myfilterHandler; set => myfilterHandler = value; }
    public bool IsCombined { get => isCombined; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        updateRigidbody();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (location == FilterBallLocation.InHand)
        {
            oldPosition = transform.position;
            oldPositionTime = Time.time;
        }
    }

    /// <summary>
    /// Updates rigidbody based on object location.
    /// </summary>
    void updateRigidbody()
    {
        if (location == FilterBallLocation.OnUi || location == FilterBallLocation.InHand || location == FilterBallLocation.Combined)
        {
            rb.isKinematic = true;
        }
        else if (location == FilterBallLocation.Free)
        {
            rb.isKinematic = false;
        }
    }

    /// <summary>
    /// Collision detection for combining filter objects.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        FilterBallObject ball = collision.gameObject.GetComponent<FilterBallObject>();

        if (filterType == FilterHandler.FilterType.actor && !IsCombined && location == FilterBallLocation.InHand
            && ball != null && ball.filterType == FilterHandler.FilterType.actor && !ball.IsCombined && ball.Location == FilterBallLocation.InHand)
        {
            isCombined = true;
            ball.isCombined = true;
            ball.Location = FilterBallLocation.Combined;

            value += "~" + ball.value;
            display += " & " + ball.display;

            ball.transform.parent = transform;
            ball.transform.position = transform.TransformPoint(new Vector3(0, -1, 0));
            ball.transform.localRotation = Quaternion.Euler(0, 0, 0);
            ball.enabled = false;
        }
    }

    /// <summary>
    /// Interaction of being grasped from UI, from the filterpool or from mid air.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="collider"></param>
    public void Grasp(Hand controller, Collider collider)
    {
        if (location == FilterBallLocation.OnUi)
        {
            // Spawn a copy of self and place it in UI
            GameObject newSelf = Instantiate(gameObject);
            FilterBallObject newFilterBall = newSelf.GetComponent<FilterBallObject>();

            newFilterBall.Location = FilterBallLocation.OnUi;
            filterMenu.changeFilterObject(gameObject, newSelf);

            // Attatch self to the hand
            Location = FilterBallLocation.InHand;
            transform.parent = controller.transform;
        }
        else if (location == FilterBallLocation.InFilterPool)
        {
            // Tell filterpool to remove self from filters and attatch self to the hand
            location = FilterBallLocation.InHand;
            MyfilterHandler.RemoveFromHandler(gameObject);
            MyfilterHandler = null;
            transform.parent = controller.transform;
        }
        else if (location == FilterBallLocation.Free)
        {
            // Attatch self to the hand
            Location = FilterBallLocation.InHand;
            transform.parent = controller.transform;
        }
    }

    public void Grasp(Hand controller)
    {
        if (location == FilterBallLocation.OnUi)
        {
            // Spawn a copy of self and place it in UI
            GameObject newSelf = Instantiate(gameObject);
            FilterBallObject newFilterBall = newSelf.GetComponent<FilterBallObject>();

            newFilterBall.Location = FilterBallLocation.OnUi;
            filterMenu.changeFilterObject(gameObject, newSelf);

            // Attatch self to the hand
            Location = FilterBallLocation.InHand;
            transform.parent = controller.transform;
        }
        else if (location == FilterBallLocation.InFilterPool)
        {
            // Tell filterpool to remove self from filters and attatch self to the hand
            location = FilterBallLocation.InHand;
            MyfilterHandler.RemoveFromHandler(gameObject);
            MyfilterHandler = null;
            transform.parent = controller.transform;
        }
        else if (location == FilterBallLocation.Free)
        {
            // Attatch self to the hand
            Location = FilterBallLocation.InHand;
            transform.parent = controller.transform;
        }
    }

    /// <summary>
    /// Method for when object released from hand.
    /// </summary>
    /// <param name="controller"></param>
    public void Release(Hand controller)
    {
        if (location != FilterBallLocation.Combined)
        {
            Location = FilterBallLocation.Free;
            transform.parent = player;

            // Calculate velocity when letting go
            Vector3 velocity = (transform.position - oldPosition) / (Time.time - oldPositionTime);
            rb.AddForce(velocity, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// Method for different outcomes after checking location of object. (deprecated)
    /// </summary>
    private void DoOnInteract()
    {
        if (location == FilterBallLocation.OnUi)
        {
            GameObject newSelf = Instantiate(gameObject);
            newSelf.transform.localScale = transform.lossyScale;
            newSelf.GetComponent<FilterBallObject>().location = FilterBallLocation.InHand;
        }
        else if (location == FilterBallLocation.InHand)
        {
            location = FilterBallLocation.Free;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().AddForce(1, -10000, 1);
        }
        else if (location == FilterBallLocation.InFilterPool)
        {
            location = FilterBallLocation.InHand;
            MyfilterHandler.RemoveFromHandler(gameObject);
            MyfilterHandler = null;
            transform.parent = null;
        }
    }
}
