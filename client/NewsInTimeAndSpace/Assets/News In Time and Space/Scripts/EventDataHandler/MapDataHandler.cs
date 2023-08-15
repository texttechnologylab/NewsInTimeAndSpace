using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

/*
 * MapDataHandler
 * 
 * This class handles the requesting and updating of data for map visualization.
 * It always holds the last received data that is returned on request.
 */
/// <summary>
/// MapDataHandler
/// 
/// This class handles updating, storing and providing of Group and NewsEvent data to the MapViewHandler for visualization.
/// </summary>
public class MapDataHandler : MonoBehaviour
{
    Group[][] groups;
    NewsEvent[] newsEvents;
    //string[] newsEventIDs; // TODO: Event IDs necessary or can be left out????

    // unread variables keep track if new data is stored but hasn't been read yet
    bool[] groupsUpdated;
    bool[] groupsUnread;
    bool newsEventsUpdated;
    bool newsEventsUnread;

    WebRequestHandler[] groupHandler = new WebRequestHandler[3];
    WebRequestHandler eventHandler;

    JSONParser jsonParser;

    public bool[] GroupsUpdated { get => groupsUpdated; }
    public bool[] GroupsUnread { get => groupsUnread; }
    public bool NewsEventsUpdated { get => newsEventsUpdated; }
    public bool NewsEventsUnread { get => newsEventsUnread; }

    private void Start()
    {
        // Initialize components
        for (int i = 0; i < 3; i++)
        {
            groupHandler[i] = gameObject.AddComponent<WebRequestHandler>();
            groupHandler[i].Request = "groups";
        }

        eventHandler = gameObject.AddComponent<WebRequestHandler>();
        eventHandler.Request = "events";
        eventHandler.SubPath = "multiple";

        // initialize variables
        groups = new Group[3][];
        newsEvents = new NewsEvent[0];

        // Data flags: updated = new data requested and available, unread = new data available and not read yet
        groupsUpdated = new bool[3];
        groupsUnread = new bool[3];
        newsEventsUpdated = false;
        newsEventsUnread = false;

        jsonParser = new JSONParser();
    }

    private void Update()
    {
        for (int aggregationLevel = 0; aggregationLevel < 3; aggregationLevel++)
        {
            // Update data if new data is available from the WebRequestHandler
            if (groupHandler[aggregationLevel].DataUpdated && groupHandler[aggregationLevel].DataUnread)
            {
                groups[aggregationLevel] = jsonParser.parseGroupResultJSON(groupHandler[aggregationLevel].Data);
                groupsUpdated[aggregationLevel] = true;
                groupsUnread[aggregationLevel] = true;
                Debug.Log(groups[aggregationLevel].Length + " " + ((GroupAggregationLevel)aggregationLevel).ToString()
                    + " groups received and parsed");
            }
        }

        if (eventHandler.DataUpdated && eventHandler.DataUnread)
        {
            newsEvents = jsonParser.parseEventResultJSON(eventHandler.Data);
            newsEventsUpdated = true;
            newsEventsUnread = true;
            Debug.Log("Events received and parsed");
        }
    }

    /// <summary>
    /// Returns the currently stored Groups based on GroupAggregationLevel
    /// </summary>
    /// <param name="aggregationLevel">Aggregation level of requested groups</param>
    /// <returns>Array of groups</returns>
    public Group[] GetGroups(GroupAggregationLevel aggregationLevel)
    {
        groupsUnread[(int)aggregationLevel] = false;
        return groups[(int)aggregationLevel];
    }

    /// <summary>
    /// Starts a webrequest to update stored groups based on aggregation level and sets updated flag.
    /// 
    /// groupsUpdated is false while waiting for a webrequest, otherwise it's true.
    /// </summary>
    /// <param name="aggregationLevel">Aggregation level of requested groups</param>
    void startGroupRequest(GroupAggregationLevel aggregationLevel)
    {
        groupHandler[(int)aggregationLevel].SubPath = "/" + aggregationLevel.ToString().ToLower() + "/ids";

        groupHandler[(int)aggregationLevel].startRequest();
        groupsUpdated[(int)aggregationLevel] = false;
    }

    /// <summary>
    /// Clears all parameters and subpaths to start a new webrequest for Group data.
    /// 
    /// StartGroupRequest() is called to start the webrequest.
    /// </summary>
    /// <param name="aggregationLevel">Aggregation level of requested groups</param>
    public void updateGroups(GroupAggregationLevel aggregationLevel)
    {
        groupHandler[(int)aggregationLevel].clearAll();
        startGroupRequest(aggregationLevel);
    }

    /// <summary>
    /// Clears the subpath and updates all parameters to start a new webrequest for Group data.
    /// 
    /// StartGroupRequest() is called to start the webrequest.
    /// </summary>
    /// <param name="aggregationLevel">Aggregation level of requested groups</param>
    /// <param name="filterPool">FilterPool containing all filters to apply to this request</param>
    public void updateGroups(GroupAggregationLevel aggregationLevel, FilterPool filterPool)
    {
        groupHandler[(int)aggregationLevel].clearAll();
        groupHandler[(int)aggregationLevel].addParameter(filterPool);
        startGroupRequest(aggregationLevel);
    }

    /// <summary>
    /// Returns the currently stored NewsEvents
    /// </summary>
    /// <returns>Array of NewsEvents</returns>
    public NewsEvent[] GetNewsEvents()
    {
        newsEventsUnread = false;
        return newsEvents;
    }

    /// <summary>
    /// Starts a webrequest to update stored NewsEvents and sets updated flag.
    /// 
    /// newsEventsUpdated is false while waiting for a webrequest, otherwise it's true.
    /// </summary>
    void startEventRequest()
    {
        eventHandler.startRequest();
        newsEventsUpdated = false;
    }

    /// <summary>
    /// Clears all parameters and subpaths to start a new webrequest for NewsEvent data.
    /// 
    /// StartEventRequest() is called to start the webrequest.
    /// </summary>
    public void updateNewsEvents()
    {
        eventHandler.clearParameters();
        startEventRequest();
    }

    /// <summary>
    /// Clears the subpath and updates all parameters to start a new webrequest for NewsEvent data.
    /// 
    /// StartEventRequest() is called to start the webrequest.
    /// </summary>
    /// <param name="filterPool">FilterPool containing all filters to apply to this request</param>
    public void updateNewsEvents(FilterPool filterPool)
    {
        eventHandler.clearParameters();
        eventHandler.addParameter(filterPool);
        startEventRequest();
    }

    /// <summary>
    /// Requests a list of NewsEvents by their IDs.
    /// 
    /// StartEventRequest() is called to start the webrequest.
    /// </summary>
    /// <param name="eventIDs">IDs of the events</param>
    public void updateNewsEvents(string[] eventIDs)
    {
        if (eventIDs.Length > 0)
        {
            eventHandler.clearParameters();
            eventHandler.addParameter("id", eventIDs);
            startEventRequest();
        }
        else
        {
            eventHandler.clearData();
        }
    }
}
