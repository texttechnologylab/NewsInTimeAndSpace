using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class FilterPreset
{
    // filterPool values to save
    public string startDate;
    public string endDate;
    public List<string> actors;
    public List<string> eventTypes;
    public float maxGoldstein;
    public float minGoldstein;
    public float maxTone;
    public float minTone;

    public List<FilterBall> filterBalls;

    public FilterPreset(FilterPool filterPool, List<GameObject> filterBall)
    {
        this.startDate = filterPool.StartDate.ToString("yyyy-MM-dd");
        this.endDate = filterPool.EndDate.ToString("yyyy-MM-dd");
        this.actors = filterPool.Actors.Distinct().ToList();
        this.eventTypes = filterPool.EventTypes.Distinct().ToList();
        if (filterPool.GoldsteinSet)
        {
            this.maxGoldstein = filterPool.MaxGoldstein;
            this.minGoldstein = filterPool.MinGoldstein;
        }
        else
        {
            this.maxGoldstein = 10;
            this.minGoldstein = -10;
        }
        if (filterPool.ToneSet)
        {
            this.maxTone = filterPool.MaxTone;
            this.minTone = filterPool.MinTone;
        }
        else
        {
            this.maxTone = 100;
            this.minTone = -100;
        }

        this.filterBalls = new List<FilterBall>();
        foreach (GameObject g in filterBall)
        {
            FilterBallObject fbObject = g.GetComponent<FilterBallObject>();
            if (fbObject != null)
            {
                this.filterBalls.Add(new FilterBall(fbObject));
            }
        }
    }

    public FilterPool toFilterPool()
    {
        FilterPool filterPool = new FilterPool();
        filterPool.setDates(DateTime.Parse(startDate), DateTime.Parse(endDate));
        filterPool.addActor(actors);
        filterPool.addEventType(eventTypes);
        filterPool.setGoldstein(minGoldstein, maxGoldstein);
        filterPool.setTone(minTone, maxTone);
        filterPool.HasChanged = true;

        return filterPool;
    }
}

[Serializable]
public class FilterBall
{
    public int filterType;
    public string value;
    public string display;

    public FilterBall(int filterType, string value, string display)
    {
        this.filterType = filterType;
        this.value = value;
        this.display = display;
    }

    public FilterBall(FilterBallObject fbObject)
    {
        this.filterType = ((int)fbObject.filterType);
        this.value = fbObject.value;
        this.display = fbObject.display;
    }
}
