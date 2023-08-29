using Org.BouncyCastle.Security;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using System.ComponentModel;
//using UnityEditorInternal;

/// <summary>
/// An interface menu that can be filled with filters, and groups for sorting them in categories. Creates interactable balls for filters and can fill them with extra objects.
/// </summary>
public class ScrollableObjectMenu : MonoBehaviour
{
    // Public
    [Header("Filter UI")]
    public GameObject firstFilterUiElement;
    public Scrollbar filterScrollBar;
    public RectTransform filterViewportRectTransform;
    [Space(10)]
    [Header("Group UI")]
    public GameObject firstGroupUiElement;
    //public Scrollbar groupScrollBar;
    [Space(10)]
    [Header("Player Camera")]
    public GameObject mainCamera;
    [Space(10)]
    [Header("GameObjects")]
    public GameObject genericFilterBall;
    [Space(10)]

    // Private
    private Dictionary<string, string> filters = new Dictionary<string, string>(); // value, display
    private Dictionary<string, string> groups = new Dictionary<string, string>(); // value, display
    private List<string> filterValues = new List<string>(); // order of filters
    private List<string> groupValues = new List<string>(); // order of groups
    private Dictionary<string, GameObject> filterValueToPrefab = new Dictionary<string, GameObject>(); // value, prefab
    private Dictionary<string, GameObject> filterValueToInstance = new Dictionary<string, GameObject>(); // value, instance (ball + prefab or just ball)
    private List<GameObject> uiElementsList = new List<GameObject>(); // current filter ui elements
    private List<RectTransform> uiElementsRectsList = new List<RectTransform>(); // current filter ui elements rects, for better performance
    private Dictionary<string, GameObject> filterValueToUiElement = new Dictionary<string, GameObject>(); // value, UI-Element (GameObject)
    private List<string> valueListFiltersTemp = new List<string>(); // current filter values
    private List<string> displayListFiltersTemp = new List<string>(); // current filter diplays
    private GameObject inactiveBallParent;

    // Start is called before the first frame update
    void Start()
    {
        inactiveBallParent = new GameObject("Inactive Ball Parent");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateInstances();
    }

    /// <summary>
    /// Updates/Replaces all filters and groups in this ui menu.
    /// Dictionaries have format (filterValue, filterDisplay) or (filterValue, Prefab)
    /// </summary>
    /// <param name="newFilters"></param>
    /// <param name="newGroups"></param>
    /// <param name="newFilterValueToPrefab"></param>
    public void UpdateData(Dictionary<string, string> newFilters, Dictionary<string, string> newGroups, Dictionary<string, GameObject> newFilterValueToPrefab)
    {
        filters = newFilters;
        groups = newGroups;
        filterValueToPrefab = newFilterValueToPrefab;
        SortFiltersAndGroups();
        DeleteOldInstances();
        CreateNewInstances();
        UpdateGroups();
        UpdateFiltersOnly();
    }

    /// <summary>
    /// Updates filters in this ui element. Called in UpdateData or from group ui objects when changing the group selection.
    /// </summary>
    public void UpdateFiltersOnly()
    {
        GameObject selectedObject = null;
        foreach (Transform t in firstGroupUiElement.transform.parent)
        {
            if (t.gameObject.GetComponent<Toggle>().isOn && t.gameObject.activeSelf)
            {
                selectedObject = t.gameObject;
                break;
            }
        }
        valueListFiltersTemp.Clear();
        displayListFiltersTemp.Clear();
        for (int i = 0; i < filterValues.Count; i++)
        {
            if ((selectedObject != null && filterValues[i].StartsWith(selectedObject.transform.Find("Value").GetComponent<Text>().text.Trim()))
                || selectedObject == null)
            {
                valueListFiltersTemp.Add(filterValues[i]);
                if (filters[filterValues[i]] != null)
                    displayListFiltersTemp.Add(filters[filterValues[i]]);
                else
                    displayListFiltersTemp.Add(filterValues[i]);
            }
        }
        UpdateFilterUi();
        DeactivateInstances();
    }

