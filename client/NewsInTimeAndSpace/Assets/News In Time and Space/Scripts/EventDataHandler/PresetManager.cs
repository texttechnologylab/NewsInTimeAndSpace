using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetManager : MonoBehaviour
{
    public int presetLimit = 5;
    public MapViewHandler mapViewHandler;
    public FilterHandler filterHandler;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Todo: manage selectable preset buttons per preset
    }

    public bool createPreset(FilterPool filterPool, int id = -1)
    {
        // TODO: Test
        FilterPreset preset = new FilterPreset(mapViewHandler.filterPool, filterHandler.filterObjects);
        string save = JsonUtility.ToJson(preset);

        if (id < 0)
        {
            for (int i = 0; i < presetLimit; i++)
            {
                if (!PlayerPrefs.HasKey("FilterPreset" + i))
                {
                    PlayerPrefs.SetString("FilterPreset" + i, save);
                    PlayerPrefs.Save();
                    Debug.Log("Preset " + i + "saved.");
                    return true;
                }
            }
            Debug.Log("Preset not saved! Maybe more than " + presetLimit + " are already saved.");
            return false;
        }
        else
        {
            if (id >= presetLimit) { id = presetLimit - 1; }
            PlayerPrefs.SetString("FilterPreset" + id, save);
            PlayerPrefs.Save();
            Debug.Log("Preset " + id + "saved or overwritten.");
            return true;
        }
    }

    public bool loadPreset(int id)
    {
        // TODO
        string presetJSON = PlayerPrefs.GetString("FilterPreset" + id, "");
        if (presetJSON == "")
        {
            Debug.Log("FilterPreset" + id + " not Found while loading.");
            return false;
        }

        FilterPreset preset = JsonUtility.FromJson<FilterPreset>(presetJSON);
        // Clear currently set filters (FilterHandler for objects, Mapviewhandler.Filterpool for data)

        // Load saved filters:
        /*filterPool.Actors;
        filterPool.EventTypes;
        filterPool.DatesSet;
        filterPool.GoldsteinSet;
        filterPool.LocationSet; // + radius
        filterPool.ToneSet;*/
        return false;
    }
}
