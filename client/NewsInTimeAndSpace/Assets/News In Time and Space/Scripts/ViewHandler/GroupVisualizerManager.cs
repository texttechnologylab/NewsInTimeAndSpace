using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GroupVisualizerManager
/// 
/// This class handles (de-)activation, positioning and updating of GroupVisualizer objects and the connections between them.
/// </summary>
public class GroupVisualizerManager : VisualizerManager
{
    public MapViewHandler mapViewHandler;
    public float[] groupBaseSize = new float[3];
    public Material lineMaterial;
    public float[] lineWidthMultiplier = new float[3]; // index=aggregation level: 0=country, 1=region
    public float[] maxArcHeight = new float[3];

    LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.generateLightingData = true;
    }

    /// <summary>
    /// (De-)activates, positions and updates necessary GroupVisualizers and builds the connection network between them.
    /// </summary>
    /// <param name="groups">Array of groups to display</param>
    /// <param name="aggregationLevel">The current aggregation level of the groups</param>
    /// <param name="globeRadius">Globe radius used for positioning of visualizers</param>
    public void updateGroupVisualizers(Group[] groups, GroupAggregationLevel aggregationLevel, float globeRadius)
    {
        // Create Group Dictionary based on GroupID
        Dictionary<int, Group> groupDict = new Dictionary<int, Group>();
        for (int i = 0; i < groups.Length; i++)
        {
            groupDict.Add(groups[i].GroupID, groups[i]);
        }

        // Update visualization
        setActiveVisualizers(groups.Length);

        List<GroupConnection> connections = new List<GroupConnection>();
        for (int i = 0; i < groups.Length; i++)
        {
            // Move & Scale visualizers
            GroupVisualizer visualizer = active[i].GetComponent<GroupVisualizer>();

            active[i].transform.position = geoToWorldPosition(groups[i].Location, globeRadius);
            visualizer.setVariables(groups[i], groupBaseSize[(int)aggregationLevel], mapViewHandler);
            visualizer.updateVisualization();

            // Find all group connections
            for (int j = 0; j < groups[i].Connections.Length; j++)
            {
                if (!groups[i].Equals(groupDict[groups[i].Connections[j]]))
                {
                    GroupConnection connection = new GroupConnection(groups[i], groupDict[groups[i].Connections[j]]);

                    if (!connections.Contains(connection))
                    {
                        connections.Add(connection);
                    }
                }
            }
        }

        // Update LineRenderer with all connections (all connections are managed by one LineRenderer component)
        List<Vector3> points = new List<Vector3>();

        foreach (GroupConnection gc in connections)
        {
            int resolution = gc.getResolution(globeRadius);
            points.AddRange(gc.calculateConnectionArc(resolution, maxArcHeight[(int)aggregationLevel], globeRadius));
            points.Add(Vector3.zero);
        }

        lineRenderer.widthMultiplier = lineWidthMultiplier[(int)aggregationLevel];
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
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

/// <summary>
/// GroupConnection
/// 
/// This class represents a connection between two groups and the visualization logic.
/// Groups are connected if actors from events from one group reference another actor from the other group.
/// </summary>
public class GroupConnection
{
    static int minResolution = 3;
    static int maxResolution = 50;

    public Group groupA;
    public Group groupB;

    /// <summary>
    /// Constructor of a connection between two groups.
    /// </summary>
    /// <param name="groupA">Group A of the connection</param>
    /// <param name="groupB">Group B of the connection</param>
    public GroupConnection(Group groupA, Group groupB)
    {
        this.groupA = groupA;
        this.groupB = groupB;
    }

    /// <summary>
    /// Calculates the resolution (number of points) of this GroupConnection.
    /// Resolution of connections scales with distance of groups from eachother.
    /// </summary>
    /// <param name="globeRadius">Radius of the globe determines the maximum possible distance between groups</param>
    /// <returns>Resolution (number of points) defining the visualization of this connection</returns>
    public int getResolution(float globeRadius)
    {
        Vector3 pointA = GeoMaths.CoordinateToPoint(new CoordinateDegrees(groupA.Location.x, groupA.Location.y).ConvertToRadians(), globeRadius);
        Vector3 pointB = GeoMaths.CoordinateToPoint(new CoordinateDegrees(groupB.Location.x, groupB.Location.y).ConvertToRadians(), globeRadius);
        float distanceFactor = Vector3.Distance(pointA, pointB) / (2 * globeRadius);
        return minResolution + Mathf.RoundToInt(distanceFactor * (maxResolution - minResolution));
    }

    /// <summary>
    /// Calculates a parabola following the shortest distance between two points on the globe.
    /// 
    /// Parabola height scales with the distance of the points.
    /// </summary>
    /// <param name="resolution">Number of points making up this connection</param>
    /// <param name="maxArcHeight">Maximum possible height of this connection</param>
    /// <param name="globeRadius">Radius of the globe determines the maximum possible distance between groups</param>
    /// <returns>Array of points describing the connection of the two stored groups</returns>
    public Vector3[] calculateConnectionArc(int resolution, float maxArcHeight, float globeRadius)
    {
        Vector3[] points = new Vector3[resolution];
        Vector3 pointA = GeoMaths.CoordinateToPoint(new CoordinateDegrees(groupA.Location.x, groupA.Location.y).ConvertToRadians(), globeRadius);
        Vector3 pointB = GeoMaths.CoordinateToPoint(new CoordinateDegrees(groupB.Location.x, groupB.Location.y).ConvertToRadians(), globeRadius);

        for (int i = 0; i < resolution; i++)
        {
            float x = (float)i / (float)(resolution - 1);
            points[i] = Vector3.Lerp(pointA, pointB, x).normalized * (globeRadius + scaledParabola(x, Vector3.Distance(pointA, pointB), maxArcHeight, globeRadius));
        }

        return points;
    }

    /*
     * Calculates inverted parabola thats >= 0 between x = 0 and x = 1.
     * Amplitude scales with distance
     */
    /// <summary>
    /// Calculates an inverted parabola that is positive for x between x=0 and x=1.
    /// 
    /// Amplitude scales with distance of the groups.
    /// </summary>
    /// <param name="x">Point on the parabola to calculate</param>
    /// <param name="distance">Distance of groups</param>
    /// <param name="maxArcHeight">Maximum possible height of the parabola</param>
    /// <param name="globeRadius">Radius of the globe determines the maximum possible distance between groups</param>
    /// <returns>Value of the calculated scaled parabola at location x</returns>
    float scaledParabola(float x, float distance, float maxArcHeight, float globeRadius)
    {
        float amplitude = (distance / (2 * globeRadius)) * maxArcHeight;
        return -4 * amplitude * Mathf.Pow((x - 0.5f), 2) + amplitude;
    }

    /// <summary>
    /// Override Equals for GroupConnection: They are equal if both store the same two groupIDs.
    /// </summary>
    /// <param name="obj">GroupConnection to compare</param>
    /// <returns>True if both GroupConnections store the same two groups, false otherwise</returns>
    public bool Equals(GroupConnection obj)
    {
        return obj != null && (
            obj.groupA.GroupID == groupA.GroupID && obj.groupB.GroupID == groupB.GroupID ||
            obj.groupA.GroupID == groupB.GroupID && obj.groupB.GroupID == groupA.GroupID
        );

    }

    /// <summary>
    /// Override Equals for GroupConnection: They are equal if both store the same two groupIDs.
    /// </summary>
    /// <param name="obj">Object to compare</param>
    /// <returns>True if both GroupConnections store the same two groups, false otherwise</returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as GroupConnection);
    }

    /// <summary>
    /// Override == operator for GroupConnection: They are equal if both store the same two groupIDs.
    /// </summary>
    /// <param name="a">Group A to compare</param>
    /// <param name="b">Group B to compare</param>
    /// <returns>True if both GroupConnections store the same two groups, false otherwise</returns>
    public static bool operator ==(GroupConnection a, GroupConnection b)
    {
        if (((object)a) == null || ((object)b) == null)
            return Object.Equals(a, b);

        return a.Equals(b);
    }

    /// <summary>
    /// Override != operator for GroupConnection: They are equal if both store the same two groupIDs.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>True if connections are unequal, false otherwise.</returns>
    public static bool operator !=(GroupConnection a, GroupConnection b)
    {
        if (((object)a) == null || ((object)b) == null)
            return !Object.Equals(a, b);

        return !(a.Equals(b));
    }

    /// <summary>
    /// Override GetHashCode for GroupConnection: Since order of stored groups is irellevant their values are XORed.
    /// </summary>
    /// <returns>HashCode of this connection</returns>
    public override int GetHashCode()
    {
        return this.groupA.GroupID ^ this.groupB.GroupID;
    }
}