using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FilterPreset
{
    FilterPool filterPool;
    FilterBallObject[] filterBall;

    public FilterPreset(FilterPool filterPool, List<GameObject> filterBall)
    {
        this.filterPool = filterPool;

        List<FilterBallObject> filterBallObject = new List<FilterBallObject>();
        foreach (GameObject gameObject in filterBall)
        {
            FilterBallObject fb = gameObject.GetComponent<FilterBallObject>();
            if (fb != null)
            {
                filterBallObject.Add(fb);
            }
        }

        this.filterBall = filterBallObject.ToArray();
    }
}