    /// <summary>
    /// Deletes old filter ui elements and creates new ones based on what current filters should be.
    /// </summary>
    private void UpdateFilterUi()
    {
        uiElementsList.Clear();
        uiElementsRectsList.Clear();
        filterValueToUiElement.Clear();
        firstFilterUiElement.SetActive(false);
        filterScrollBar.GetComponent<Scrollbar>().value = 1;
        for (int i = 0; i < firstFilterUiElement.transform.parent.childCount; i++)
        {
            if (i == 0)
                continue;
            Destroy(firstFilterUiElement.transform.parent.GetChild(i).gameObject);
        }
        for (int i = 0; i < valueListFiltersTemp.Count; i++)
        {
            if (i == 0)
            {
                firstFilterUiElement.SetActive(true);
                uiElementsList.Add(firstFilterUiElement);
                uiElementsRectsList.Add(firstFilterUiElement.GetComponent<RectTransform>());
                filterValueToUiElement.Add(valueListFiltersTemp[i], firstFilterUiElement);
                continue;
            }
            GameObject uiElement = Instantiate(firstFilterUiElement, firstFilterUiElement.transform.parent);
            uiElementsList.Add(uiElement);
            uiElementsRectsList.Add(uiElement.GetComponent<RectTransform>());
            filterValueToUiElement.Add(valueListFiltersTemp[i], uiElement);
        }
    }

    /// <summary>
    /// Deactivates ball instances that arent visible currently.
    /// </summary>
    private void DeactivateInstances()
    {
        foreach (GameObject obj in filterValueToInstance.Values)
        {
            obj.SetActive(false);
            obj.transform.parent = inactiveBallParent.transform;
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
            //Debug.Log("LONG2");
            if (filterValueToInstance[value].activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(filterViewportRectTransform, uiElementsRectsList[uiElementsList.IndexOf(filterValueToUiElement[value])].position))
            {
                Debug.Log("LONG2 REAL");
                filterValueToInstance[value].SetActive(false);
                filterValueToInstance[value].transform.parent = inactiveBallParent.transform;
            }
            else if (filterValueToInstance[value].activeSelf)
            {
                filterValueToInstance[value].transform.position = filterValueToUiElement[value].transform.position;
                //filterValueToInstance[value].transform.eulerAngles = mainCamera.transform.position - filterValueToInstance[value].transform.position;
                filterValueToInstance[value].transform.rotation = mainCamera.transform.rotation;
            }
        }

        // Create Instances
        if (filterValueToInstance.Any(a => a.Value.activeSelf))
        {
            int startIndex = 0;
            string valueKey = filterValueToInstance.FirstOrDefault(x => x.Value.activeSelf).Key;
            if (valueKey != null)
                startIndex = uiElementsList.IndexOf(filterValueToUiElement[valueKey]);

            int upindex = startIndex;
            while (upindex < uiElementsList.Count)
            {
                var variables = CheckIfUiVisible(upindex, true);
                if (variables.breakLoop)
                    break;
                upindex++;
            }
            int downindex = startIndex;
            while (downindex >= 0)
            {
                var variables = CheckIfUiVisible(downindex, true);
                if (variables.breakLoop)
                    break;
                downindex--;
            }
        }
        else if (valueListFiltersTemp.Count > 0)
        {
            bool hasReachedActivatedElements = false;
            for (int i = 0; i < uiElementsList.Count; i++)
            {
                var variables = CheckIfUiVisible(i, false);
                if (variables.breakLoop)
                    break;
                hasReachedActivatedElements = variables.hasReachedActivatedElements;
            }
        }
    }

