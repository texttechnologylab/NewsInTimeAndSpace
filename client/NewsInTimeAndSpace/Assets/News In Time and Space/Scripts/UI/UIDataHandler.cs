using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDataHandler : MonoBehaviour
{
    public WebRequestHandler eventTypesHandler;
    public WebRequestHandler actorsHandler;

    bool eventTypesUpdated;
    bool eventTypesUnread;
    bool actorsUpdated;
    bool actorsUnread;
    Actor[] actors;
    EventType[] eventTypes;

    JSONParser jsonParser;

    public bool EventTypesUpdated { get => eventTypesUpdated; }
    public bool EventTypesUnread { get => eventTypesUnread; }
    public bool ActorsUpdated { get => actorsUpdated; }
    public bool ActorsUnread { get => actorsUnread; }

    // Start is called before the first frame update
    void Awake()
    {
        eventTypesHandler.Request = "extra";
        eventTypesHandler.SubPath = "types";
        actorsHandler.Request = "extra";
        actorsHandler.SubPath = "actors";
        eventTypesUpdated = false;
        eventTypesUnread = false;
        actorsUpdated = false;
        actorsUnread = false;
        actors = new Actor[0];

        jsonParser = new JSONParser();
    }

    // Update is called once per frame
    void Update()
    {
        if (actorsHandler.DataUpdated && actorsHandler.DataUnread)
        {
            actors = jsonParser.parseActorsResultJSON(actorsHandler.Data);
            actorsUpdated = true;
            actorsUnread = true;
            Debug.Log("Actors received and parsed.");
        }
        if (eventTypesHandler.DataUpdated && eventTypesHandler.DataUnread)
        {
            eventTypes = jsonParser.parseEventTypesResultJSON(eventTypesHandler.Data);
            eventTypesUpdated = true;
            eventTypesUnread = true;
            Debug.Log("EventTypes received and parsed.");
        }
    }

    public Actor[] GetActors()
    {
        actorsUnread = false;
        return actors;
    }

    void startActorsRequest()
    {
        actorsHandler.startRequest();
        actorsUpdated = false;
    }

    public void updateActors(FilterPool filterPool)
    {
        actorsHandler.clearParameters();
        actorsHandler.addParameter(filterPool);
        startActorsRequest();
    }

    public EventType[] GetEventTypes()
    {
        eventTypesUnread = false;
        return eventTypes;
    }

    void startEventTypesRequest()
    {
        eventTypesHandler.startRequest();
        eventTypesUpdated = false;
    }

    public void updateEventTypes(FilterPool filterPool)
    {
        eventTypesHandler.clearParameters();
        eventTypesHandler.addParameter(filterPool);
        startEventTypesRequest();
    }

}
