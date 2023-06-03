using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;
/// <summary>
/// Class for handling FilterHandler and FilterDetectionBox Objects and managing selected filter objects inside FilterHandler.
/// </summary>
public class FilterHandler : MonoBehaviour
{
    public enum FilterType
    {
        actor,
        eventType
    };
    [Header("Handlers")]
    public MapViewHandler mapViewHandler;
    public UIDataHandler dataHandler;
    [Header("Menues")]
    public FilterMenu3D actorFilter;
    public FilterMenu3D eventTypeFilter;
    public GameObject filterDetection;
    public GameObject eventTypeScrollBar;
    public GameObject actorScrollBar;
    public GameObject dateFilterUIObject;
    [Header("GameObjects")]
    public List<GameObject> eventTypeObjects = new List<GameObject>();
    public GameObject genericActorObject;
    public GameObject flagObject;
    [Header("Text Based")]
    private CountryCodeTranslation countryCodeTranslation;
    public TextAsset countryCodesNamesJson;
    private List<string> alphabet = new List<string>();
    public string pathToFlagFolder = "Assets/News In Time and Space/CountryFlags";
    private Dictionary<string, GameObject> temporaryDictOfFlags = new Dictionary<string, GameObject>();

    // other private
    [HideInInspector]
    public List<GameObject> filterObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        alphabet.AddRange(new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" });

        CountriesToCodes countriesToCodes;
        countriesToCodes = JsonUtility.FromJson<CountriesToCodes>(countryCodesNamesJson.text);
        countryCodeTranslation = new CountryCodeTranslation();
        this.countryCodeTranslation.CountryCodesList = countriesToCodes.list;
    }

    // Update is called once per frame
    /// <summary>
    /// This Update Method checks if actor filters or eventType filters have been updated but not yet handled.
    /// If true, creates data structures for FilterMenu3D objects.
    /// </summary>
    void Update()
    {
        if (dataHandler.ActorsUpdated && dataHandler.ActorsUnread)
        {
            Actor[] actors = dataHandler.GetActors();

            Debug.Log("Actors size: " + actors.Length);

            Dictionary<string, string> filters = new Dictionary<string, string>();
            Dictionary<string, string> groups = new Dictionary<string, string>();
            Dictionary<string, GameObject> filterToPrefab = new Dictionary<string, GameObject>();

            foreach (Actor a in actors)
            {
                filters.Add(a.Name, a.Name);
                if (countryCodeTranslation.GetCountryCode(a.Name) != "" && !temporaryDictOfFlags.ContainsKey(a.Name))
                {
                    //Assets / News In Time and Space/ CountryFlags / us.png
                    Texture texture = Resources.Load<Texture>("CountryFlags/" + countryCodeTranslation.GetCountryCode(a.Name));
                    if (texture != null)
                    {
                        GameObject flagObject = Instantiate(this.flagObject);
                        flagObject.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = texture;
                        flagObject.transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = texture;
                        flagObject.SetActive(false);
                        flagObject.transform.parent = transform;
                        temporaryDictOfFlags.Add(a.Name, flagObject);
                        filterToPrefab.Add(a.Name, flagObject);
                        continue;
                    }
                }
                filterToPrefab.Add(a.Name, genericActorObject);
            }
            foreach (string letter in alphabet)
            {
                groups.Add(letter, letter);
            }
            actorFilter.UpdateData(filters, groups, filterToPrefab, FilterType.actor);
        }

        if (dataHandler.EventTypesUpdated && dataHandler.EventTypesUnread)
        {
            EventType[] eventTypes = dataHandler.GetEventTypes();

            Debug.Log("EventTypes size: " + eventTypes.Length);

            Dictionary<string, string> filters = new Dictionary<string, string>();
            Dictionary<string, string> groups = new Dictionary<string, string>();
            Dictionary<string, GameObject> filterToPrefab = new Dictionary<string, GameObject>();

            foreach (EventType a in eventTypes)
            {
                filters.Add(a.TypeNumber, a.TypeName);
                if (!groups.ContainsKey(a.BaseTypeNumber))
                    groups.Add(a.BaseTypeNumber, a.BaseTypeName);
                int intValue;
                if (int.TryParse(a.BaseTypeNumber, out intValue) && eventTypeObjects.Count > 0
                    && int.Parse(a.BaseTypeNumber) >= 0 && int.Parse(a.BaseTypeNumber) < eventTypeObjects.Count)
                    filterToPrefab.Add(a.TypeNumber, eventTypeObjects[int.Parse(a.BaseTypeNumber) - 1]);
            }
            eventTypeFilter.UpdateData(filters, groups, filterToPrefab, FilterType.eventType);
        }
    }

    /// <summary>
    /// Method for trying to add new filter into FilterHandler.
    /// </summary>
    /// <param name="filterObject"></param>
    public void TryAddFilter(GameObject filterObject)
    {
        FilterBallObject scriptComponent = filterObject.GetComponent<FilterBallObject>();
        if (scriptComponent != null && scriptComponent.Location == FilterBallObject.FilterBallLocation.Free && !filterObjects.Contains(filterObject.gameObject))
        {
            scriptComponent.Location = FilterBallObject.FilterBallLocation.InFilterPool;
            filterObjects.Add(filterObject.gameObject);
            switch (scriptComponent.filterType)
            {
                case FilterType.actor:
                    //actors.Add(new Tuple<string, string>(scriptComponent.value, null));
                    actorScrollBar.GetComponent<FilterPoolScrollBar>().AddFilterObject(filterObject);
                    mapViewHandler.filterPool.addActor(scriptComponent.value);
                    break;
                case FilterType.eventType:
                    //eventTypes.Add(scriptComponent.value);
                    eventTypeScrollBar.GetComponent<FilterPoolScrollBar>().AddFilterObject(filterObject);
                    mapViewHandler.filterPool.addEventType(scriptComponent.value);
                    break;
            }
            filterObject.GetComponent<Rigidbody>().isKinematic = true;
            filterObject.GetComponent<FilterBallObject>().MyfilterHandler = this;
        }
    }

    /// <summary>
    /// Method for removing filter from FilterHandler.
    /// </summary>
    /// <param name="filterObject"></param>
    public void RemoveFromHandler(GameObject filterObject)
    {
        string value = filterObject.GetComponent<FilterBallObject>().value;
        FilterBallObject scriptComponent = filterObject.GetComponent<FilterBallObject>();
        switch (scriptComponent.filterType)
        {
            case FilterType.actor:
                //actors.RemoveAll(x => x.Item1 == scriptComponent.value || x.Item2 == scriptComponent.value);
                actorScrollBar.GetComponent<FilterPoolScrollBar>().RemoveFilterObject(filterObject);
                mapViewHandler.filterPool.removeActor(scriptComponent.value);
                break;
            case FilterType.eventType:
                //eventTypes.RemoveAll(x => x == scriptComponent.value);
                eventTypeScrollBar.GetComponent<FilterPoolScrollBar>().RemoveFilterObject(filterObject);
                mapViewHandler.filterPool.removeEventType(scriptComponent.value);
                break;
        }
        filterObjects.Remove(filterObject);
    }
}
