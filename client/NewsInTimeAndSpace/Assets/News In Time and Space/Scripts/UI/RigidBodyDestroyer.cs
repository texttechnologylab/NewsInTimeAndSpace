using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will destroy gameobjects with a rigidbody component that touches its collider.
/// It serves as a bounding box to contain all player and physics interaction to not use resources on objects that leave the box.
/// </summary>
public class RigidBodyDestroyer : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        FilterBallObject fb = other.gameObject.GetComponent<FilterBallObject>();
        if (fb != null)
        {
            Destroy(other.gameObject);
        }
    }
}
