using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// FilterPool
/// 
/// This class contains all filters customizable by the user.
/// </summary>
[Serializable]
public class FilterPool
{
    bool datesSet;
    DateTime startDate;
    DateTime endDate;
    bool locationSet;
    Vector2 location;
    float radius;
    List<string> actors;
    List<string> eventTypes; // Can't be int to keep leading zeros
    float maxGoldstein;
    float minGoldstein;
    bool goldsteinSet;
    float maxTone;
    float minTone;
    bool toneSet;
    int limit;
    bool limitSet;

    bool hasChanged; // True if filter updated and unread

    public DateTime StartDate { get { HasChanged = false; return startDate; } }
    public DateTime EndDate { get { HasChanged = false; return endDate; } }
    public Vector2 Location { get { HasChanged = false; return location; } }
    /// <summary>
    /// Get & Set for the radius filter parameter.
    /// Reading this parameter will set HasChanged to false.
    /// </summary>
    public float Radius
    {
        get { HasChanged = false; return radius; }
        set { HasChanged = true; radius = value; }
    }
    /// <summary>
    /// Get & Set for the actors filter parameter.
    /// Reading this parameter will set HasChanged to false.
    /// </summary>
    public List<string> Actors
    {
        get { HasChanged = false; return actors; }
        set { HasChanged = true; actors = value; }
    }
    /// <summary>
    /// Get & Set for the Event types filter parameter.
    /// Reading this parameter will set HasChanged to false.
    /// </summary>
    public List<string> EventTypes
    {
        get { HasChanged = false; return eventTypes; }
        set { HasChanged = true; eventTypes = value; }
    }
    public float MinGoldstein
    {
        get { HasChanged = false; return minGoldstein; }
    }
    public float MaxGoldstein
    {
        get { HasChanged = false; return maxGoldstein; }
    }
    public float MinTone
    {
        get { HasChanged = false; return minTone; }
    }
    public float MaxTone
    {
        get { HasChanged = false; return maxTone; }
    }
    /// <summary>
    /// Get & Set for the limit filter parameter.
    /// Reading this parameter will set HasChanged to false.
    /// </summary>
    public int Limit
    {
        get { HasChanged = false; return limit; }
        set { HasChanged = true; limit = value; }
    }

    public bool HasChanged { get => hasChanged; set => hasChanged = value; }
    public bool DatesSet { get => datesSet; set => datesSet = value; }
    public bool LocationSet { get => locationSet; set => locationSet = value; }
    public bool GoldsteinSet { get => goldsteinSet; set => goldsteinSet = value; }
    public bool ToneSet { get => toneSet; set => toneSet = value; }
    public bool LimitSet { get => limitSet; set => limitSet = value; }

    /// <summary>
    /// Constructor for a new FilterPool object.
    /// </summary>
    public FilterPool()
    {
        actors = new List<string>();
        eventTypes = new List<string>();
        HasChanged = false;
    }

    public FilterPool withoutEventTypes()
    {
        FilterPool fp = new FilterPool();

        if (this.datesSet)
        {
            fp.setDates(this.startDate, this.endDate);
        }
        if (this.locationSet)
        {
            fp.setLocation(this.location, this.radius);
        }
        if (this.actors.Count > 0)
        {
            fp.addActor(this.actors);
        }
        if (this.limitSet)
        {
            fp.setLimit(this.limit);
        }

        return fp;
    }

    public FilterPool withoutActors()
    {
        FilterPool fp = new FilterPool();

        if (this.datesSet)
        {
            fp.setDates(this.startDate, this.endDate);
        }
        if (this.locationSet)
        {
            fp.setLocation(this.location, this.radius);
        }
        if (this.eventTypes.Count > 0)
        {
            fp.addEventType(this.eventTypes);
        }
        if (this.limitSet)
        {
            fp.setLimit(this.limit);
        }

        return fp;
    }

