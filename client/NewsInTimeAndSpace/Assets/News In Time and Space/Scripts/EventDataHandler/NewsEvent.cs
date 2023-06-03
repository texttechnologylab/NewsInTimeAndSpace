using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// NewsEvent
/// 
/// This class represents a singular news event.
/// </summary>
[Serializable]
public class NewsEvent
{
    string id;
    Vector2 location;
    Actor[] actors;
    string type;
    DateTime date;
    string source;
    Count[] counts;
    Media[] media;
    string title;
    string locationName;
    float tone;
    float goldsteinScale;

    /// <summary>
    /// Constructor of a NewsEvent object.
    /// </summary>
    /// <param name="id">Global event id of this NewsEvent</param>
    /// <param name="location">Geolocation of this NewsEvent</param>
    /// <param name="actors">Actors involved in this NewsEvent</param>
    /// <param name="type">Type of action between actors of this NewsEvent</param>
    /// <param name="date">Date this NewsEvent took place</param>
    /// <param name="source">Source of the information on this NewsEvent</param>
    /// <param name="counts">Mentions of counts in the article of this NewsEvent</param>
    /// <param name="media">Media linked to this NewsEvent</param>
    public NewsEvent(string id, Vector2 location, Actor[] actors, string type, DateTime date, string source, Count[] counts, Media[] media, string title, string locationName, float tone, float goldsteinScale)
    {
        this.Id = id;
        this.Location = location;
        this.Actors = actors;
        this.Type = type;
        this.Date = date;
        this.Source = source;
        this.Counts = counts;
        this.Media = media;
        this.Title = title;
        this.LocationName = locationName;
        this.Tone = tone;
        this.GoldsteinScale = goldsteinScale;
    }

    public string Id { get => id; set => id = value; }
    public Vector2 Location { get => location; set => location = value; }
    public Actor[] Actors { get => actors; set => actors = value; }
    public string Type { get => type; set => type = value; }
    public DateTime Date { get => date; set => date = value; }
    public string Source { get => source; set => source = value; }
    public Count[] Counts { get => counts; set => counts = value; }
    public Media[] Media { get => media; set => media = value; }
    public string Title { get => title; set => title = value; }
    public string LocationName { get => locationName; set => locationName = value; }
    public float Tone { get => tone; set => tone = value; }
    public float GoldsteinScale { get => goldsteinScale; set => goldsteinScale = value; }
}

/// <summary>
/// Actor
/// 
/// An actor participating in a NewsEvent.
/// </summary>
[Serializable]
public class Actor
{
    string name;
    Vector2 location;
    string type;
    int count;

    /// <summary>
    /// Constructor of an Actor object.
    /// </summary>
    /// <param name="name">Name of this actor</param>
    /// <param name="location">Location of this actor</param>
    /// <param name="type">Type of this actor</param>
    public Actor(string name, Vector2 location, string type, int count)
    {
        this.Name = name;
        this.Location = location;
        this.Type = type;
        this.Count = count;
    }

    public string Name { get => name; set => name = value; }
    public Vector2 Location { get => location; set => location = value; }
    public string Type { get => type; set => type = value; }
    public int Count { get => count; set => count = value; }
}

/*
 * EventType
 * 
 * This class represents an eventtype of a NewsEvent
 */
[Serializable]
public class EventType
{
    string typeNumber;
    string typeName;
    string baseTypeNumber;
    string baseTypeName;
    int count;

    public EventType(string typeNumber, string typeName, string baseTypeNumber, string baseTypeName, int count = 0)
    {
        this.TypeNumber = typeNumber;
        this.TypeName = typeName;
        this.BaseTypeNumber = baseTypeNumber;
        this.BaseTypeName = baseTypeName;
        this.Count = count;
    }

    public string TypeNumber { get => typeNumber; set => typeNumber = value; }
    public string TypeName { get => typeName; set => typeName = value; }
    public string BaseTypeNumber { get => baseTypeNumber; set => baseTypeNumber = value; }
    public string BaseTypeName { get => baseTypeName; set => baseTypeName = value; }
    public int Count { get => count; set => count = value; }
}

/// <summary>
/// Counts
/// 
/// This class represents mentions of counts (e.g. "five people") in the article of a NewsEvent.
/// </summary>
[Serializable]
public class Count
{
    string type;
    int number;
    Vector2 location;

