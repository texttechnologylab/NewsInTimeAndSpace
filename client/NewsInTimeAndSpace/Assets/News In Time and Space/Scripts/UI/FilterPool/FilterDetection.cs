using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class for the FilterHandler. Needs to be attached to a box in which to place filters for FilterHandler.
/// Detects new gameobjects and tells FilterHandler.
/// </summary>
public class FilterDetection : MonoBehaviour
{
    public FilterHandler filterHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// Calls method on FilterHandler.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        filterHandler.TryAddFilter(other.gameObject);
    }
}
