using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script demonstrates how to trigger data updates and receive the newest data from a UIDataHandler object.
 * 
 * This funcionality could be embedded into a UI Manager that triggers data updates, fetches the newest data and forwards it to UI elements for visualization.
 */
public class UIDataHandlerSample : MonoBehaviour
{
    public UIDataHandler dataHandler;
    public ScrollableObjectMenu actorsFilterMenu;
    public ScrollableObjectMenu typesFilterMenu;

    private List<string> alphabet = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        // For now an empty filterPool is used
        //FilterPool filterPool = new FilterPool();
        //filterPool.setDates(new System.DateTime(2023, 03, 09), new System.DateTime(2023, 03, 10));
        //dataHandler.updateActors(filterPool); // Move to update and set conditions that trigger an update (e.g. filterPool.HasChanged (would need a reference to MapViewHandler.filterPool))
        alphabet.AddRange(new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" });

    }

    // Update is called once per frame
    void Update()
    {
        /*if (dataHandler.ActorsUpdated && dataHandler.ActorsUnread)
        {
            Actor[] actors = dataHandler.GetActors();

            Debug.Log("Actors size: " + actors.Length);

            List<string> filterValues = new List<string>();
            List<string> filterDisplays = new List<string>();
            List<string> groupValues = new List<string>();
            List<string> groupDisplays = new List<string>();

            foreach (Actor a in actors)
            {
                filterValues.Add(a.Name);
                filterDisplays.Add(a.Name);
            }
            groupValues.AddRange(alphabet);
            groupDisplays.AddRange(alphabet);

            //actorsFilterMenu.UpdateData(filterValues, filterDisplays, groupValues, groupDisplays);


        }*/
    }
}
