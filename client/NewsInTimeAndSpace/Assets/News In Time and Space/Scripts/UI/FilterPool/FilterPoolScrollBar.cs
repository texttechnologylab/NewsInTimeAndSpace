using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class for a FilterDetectionBox / FilterHandler - ScrollBar.
/// Made out of FilterMenu3D script. Works similar but without groups.
/// </summary>
public class FilterPoolScrollBar : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> filterObjects = new List<GameObject>();
    private int filterCurrentIndex = 0;
    private GameObject inactiveBallParent;
    private List<Transform> filterPoints = new List<Transform>();

    public GameObject filterUpArrow;
    public GameObject filterDownArrow;
    public GameObject filterPointsParent;
    public GameObject title;

    // Start is called before the first frame update
    void Start()
    {
        setArrowVariables();

        inactiveBallParent = new GameObject("Inactive Ball Parent");
        inactiveBallParent.transform.parent = transform;

        foreach (Transform t in filterPointsParent.transform)
        {
            filterPoints.Add(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInstances();
    }

    /// <summary>
    /// Updates instance/filter positions.
    /// </summary>
    public void UpdateInstances()
    {
        // Deactivate Instances and update positions
        foreach (GameObject filter in filterObjects)
        {
            if (filter.activeSelf &&
                (filterObjects.IndexOf(filter) < filterCurrentIndex || filterObjects.IndexOf(filter) >= (filterCurrentIndex + filterPoints.Count)))
            {
                filter.SetActive(false);
                filter.transform.parent = inactiveBallParent.transform;
            }
            else if (filter.activeSelf)
            {
                filter.transform.position = filterPoints[filterObjects.IndexOf(filter) - filterCurrentIndex].transform.position;
                filter.transform.rotation = title.transform.rotation;
            }
        }

        // activate instances
        int counter = 0;
        for (int i = 0; i < filterObjects.Count; i++)
        {
            counter++;
            if (i >= filterCurrentIndex && i < (filterCurrentIndex + filterPoints.Count))
            {
                ActivateInstance(filterObjects[i]);
            }
        }
    }

    /// <summary>
    /// Activate filter object instances and set orientation.
    /// </summary>
    /// <param name="value"></param>
    private void ActivateInstance(GameObject filter)
    {
        filter.SetActive(true);
        filter.transform.parent = transform;
        filter.transform.position = filterPoints[filterObjects.IndexOf(filter) - filterCurrentIndex].transform.position;
        filter.transform.rotation = title.transform.rotation;
    }

    /// <summary>
    /// Add new filter object.
    /// </summary>
    /// <param name="filter"></param>
    public void AddFilterObject(GameObject filter)
    {
        filterObjects.Add(filter);
        filter.transform.parent = inactiveBallParent.transform;
    }

    /// <summary>
    /// Remove filter object.
    /// </summary>
    /// <param name="filter"></param>
    public void RemoveFilterObject(GameObject filter)
    {
        filterObjects.Remove(filter);
    }

    /// <summary>
    /// Sets variables of arrow objects.
    /// </summary>
    private void setArrowVariables()
    {
        filterUpArrow.GetComponent<ArrowElement>().setFilterPoolScrollBar(this);
        filterDownArrow.GetComponent<ArrowElement>().setFilterPoolScrollBar(this);
    }

    /// <summary>
    /// Change Indexes via parameters. Called from arrow gameobjects.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="direction"></param>
    public void moveIndexesByArrow(ArrowElement.Type type, ArrowElement.Direction direction)
    {
        if (type == ArrowElement.Type.Filter && direction == ArrowElement.Direction.Down)
        {
            if (filterCurrentIndex < filterObjects.Count - filterPoints.Count)
                filterCurrentIndex += filterPoints.Count;
        }
        else if (type == ArrowElement.Type.Filter && direction == ArrowElement.Direction.Up)
        {
            if (filterCurrentIndex >= filterPoints.Count)
                filterCurrentIndex -= filterPoints.Count;
        }
    }
}