    /// <summary>
    /// Sets the start and end date parameters and the datesSet flag.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    /// <param name="startDate">Earliest date of NewsEvents to include</param>
    /// <param name="endDate">Latest date of NewsEvents to include</param>
    public void setDates(DateTime startDate, DateTime endDate)
    {
        HasChanged = true;
        DatesSet = true;
        this.startDate = startDate;
        this.endDate = endDate;
    }

    /// <summary>
    /// Removes the start and end date parameters by setting the datesSet flag to false.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    public void clearDates()
    {
        HasChanged = true;
        DatesSet = false;
    }

    /// <summary>
    /// Sets the location and radius parameters and the locationSet flag.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    /// <param name="location">Geolocation of the search filter in {longitude, latitude} order</param>
    /// <param name="radius">Radius in km around the location for the filter</param>
    public void setLocation(Vector2 location, float radius)
    {
        HasChanged = true;
        LocationSet = true;
        this.location = location;
        this.radius = radius;
    }
    /// <summary>
    /// Sets the limit parameter and the limitSet flag.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    /// <param name="limit">Limit of Documents in the response</param>
    public void setLimit(int limit)
    {
        HasChanged = true;
        LimitSet = true;
        this.limit = limit;
    }

    public void setGoldstein(float min, float max)
    {
        HasChanged = true;
        if (min < -10)
        {
            min = -10;
        }
        if (max > 10)
        {
            max = 10;
        }
        if (min == -10 && max == 10)
        {
            GoldsteinSet = false;
        }
        else
        {
            GoldsteinSet = true;
            this.minGoldstein = min;
            this.maxGoldstein = max;
        }
    }

    public void clearGoldstein()
    {
        HasChanged = true;
        GoldsteinSet = false;
    }

    public void setTone(float min, float max)
    {
        HasChanged = true;
        if (min < -100)
        {
            min = -100;
        }
        if (max > 100)
        {
            max = 100;
        }
        if (min == -100 && max == 100)
        {
            ToneSet = false;
        }
        else
        {
            ToneSet = true;
            this.minTone = min;
            this.maxTone = max;
        }
    }

    public void clearTone()
    {
        HasChanged = true;
        ToneSet = false;
    }

    /// <summary>
    /// Removes the location and radius parameter by setting the locationSet flag to false.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    public void clearLocation()
    {
        HasChanged = true;
        LocationSet = false;
    }

    /// <summary>
    /// Adds an actor to the actors parameter.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    /// <param name="actor">Actor to add to the filterpool</param>
    public void addActor(string actorName)
    {
        HasChanged = true;
        actors.Add(actorName);
    }

    /// <summary>
    /// Adds multiple actors to the actors parameter.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    /// <param name="actor">Actors to add to the filterpool</param>
    public void addActor(IEnumerable<string> actorNames)
    {
        HasChanged = true;
        actors.AddRange(actorNames);
    }

    public void removeActor(string actorName)
    {
        HasChanged = true;
        actors.Remove(actorName);
    }

    /// <summary>
    /// Removes the actors parameter.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    public void clearActors()
    {
        HasChanged = true;
        actors.Clear();
    }

    /// <summary>
    /// Adds an event type cameo code to the event type filters parameter.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    /// <param name="eventType"></param>
    public void addEventType(string eventType)
    {
        if (int.TryParse(eventType, out int num) && num >= 0)
        {
            HasChanged = true;
            eventTypes.Add(eventType);
        }
        else
        {
            Debug.LogError("Trying to pass an eventtype thats invalid (NaN or negative): " + eventType);
        }
    }

    /// <summary>
    /// Adds multiple event type cameo codes to the event type filters parameter.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    /// <param name="eventType"></param>
    public void addEventType(IEnumerable<string> eventType)
    {
        foreach (string type in eventType)
        {
            addEventType(type);
        }
    }

    public void removeEventType(string eventType)
    {
        HasChanged = true;
        eventTypes.Remove(eventType);
    }

    /// <summary>
    /// Removes the event types parameter.
    /// Calling this method will set HasChanged to true.
    /// </summary>
    public void clearEventTypes()
    {
        HasChanged = true;
        eventTypes.Clear();
    }
}
