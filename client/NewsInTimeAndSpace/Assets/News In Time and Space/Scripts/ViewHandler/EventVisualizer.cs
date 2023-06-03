using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: EventVisualizer
/*
 * EventVisualizer
 * 
 * This class handles the visualization of an event object seperate from the map.
 */
public class EventVisualizer : Visualizer
{
    NewsEvent newsEvent;

    public NewsEvent NewsEvent { get => newsEvent; set => newsEvent = value; }

    public void setVariables(NewsEvent newsEvent, float baseSize)
    {
        this.NewsEvent = newsEvent;
        this.BaseSize = baseSize;
    }

    public override void updateVisualization()
    {
        // TODO: visualizer of events
    }

    public override void onRaycastSelect()
    {
        // TODO: Show information of this specific event
    }

    public override void onRaycastDeselect()
    {
        // TODO: Hide information of this event
    }
}
