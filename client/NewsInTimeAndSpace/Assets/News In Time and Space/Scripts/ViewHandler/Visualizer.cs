using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* (deprecated??)
 * Visualizer
 * 
 * Abstract class that other visualizers are based upon
 */
public abstract class Visualizer : MonoBehaviour
{
    float baseSize;

    public float BaseSize { get => baseSize; set => baseSize = value; }

    public void setVariables(float baseSize)
    {
        this.BaseSize = baseSize;
    }

    public abstract void updateVisualization();
    public abstract void onRaycastSelect();
    public abstract void onRaycastDeselect();
}
