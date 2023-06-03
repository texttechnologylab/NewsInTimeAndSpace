using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This class provides a script for the FilterMenu3D Prefab.
/// It handles FilterMenu3D data structures and useability.
/// </summary>
public class FilterMenu3D : MonoBehaviour
{

    // Public
    [Header("Filter UI")]
    public GameObject filterPointsParent;
    public GameObject filterUpArrow;
    public GameObject filterDownArrow;
    [Space(10)]
    [Header("Group UI")]
    public GameObject groupPointsParent;
    public GameObject groupUpArrow;
    public GameObject groupDownArrow;
    public GameObject basicGroupElement;
    [Space(10)]
    [Header("Player Camera")]
    public GameObject mainCamera;
    public GameObject player;
    public GameObject title;
    [Space(10)]
    [Header("GameObjects")]
    public GameObject genericFilterBall;
    [Space(10)]

    // Private data lists and dicts
    private Dictionary<string, string> filters = new Dictionary<string, string>(); // value, display
    private Dictionary<string, string> groups = new Dictionary<string, string>(); // value, display
    private List<string> filterValues = new List<string>(); // order of filters
    private List<string> groupValues = new List<string>(); // order of groups
    private Dictionary<string, GameObject> filterValueToPrefab = new Dictionary<string, GameObject>(); // value, prefab
    private Dictionary<string, GameObject> filterValueToInstance = new Dictionary<string, GameObject>(); // value, instance (ball + prefab or just ball)
    private List<string> valueListFiltersTemp = new List<string>(); // current filter values
    private Dictionary<string, GameObject> groupValueToInstance = new Dictionary<string, GameObject>(); // value, group instance
    private Dictionary<GameObject, string> instanceToGroupValue = new Dictionary<GameObject, string>(); // group instance, value

    // private menu formatter variables
    private GameObject ballParent;
    private GameObject groupParent;
    private List<Transform> groupPoints = new List<Transform>();
    private List<Transform> filterPoints = new List<Transform>();
    private int filterCurrentIndex = 0;
    private int groupCurrentIndex = 0;
    private FilterHandler.FilterType filterType;

