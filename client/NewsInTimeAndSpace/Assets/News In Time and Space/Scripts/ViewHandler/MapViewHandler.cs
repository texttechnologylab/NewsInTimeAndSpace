using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * MapViewHandler
 * 
 * This class handles what to display to the user and invokes necessary data updates from components.
 */
/// <summary>
/// MapViewHandler
/// 
/// This class handles what to display and requests visualization and data updates if necessary.
/// </summary>
[RequireComponent(typeof(MapDataHandler))]
[RequireComponent(typeof(VisualizerManager))]
public class MapViewHandler : MonoBehaviour
{
    // View variables
    public Transform player;
    public GroupVisualizerManager groupViewManager;
    public EventVisualizerManager eventViewManager;
    public UIDataHandler uiDataHandler;
    public float regionDistanceThreshold = 250f;
    public float cityDistanceThreshold = 175f;
    public float globeRadius;
    public GameObject loadSymbol;
    System.DateTime defaultStartDate = System.DateTime.Now.AddDays(0);
    System.DateTime defaultEndDate = System.DateTime.Now;
    ViewMode viewMode;
    GroupAggregationLevel groupAggregationLevel;
    bool filterPoolChange;
    bool updateGroupVisualization;
    bool updateEventVisualization;
    bool updateGroupData;
    bool updateEventData;
    string[] eventIDs;

    // Data variables
    public FilterPool filterPool;
    MapDataHandler dataHandler;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize data handlers
        filterPool = new FilterPool();
        filterPool.setDates(defaultStartDate, defaultEndDate);
        dataHandler = GetComponent<MapDataHandler>();

        // Debug
        //string[] types = { "040", "041", "042" };
        //filterPool.addEventType(types);
        // Debug end

        // Initialize display variables
        viewMode = ViewMode.Groups;
        groupAggregationLevel = GroupAggregationLevel.Country;
        eventIDs = new string[0];
        loadSymbol.SetActive(false);

        // Trigger initial data update
        updateGroupData = true;
        updateGroupVisualization = false;
    }

    // Update is called once per frame
    void Update()
    {
        filterPoolChange = filterPool.HasChanged;
        groupVisualizationUpdate();
        eventVisualizationUpdate();
        aggregationLevelUpdate();
        groupDataUpdate();
        eventDataUpdate();
    }

    /// <summary>
    /// Triggers group data updates if the applied filters have been changed.
    /// </summary>
    private void groupDataUpdate()
    {
        // Trigger Data update if filters changed
        updateGroupData = filterPoolChange;
        if (updateGroupData)
        {
            // Trigger UI data updates
            uiDataHandler.updateActors(filterPool.withoutActors());
            uiDataHandler.updateEventTypes(filterPool.withoutEventTypes());

            // Trigger data update for all aggregation levels
            for (int aggregationLevel = 0; aggregationLevel < 3; aggregationLevel++)
            {
                dataHandler.updateGroups((GroupAggregationLevel)aggregationLevel, filterPool);
            }

            updateGroupData = false;
            loadSymbol.SetActive(true);
        }
    }

    private void eventDataUpdate()
    {
        // TODO: filterPoolChange updates groups. If an already selected group gets updated (contains new set of eventIDs) it needs to re trigger showEvents with new IDs
        updateEventData = updateEventData || filterPoolChange;
        if (updateEventData)
        {
            dataHandler.updateNewsEvents(eventIDs);
            updateEventData = false;
        }
    }

    /// <summary>
    /// Updates the group aggregation level based on player distance from the globe.
    /// Changing the aggregation level triggers the group visualizaion to update.
    /// </summary>
    private void aggregationLevelUpdate()
    {
        // Update Aggregation level based on player distance from globe & trigger visualization update
        float playerDistance = Vector3.Distance(player.transform.position, Vector3.zero);
        if (playerDistance > regionDistanceThreshold)
        {
            if (dataHandler.GroupsUpdated[(int)GroupAggregationLevel.Country])
            {
                if (groupAggregationLevel != GroupAggregationLevel.Country)
                {
                    groupAggregationLevel = GroupAggregationLevel.Country;
                    updateGroupVisualization = true;
                    Debug.Log("Aggregation level changed to " + groupAggregationLevel.ToString());
                }
                loadSymbol.SetActive(false);
            } 
            else
            {
                loadSymbol.SetActive(true);
            }
        }
        else if (playerDistance > cityDistanceThreshold)
        {
            if (dataHandler.GroupsUpdated[(int)GroupAggregationLevel.Region])
            {
                if (groupAggregationLevel != GroupAggregationLevel.Region)
                {
                    groupAggregationLevel = GroupAggregationLevel.Region;
                    updateGroupVisualization = true;
                    Debug.Log("Aggregation level changed to " + groupAggregationLevel.ToString());
                }
                loadSymbol.SetActive(false);
            }
            else
            {
                loadSymbol.SetActive(true);
            }
        }
        else
        {
            if (dataHandler.GroupsUpdated[(int)GroupAggregationLevel.City])
            {
                if (groupAggregationLevel != GroupAggregationLevel.City)
                {
                    groupAggregationLevel = GroupAggregationLevel.City;
                    updateGroupVisualization = true;
                    Debug.Log("Aggregation level changed to " + groupAggregationLevel.ToString());
                }
                loadSymbol.SetActive(false);
            }
            else
            {
                loadSymbol.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Updates group visualization if new data is available or it was triggered by an aggregation level change.
    /// </summary>
    private void groupVisualizationUpdate()
    {
        // Update Visualization if new data for current view mode available
        updateGroupVisualization = updateGroupVisualization || dataHandler.GroupsUpdated[(int)groupAggregationLevel] && dataHandler.GroupsUnread[(int)groupAggregationLevel];
        if (updateGroupVisualization)
        {
            groupViewManager.updateGroupVisualizers(dataHandler.GetGroups(groupAggregationLevel), groupAggregationLevel, globeRadius);
            updateGroupVisualization = false;
            Debug.Log("Visualization updated.");
            loadSymbol.SetActive(false);
        }
    }

    private void eventVisualizationUpdate()
    {
        updateEventVisualization = updateEventVisualization || dataHandler.NewsEventsUpdated && dataHandler.NewsEventsUnread;
        if (updateEventVisualization)
        {
            eventViewManager.updateEventVisualizers(dataHandler.GetNewsEvents(), globeRadius);
            updateEventVisualization = false;
            Debug.Log("Event visualization updated.");
        }
    }

    /// <summary>
    /// Saves eventIDs of NewsEvents to show on the globe and triggers the necessary data update.
    /// </summary>
    /// <param name="eventIDs">IDs of the events to show on the globe</param>
    public void showEvents(string[] eventIDs)
    {
        this.eventIDs = eventIDs;
        updateEventData = true;
    }

    /// <summary>
    /// Removes saved eventIDs and deactivates all visualizers.
    /// </summary>
    public void clearEvents()
    {
        this.eventIDs = new string[0];
        eventViewManager.deactivateVisualization();
    }
}
