using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abtract class that all via raycast clickable UI elements should inherit from.
/// </summary>
public abstract class UIButton : MonoBehaviour
{
    public abstract void onRaycastInteraction();
}
