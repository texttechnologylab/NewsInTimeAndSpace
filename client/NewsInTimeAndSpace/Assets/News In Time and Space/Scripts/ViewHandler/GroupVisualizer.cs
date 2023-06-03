using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GroupVisualizer
/// 
/// This class handles the visual representation of a group object.
/// </summary>
public class GroupVisualizer : Visualizer
{
    public TMPro.TMP_Text text;
    MapViewHandler mapViewHandler;
    Group group;

    public Group Group { get => group; set => group = value; }

    /*
     * Overwrite the variables needed for group visualization
     * @param group     New group object corresponding to this visualizer
     * @param baseSize  (deprecated??) The minimum size a visualizer can be
     * @param color     (deprecated??) The color of this visualizer
     */
    /// <summary>
    /// Overwrite the variables needed for visualization.
    /// </summary>
    /// <param name="group">Group object belongig to this visualizer</param>
    /// <param name="baseSize">Minimum size a visualizer can be</param>
    public void setVariables(Group group, float baseSize, MapViewHandler mapViewHandler)
    {
        this.Group = group;
        this.BaseSize = baseSize;
        this.mapViewHandler = mapViewHandler;
    }

    /// <summary>
    /// Updates the visualizaion of this group based on its stored data.
    /// Scales itself logarithmically based on the number of events in this group and sets its name to the country, region, city.
    /// </summary>
    public override void updateVisualization()
    {
        // Update scale & name
        float scale = BaseSize * Mathf.Log(2.0f + (float)group.EventCount, 2.0f);
        this.name = group.Country + group.Region + group.City + "(" + group.EventCount + ")" + "(" + group.Location + ")";
        text.text = group.EventCount.ToString();
        text.transform.LookAt(Vector3.zero);

        transform.localScale = new Vector3(scale, scale, scale);
    }

    public override void onRaycastSelect()
    {
        if (group.AggregationLevel == GroupAggregationLevel.Region || group.AggregationLevel == GroupAggregationLevel.City || group.AggregationLevel == GroupAggregationLevel.Country)
        {
            // Tell mapviewhandler to trigger event data update & event visualization update
            mapViewHandler.showEvents(group.EventIDs);
        }

        // TODO: Handle deactivation of eventVisualizers (self.onDeactivate and on change of aggregation)
    }

    public override void onRaycastDeselect()
    {
        mapViewHandler.clearEvents();
    }
}