    /// <summary>
    /// Constructor of a Count object.
    /// </summary>
    /// <param name="type">Type of this count</param>
    /// <param name="count">Number of this count</param>
    /// <param name="location">Location of this count</param>
    public Count(string type, int count, Vector2 location)
    {
        this.Type = type;
        this.Number = count;
        this.Location = location;
    }

    public string Type { get => type; set => type = value; }
    public int Number { get => number; set => number = value; }
    public Vector2 Location { get => location; set => location = value; }
}

/// <summary>
/// Media
/// 
/// This class represents media linked to a NewsEvent.
/// </summary>
[Serializable]
public class Media
{
    string type; // can be "picture", "text", "video"
    string content;

    /// <summary>
    /// Constructor of a Media object.
    /// </summary>
    /// <param name="type">Type of this media (e.g. picture, text, video)</param>
    /// <param name="content">Link to the content of this media</param>
    public Media(string type, string content)
    {
        this.Type = type;
        this.Content = content;
    }

    public string Type { get => type; set => type = value; }
    public string Content { get => content; set => content = value; }
}

/// <summary>
/// Group
/// 
/// This class represents a collection of NewsEvents grouped together by country, region or city.
/// </summary>
[Serializable]
public class Group
{
    int groupID;
    string city;
    string region;
    string country;
    string countryCode;
    Vector2 location;
    string[] eventIDs;
    int[] connections;

    public int EventCount { get => eventIDs.Length; }
    public GroupAggregationLevel AggregationLevel
    {
        get
        {
            GroupAggregationLevel level = GroupAggregationLevel.Country;
            if (Region != "")
            {
                level = GroupAggregationLevel.Region;
            }
            else if (City != "")
            {
                level = GroupAggregationLevel.City;
            }

            return level;
        }
    }

    public int GroupID { get => groupID; set => groupID = value; }
    public string City { get => city; set => city = value; }
    public string Region { get => region; set => region = value; }
    public string Country { get => country; set => country = value; }
    public string CountryCode { get => countryCode; set => countryCode = value; }
    public Vector2 Location { get => location; set => location = value; }
    public string[] EventIDs { get => eventIDs; set => eventIDs = value; }
    public int[] Connections { get => connections; set => connections = value; }

    /// <summary>
    /// Contructor of a Group object.
    /// </summary>
    /// <param name="groupID">ID of this group within the same Server response</param>
    /// <param name="city">City this group corresponds to (might be empty)</param>
    /// <param name="region">Region this group corresponds to (might be empty)</param>
    /// <param name="country">Country this group corresponds to</param>
    /// <param name="countryCode">CountryCode of this group</param>
    /// <param name="location">Location of this group in {longitude, latitude} order</param>
    /// <param name="eventIDs">Global event IDs of events contained in this group</param>
    /// <param name="connections">Connections of this group to others (present if a NewsEvent involved actors from both groups)</param>
    public Group(int groupID, string city, string region, string country, string countryCode, Vector2 location, string[] eventIDs, int[] connections)
    {
        GroupID = groupID;
        City = city;
        Region = region;
        Country = country;
        CountryCode = countryCode;
        Location = location;
        EventIDs = eventIDs;
        Connections = connections;
    }

    public bool Equals(Group obj)
    {
        return obj != null && obj.GroupID == GroupID;

    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Group);
    }

    public static bool operator ==(Group a, Group b)
    {
        if (((object)a) == null || ((object)b) == null)
            return Equals(a, b);

        return a.Equals(b);
    }

    public static bool operator !=(Group a, Group b)
    {
        if (((object)a) == null || ((object)b) == null)
            return !Equals(a, b);

        return !(a.Equals(b));
    }

    public override int GetHashCode()
    {
        return this.groupID;
    }
}

/// <summary>
/// This enum describes the three possible aggregation level of groups, namely country, region and city.
/// </summary>
public enum GroupAggregationLevel
{
    Country = 0,
    Region = 1,
    City = 2
}

/// <summary>
/// This enum describes the three possible viewmodes for the player, namely groups, events and a singular event
/// </summary>
public enum ViewMode
{
    Groups = 0,
    Events = 1,
    Event = 2
}
