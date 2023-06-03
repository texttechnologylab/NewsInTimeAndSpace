using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventVisualizerManager : VisualizerManager
{
    public float eventBaseSize;
    public EventView eventView;
    private List<EventVisualizer> eventVisualizers = new List<EventVisualizer>();

    private void Start()
    {
        eventView.gameObject.SetActive(false);
    }

    public void updateEventVisualizers(NewsEvent[] events, float globeRadius)
    {
        setActiveVisualizers(events.Length);
        Debug.Log("Showing " + events.Length + " events");
        eventVisualizers.Clear();

        for (int i = 0; i < events.Length; i++)
        {
            EventVisualizer visualizer = active[i].GetComponent<EventVisualizer>();

            active[i].transform.position = geoToWorldPosition(events[i].Location, globeRadius);
            visualizer.setVariables(events[i], eventBaseSize);
            visualizer.updateVisualization();
            eventVisualizers.Add(visualizer);
        }
        eventView.gameObject.SetActive(true);
        eventView.openEventView(eventVisualizers);
    }

    public void deactivateVisualization()
    {
        eventView.gameObject.SetActive(false);
        setActiveVisualizers(0);
    }

    /// <summary>
    /// Converts a geolocation into a position in world space.
    /// </summary>
    /// <param name="location">Geolocation in {longitude, latitude} order</param>
    /// <param name="globeRadius">Radius of the globe used for conversion</param>
    /// <returns>Position in world space of the geolocation</returns>
    public static Vector3 geoToWorldPosition(Vector2 location, float globeRadius)
    {
        return GeoMaths.CoordinateToPoint(new CoordinateDegrees(location.x, location.y).ConvertToRadians(), globeRadius);
    }
}