    private (bool breakLoop, bool hasReachedActivatedElements) CheckIfUiVisible(int index, bool hasReachedActivatedElements)
    {
        Debug.Log("LONG1");
        if (RectTransformUtility.RectangleContainsScreenPoint(filterViewportRectTransform, uiElementsRectsList[index].position)
               && !filterValueToInstance[filterValueToUiElement.FirstOrDefault(x => x.Value == uiElementsList[index]).Key].activeSelf)
        {
            ActivateInstance(index);
            hasReachedActivatedElements = true;
        }
        else if (!filterValueToInstance[filterValueToUiElement.FirstOrDefault(x => x.Value == uiElementsList[index]).Key].activeSelf && hasReachedActivatedElements)
        {
            return (true, hasReachedActivatedElements);
        }
        return (false, hasReachedActivatedElements);
    }
    
    /// <summary>
    /// Activate ball instance;
    /// </summary>
    /// <param name="i"></param>
    private void ActivateInstance(int i)
    {
        string value = valueListFiltersTemp[i];
        filterValueToInstance[value].SetActive(true);
        filterValueToInstance[value].transform.localScale = transform.root.localScale / 10;
        filterValueToInstance[value].transform.parent = transform;
    }

    /// <summary>
    /// Destroy old groups, create new groups and set parameters of new groups.
    /// </summary>
    private void UpdateGroups()
    {
        // Destroy all group ui elements and deactivate the first one
        firstGroupUiElement.GetComponent<Toggle>().isOn = false;
        firstGroupUiElement.SetActive(false);
        for (int i = 0; i < firstGroupUiElement.transform.parent.childCount; i++)
        {
            if (i == 0)
                continue;
            firstGroupUiElement.transform.parent.GetChild(i).gameObject.SetActive(false);
            Destroy(firstGroupUiElement.transform.parent.GetChild(i).gameObject);
        }

        // Create new group ui elements and activate the first one (depending on new number of groups)
        for (int i = 0; i < groupValues.Count; i++)
        {
            GameObject uiElement;
            if (i == 0)
            {
                uiElement = firstGroupUiElement;
                uiElement.SetActive(true);
            }
            else
                uiElement = Instantiate(firstGroupUiElement, firstGroupUiElement.transform.parent);

            // Set display text as either display text or value text
            if (groups[groupValues[i]] != null)
                uiElement.transform.Find("Display").GetComponent<Text>().text = groups[groupValues[i]];
            else
                uiElement.transform.Find("Display").GetComponent<Text>().text = groupValues[i];

            uiElement.transform.Find("Value").GetComponent<Text>().text = groupValues[i];
        }
    }

    /// <summary>
    /// Sort filters and groups (optional).
    /// </summary>
    private void SortFiltersAndGroups()
    {
        groupValues = groups.Keys.ToList();
        //groupValues.Sort();

        filterValues = filters.Keys.ToList();
        //filterValues.Sort();
    }

    /// <summary>
    /// Create all filter instances and deactivate them.
    /// </summary>
    private void CreateNewInstances()
    {
        foreach (string value in filters.Keys)
        {
            GameObject prefabInstance = Instantiate(genericFilterBall);
            //GameObject prefabInstance = new GameObject("test");
            prefabInstance.SetActive(false);
            prefabInstance.transform.GetChild(0).GetComponent<TextMesh>().text = filters[value];
            filterValueToInstance.Add(value, prefabInstance);
            if (filterValueToPrefab.ContainsKey(value) && filterValueToPrefab[value] != null)
            {
                GameObject childInstance = Instantiate(filterValueToPrefab[value]);
                childInstance.transform.parent = prefabInstance.transform;
                childInstance.transform.position = prefabInstance.transform.position;
                childInstance.transform.rotation = prefabInstance.transform.rotation;
            }
            prefabInstance.transform.localScale *= 1.5f;
            prefabInstance.transform.parent = inactiveBallParent.transform;
        }
    }

    /// <summary>
    /// Delete all instances.
    /// </summary>
    private void DeleteOldInstances()
    {
        foreach (string value in filterValueToInstance.Keys)
        {
            filterValueToInstance[value].SetActive(false);
            Destroy(filterValueToInstance[value]);
        }
        filterValueToInstance.Clear();
    }


}
