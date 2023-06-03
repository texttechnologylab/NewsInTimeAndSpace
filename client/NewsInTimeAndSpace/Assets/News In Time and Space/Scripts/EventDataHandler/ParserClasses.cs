using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using Org.BouncyCastle.Utilities;

/*
 * JSONParser
 * 
 * This class provides functions to extract Group and NewsEvent objects from JSON data.
 */
public class JSONParser
{
    /*
     * Parses the JSON response from the server when requesting events.
     */
    public NewsEvent[] parseEventResultJSON(string jsonData)
    {
        ParserClasses.EventResult eventResult = JsonUtility.FromJson<ParserClasses.EventResult>(jsonData);

        return parseJSONEvents(eventResult.results);
    }

    /*
     * Parses the JSON response from the server when requesting groups.
     */
    public Group[] parseGroupResultJSON(string jsonData)
    {
        ParserClasses.GroupResult groupResult = JsonUtility.FromJson<ParserClasses.GroupResult>(jsonData);
        Group[] groups = new Group[groupResult.results.Length];

        for (int i = 0; i < groupResult.results.Length; i++)
        {
            ParserClasses.JSONGroup jsonGroup = groupResult.results[i];
            Vector2 location = new Vector2(jsonGroup.Location.coordinates[0], jsonGroup.Location.coordinates[1]);
            string[] eventIDs = parseJSONGroupEvents(jsonGroup.Events);

            groups[i] = new Group(
                jsonGroup.Group_ID,
                jsonGroup.City,
                jsonGroup.Region,
                jsonGroup.Country,
                jsonGroup.Country_Code,
                location,
                eventIDs,
                jsonGroup.Connections);
        }

        return groups;
    }

    string[] parseJSONGroupEvents(ParserClasses.JSONGroupEvent[] JSONGroupEvents)
    {
        string[] eventIDs = new string[JSONGroupEvents.Length];

        for (int i = 0; i < JSONGroupEvents.Length; i++)
        {
            eventIDs[i] = JSONGroupEvents[i].GLOBALEVENTID;
        }

        return eventIDs;
    }

    /*
     * Parses an array of events in JSON format into an array of NewsEvents objects.
     */
    NewsEvent[] parseJSONEvents(ParserClasses.JSONEvent[] jsonEvents)
    {
        NewsEvent[] newsEvents = new NewsEvent[jsonEvents.Length];

        for (int i = 0; i < jsonEvents.Length; i++)
        {
            newsEvents[i] = parseJSONEvent(jsonEvents[i]);
        }

        return newsEvents;
    }

    /*
     * Parses an event in JSON format into a NewsEvent object.
     */
    NewsEvent parseJSONEvent(ParserClasses.JSONEvent jsonEvent)
    {
        Vector2 location = new Vector2(jsonEvent.Location.coordinates[0], jsonEvent.Location.coordinates[1]);
        DateTime date = DateTime.Parse(jsonEvent.Date);
        Actor[] actors = parseJSONEventActors(jsonEvent.Actors);
        Media[] media = parseJSONMedia(jsonEvent.Media);

        return new NewsEvent(jsonEvent.GLOBALEVENTID, location, actors, jsonEvent.Type, date, jsonEvent.Source, null, media, jsonEvent.Title, jsonEvent.Location_Name, jsonEvent.AvgTone, jsonEvent.GoldsteinScale);
    }

    Media[] parseJSONMedia(ParserClasses.JSONMedia jsonMedia)
    {
        Media[] media = new Media[1];
        media[0] = new Media(jsonMedia.type, jsonMedia.content);

        return media;
    }

    Actor[] parseJSONEventActors(ParserClasses.JSONEventActor[] jsonActors)
    {
        Actor[] actors = new Actor[jsonActors.Length];

        for (int i = 0; i < jsonActors.Length; i++)
        {
            actors[i] = parseJSONEventActor(jsonActors[i]);
        }

        return actors;
    }