    // Start is called before the first frame update
    void Start()
    {
        setArrowVariables();

        ballParent = new GameObject("Ball Parent");
        ballParent.transform.parent = transform;
        ballParent.transform.localScale = new Vector3(1, 1, 1);
        groupParent = new GameObject("Group Parent");
        groupParent.transform.parent = transform;
        groupParent.transform.localScale = new Vector3(1, 1, 1);

        foreach (Transform t in filterPointsParent.transform)
        {
            filterPoints.Add(t);
        }
        foreach (Transform t in groupPointsParent.transform)
        {
            groupPoints.Add(t);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        moveIndexesByKeys();
        UpdateInstances();
        UpdateGroups();
    }

    /// <summary>
    /// Updates/Replaces all filters and groups in this ui menu.
    /// Dictionaries have format (filterValue, filterDisplay) or (filterValue, Prefab)
    /// </summary>
    /// <param name="newFilters"></param>
    /// <param name="newGroups"></param>
    /// <param name="newFilterValueToPrefab"></param>
    public void UpdateData(Dictionary<string, string> newFilters, Dictionary<string, string> newGroups, Dictionary<string, GameObject> newFilterValueToPrefab, FilterHandler.FilterType filterType)
    {
        filters = newFilters;
        groups = newGroups;
        filterValueToPrefab = newFilterValueToPrefab;
        filterCurrentIndex = 0;
        groupCurrentIndex = 0;
        this.filterType = filterType;
        SortFiltersAndGroups();
        DeleteOldInstances(); // not delete, only deactivate
        CreateNewInstances(); // only if not already created before
        CreateGroups(); // only if not already created before
        UpdateFiltersOnly();
    }

    /// <summary>
    /// Sort filters and groups, and add to lists for correct order (if needs to change exist).
    /// </summary>
    private void SortFiltersAndGroups()
    {
        groupValues = groups.Keys.ToList();
        //groupValues.Sort();

        filterValues = filters.Keys.ToList();
        //filterValues.Sort();
    }

    /// <summary>
    /// Deactivate all instances.
    /// </summary>
    private void DeleteOldInstances()
    {
        foreach (string value in filterValueToInstance.Keys)
        {
            filterValueToInstance[value].SetActive(false);
            //Destroy(filterValueToInstance[value]);
        }
        //filterValueToInstance.Clear();
    }

    /// <summary>
    /// Create all filter instances and deactivate them.
    /// </summary>
    private void CreateNewInstances()
    {
        foreach (string value in filters.Keys)
        {
            if (!filterValueToInstance.ContainsKey(value))
            {
                GameObject prefabInstance = Instantiate(genericFilterBall);
                prefabInstance.SetActive(false);
                FilterBallObject script = prefabInstance.GetComponent<FilterBallObject>();
                script.value = value;
                script.display = filters[value];
                script.filterType = filterType;
                script.filterMenu = this;
                script.player = player.transform;
                prefabInstance.transform.GetChild(0).GetComponent<TextMesh>().text = filters[value];
                filterValueToInstance.Add(value, prefabInstance);
                if (filterValueToPrefab.ContainsKey(value) && filterValueToPrefab[value] != null)
                {
                    GameObject childInstance = Instantiate(filterValueToPrefab[value]);
                    childInstance.SetActive(true);
                    childInstance.transform.parent = prefabInstance.transform;
                    childInstance.transform.position = prefabInstance.transform.position;
                    childInstance.transform.rotation = prefabInstance.transform.rotation;
                }
                prefabInstance.transform.parent = ballParent.transform;
                prefabInstance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
        }
    }

    /// <summary>
    /// Deactivate old groups, create new groups and set parameters of new groups.
    /// </summary>
    private void CreateGroups()
    {
        // Destroy all group ui elements
        foreach (string value in groupValueToInstance.Keys)
        {
            groupValueToInstance[value].SetActive(false);
            groupValueToInstance[value].GetComponent<GroupElement>().deselectSelf();
            //Destroy(groupValueToInstance[value]);
        }
        //groupValueToInstance.Clear();
        //instanceToGroupValue.Clear();
        groupCurrentIndex = 0;

        // Create new group elements
        for (int i = 0; i < groupValues.Count; i++)
        {
            if (!groupValueToInstance.ContainsKey(groupValues[i]))
            {
                GameObject groupElement = Instantiate(basicGroupElement);
                groupElement.GetComponent<GroupElement>().filterMenu3DScript = this;
                groupValueToInstance.Add(groupValues[i], groupElement);
                instanceToGroupValue.Add(groupElement, groupValues[i]);
                groupElement.transform.parent = groupParent.transform;
                groupElement.transform.localScale = new Vector3(1 / 30f, 1 / 30f, 1 / 30f);

                // Set display text as either display text or value text
                if (groups[groupValues[i]] != null)
                    groupElement.GetComponent<GroupElement>().changeText(groups[groupValues[i]]);
                else
                    groupElement.GetComponent<GroupElement>().changeText(groupValues[i]);
            }
        }
    }

    /// <summary>
    /// Updates filters based on groups. Called in UpdateData or from group ui objects when changing the group selection.
    /// </summary>
    public void UpdateFiltersOnly()
    {
        filterCurrentIndex = 0;
        GameObject selectedObject = null;
        foreach (string value in groupValueToInstance.Keys)
        {
            if (groupValueToInstance[value].GetComponent<GroupElement>().isSelected)
            {
                selectedObject = groupValueToInstance[value];
                break;
            }
        }
        valueListFiltersTemp.Clear();

        for (int i = 0; i < filterValues.Count; i++)
        {
            if ((selectedObject != null && filterValues[i].StartsWith(instanceToGroupValue[selectedObject]))
                || selectedObject == null)
            {
                valueListFiltersTemp.Add(filterValues[i]);
            }
        }
    }

    /// <summary>
    /// Deactivate old ball instances, activate new ball instances and update positions of ball instances.
    /// </summary>
    private void UpdateInstances()
    {
        // Deactivate Instances and update positions
        foreach (string value in filterValueToInstance.Keys)
        {
            if (filterValueToInstance[value].activeSelf &&
                (!valueListFiltersTemp.Contains(value) || valueListFiltersTemp.IndexOf(value) < filterCurrentIndex || valueListFiltersTemp.IndexOf(value) >= (filterCurrentIndex + filterPoints.Count)))
            {
                filterValueToInstance[value].SetActive(false);
                filterValueToInstance[value].transform.parent = ballParent.transform;
            }
            else if (filterValueToInstance[value].activeSelf)
            {
                filterValueToInstance[value].transform.position = filterPoints[valueListFiltersTemp.IndexOf(value) - filterCurrentIndex].transform.position;
                filterValueToInstance[value].transform.rotation = title.transform.rotation;
                //filterValueToInstance[value].transform.LookAt(getPointToLookAt(filterValueToInstance[value].transform, mainCamera.transform), Vector3.up);
            }
        }

        // activate instances
        int counter = 0;
        for (int i = 0; i < valueListFiltersTemp.Count; i++)
        {
            counter++;
            string value = valueListFiltersTemp[i];
            if (i >= filterCurrentIndex && i < (filterCurrentIndex + filterPoints.Count))
            {
                ActivateInstance(value);
            }
        }
    }

    /// <summary>
    /// Activate Ball instances and set orientation.
    /// </summary>
    /// <param name="value"></param>
    private void ActivateInstance(string value)
    {
        filterValueToInstance[value].SetActive(true);
        filterValueToInstance[value].transform.position = filterPoints[valueListFiltersTemp.IndexOf(value) - filterCurrentIndex].transform.position;
        filterValueToInstance[value].transform.rotation = title.transform.rotation;
        //filterValueToInstance[value].transform.LookAt(getPointToLookAt(filterValueToInstance[value].transform, mainCamera.transform));
    }

    /// <summary>
    /// Activate Group instances and set orientation.
    /// </summary>
    /// <param name="value"></param>
    private void ActivateGroupInstance(string value)
    {
        groupValueToInstance[value].SetActive(true);
        groupValueToInstance[value].transform.position = groupPoints[groupValues.IndexOf(value) - groupCurrentIndex].transform.position;
        groupValueToInstance[value].transform.rotation = title.transform.rotation;
        //groupValueToInstance[value].transform.LookAt(getPointToLookAt(groupValueToInstance[value].transform, mainCamera.transform));
    }

    /// <summary>
    /// Deactivate old group instances, activate new group instances and update positions of group instances.
    /// </summary>
    private void UpdateGroups()
    {
        // Activate group instances and positions
        foreach (string value in groupValueToInstance.Keys)
        {
            if (groupValueToInstance[value].activeSelf &&
                (!groupValues.Contains(value) || groupValues.IndexOf(value) < groupCurrentIndex || groupValues.IndexOf(value) >= (groupCurrentIndex + groupPoints.Count)))
            {
                groupValueToInstance[value].SetActive(false);
                groupValueToInstance[value].transform.parent = groupParent.transform;
            }
            else if (groupValueToInstance[value].activeSelf)
            {
                groupValueToInstance[value].transform.position = groupPoints[groupValues.IndexOf(value) - groupCurrentIndex].transform.position;
                groupValueToInstance[value].transform.rotation = title.transform.rotation;
                //groupValueToInstance[value].transform.LookAt(getPointToLookAt(groupValueToInstance[value].transform, mainCamera.transform));
            }
        }

        // activate group instances
        foreach (string value in groupValues)
        {
            if (groupValues.IndexOf(value) >= groupCurrentIndex && groupValues.IndexOf(value) < (groupCurrentIndex + groupPoints.Count))
            {
                ActivateGroupInstance(value);
            }
        }
    }

    /// <summary>
    /// Deactivate isSelected variable in all group instances and activate in newlySelected. Also calls itself to update filters.
    /// </summary>
    /// <param name="newlySelected"></param>
    public void changeGroupSelection(GameObject newlySelected)
    {
        foreach (GameObject g in groupValueToInstance.Values)
        {
            if (g != newlySelected)
                g.GetComponent<GroupElement>().changeState(false);
            else
                g.GetComponent<GroupElement>().changeState(true);
        }
        UpdateFiltersOnly();
    }

    /// <summary>
    /// Change Indexes via parameters (called from arrow gameobjects).
    /// </summary>
    /// <param name="type"></param>
    /// <param name="direction"></param>
    public void moveIndexesByArrow(ArrowElement.Type type, ArrowElement.Direction direction)
    {
        if (type == ArrowElement.Type.Filter && direction == ArrowElement.Direction.Down)
        {
            if (filterCurrentIndex < valueListFiltersTemp.Count - filterPoints.Count)
                filterCurrentIndex += filterPoints.Count;
        }
        else if (type == ArrowElement.Type.Filter && direction == ArrowElement.Direction.Up)
        {
            if (filterCurrentIndex >= filterPoints.Count)
                filterCurrentIndex -= filterPoints.Count;
        }
        if (type == ArrowElement.Type.Group && direction == ArrowElement.Direction.Down)
        {
            if (groupCurrentIndex < groupValues.Count - groupPoints.Count)
                groupCurrentIndex += groupPoints.Count;
        }
        else if (type == ArrowElement.Type.Group && direction == ArrowElement.Direction.Up)
        {
            if (groupCurrentIndex >= groupPoints.Count)
                groupCurrentIndex -= groupPoints.Count;
        }
    }

    /// <summary>
    /// Change Indexes via predefined keys.
    /// </summary>
    private void moveIndexesByKeys()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (filterCurrentIndex < valueListFiltersTemp.Count - filterPoints.Count)
                filterCurrentIndex += filterPoints.Count;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            if (filterCurrentIndex >= filterPoints.Count)
                filterCurrentIndex -= filterPoints.Count;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (groupCurrentIndex < groupValues.Count - groupPoints.Count)
                groupCurrentIndex += groupPoints.Count;
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            if (groupCurrentIndex >= groupPoints.Count)
                groupCurrentIndex -= groupPoints.Count;
        }
    }

    /// <summary>
    /// Sets variables of arrow objects.
    /// </summary>
    private void setArrowVariables()
    {
        filterUpArrow.GetComponent<ArrowElement>().setFilterMenu3d(this);
        filterDownArrow.GetComponent<ArrowElement>().setFilterMenu3d(this);
        groupUpArrow.GetComponent<ArrowElement>().setFilterMenu3d(this);
        groupDownArrow.GetComponent<ArrowElement>().setFilterMenu3d(this);
    }

    /// <summary>
    /// Activates from filterBallObject when grabbing filterBallObject from FilterMenu3D.
    /// Sets newObject to be inside FilterMenu3D instead of oldObject. (only works when newObject is copy of oldObject)
    /// </summary>
    /// <param name="oldObject"></param>
    /// <param name="newObject"></param>
    public void changeFilterObject(GameObject oldObject, GameObject newObject)
    {
        string value = oldObject.GetComponent<FilterBallObject>().value;
        if (value == null)
        {
            Debug.Log("Object does not have a filter value");
            return;
        }
        if (filterValues.Contains(value))
        {
            filterValueToInstance[value] = newObject;
            newObject.transform.parent = ballParent.transform;
            newObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }
    }
}