    Actor parseJSONEventActor(ParserClasses.JSONEventActor jsonActor)
    {
        Vector2 location = new Vector2(jsonActor.Location.coordinates[0], jsonActor.Location.coordinates[1]);

        return new Actor(jsonActor.Name, location, jsonActor.Type, 0);
    }

    public Actor[] parseActorsResultJSON(string jsonData)
    {
        ParserClasses.ActorsResult actorsResult = JsonUtility.FromJson<ParserClasses.ActorsResult>(jsonData);
        Actor[] actors = new Actor[actorsResult.results.Length];

        for (int i = 0; i < actorsResult.results.Length; i++)
        {
            ParserClasses.JSONActor jsonActor = actorsResult.results[i];
            Vector2 location = new Vector2(jsonActor.Location.coordinates[0], jsonActor.Location.coordinates[1]);
            actors[i] = new Actor(jsonActor.Actor, location, jsonActor.Type, jsonActor.Count);
        }

        return actors;
    }

    public EventType[] parseEventTypesResultJSON(string jsonData)
    {
        ParserClasses.EventTypesResult eventTypesResult = JsonUtility.FromJson<ParserClasses.EventTypesResult>(jsonData);
        EventType[] eventTypes = new EventType[eventTypesResult.results.Length];

        for (int i = 0; i < eventTypesResult.results.Length; i++)
        {
            ParserClasses.JSONEventType jsonEventType = eventTypesResult.results[i];
            eventTypes[i] = new EventType(jsonEventType.Type, jsonEventType.Type_Name, jsonEventType.BaseType, jsonEventType.BaseType_Name, jsonEventType.Count);
        }

        return eventTypes;
    }
}

/*
 * This namespace contains classes that exactly match the structure found in JSON responses from the server.
 * They are only ment to be utilized with zhe Unity JsonUtility which requires naming of variables and structure to match exactly.
 */
namespace ParserClasses
{
    /*
     * EventResult
     * 
     * When requesting events from the server the result contains an array of events in JSON format.
     */
    [Serializable]
    public class EventResult
    {
        public JSONEvent[] results;
    }

    /*
     * JSONEvent
     * 
     * News events in JSON format
     */
    [Serializable]
    public class JSONEvent
    {
        public string GLOBALEVENTID;
        public string Location_Name;
        public JSONLocation Location;
        public JSONEventActor[] Actors;
        public string Source;
        public float GoldsteinScale;
        public float AvgTone;
        public string Date;
        public string Type;
        public JSONMedia Media;
        public string Title;
    }

    [Serializable]
    public class JSONMedia
    {
        public string type;
        public string content;
    }

    [Serializable]
    public class JSONEventActor
    {
        public string Name;
        public string Type;
        public JSONLocation Location;
    }

    /*
     * EventResult
     * 
     * When requesting groups from the server the result contains an array of groups in JSON format.
     */
    [Serializable]
    public class GroupResult
    {
        public JSONGroup[] results;
    }

    /*
     * JSONGroup
     * 
     * Event groups in JSON format
     */
    [Serializable]
    public class JSONGroup
    {
        public int Group_ID;
        public int Count;
        public string City;
        public string Region;
        public string Country;
        public string Country_Code;
        public JSONLocation Location;
        public JSONGroupEvent[] Events;
        public int[] Connections;
    }

    /*
     * JSONEventID
     * 
     * An object containing an event ID in JSON format
     */
    [Serializable]
    public class JSONGroupEvent
    {
        public string GLOBALEVENTID;
    }

    /* 
     * JSONLocation
     * 
     * Geolocations in JSON format
     */
    [Serializable]
    public class JSONLocation
    {
        public float[] coordinates;
    }

    [Serializable]
    public class ActorsResult
    {
        public JSONActor[] results;
    }

    [Serializable]
    public class JSONActor
    {
        public string Actor;
        public JSONLocation Location;
        public string Type;
        public int Count;
    }

    [Serializable]
    public class EventTypesResult
    {
        public JSONEventType[] results;
    }

    [Serializable]
    public class JSONEventType
    {
        public string Type;
        public string Type_Name;
        public string BaseType;
        public string BaseType_Name;
        public int Count;
    }
}
