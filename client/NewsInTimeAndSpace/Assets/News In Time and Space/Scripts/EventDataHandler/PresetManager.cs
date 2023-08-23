using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Delete preset
public class PresetManager : MonoBehaviour
{
    public Transform[] presetButtonTransform;
    public MapViewHandler mapViewHandler;
    public FilterHandler filterHandler;
    public Transform filterDetectionBox;
    public GameObject filterBallPrefab;
    public Transform player;
    public ToneSlider toneSlider;
    public GoldsteinScaleSlider goldSteinSlider;
    public DateSlider dateSlider;

    PresetButton[] presetButtonScript;

    void Start()
    {
        presetButtonScript = new PresetButton[presetButtonTransform.Length];
        for (int i = 0; i < presetButtonTransform.Length; i++)
        {
            presetButtonScript[i] = presetButtonTransform[i].GetComponentInChildren<PresetButton>();

            // Initialize buttons for saved presets, hide buttons for empty slots
            string presetJSON = PlayerPrefs.GetString("FilterPreset" + i, "");
            if (presetJSON != "")
            {
                presetButtonScript[i].SetPreset(JsonUtility.FromJson<FilterPreset>(presetJSON), i);
            }
            else
            {
                presetButtonTransform[i].gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Todo: manage selectable preset buttons per preset
    }

    public bool createPreset(int id = -1)
    {
        // TODO: Test
        FilterPreset preset = new FilterPreset(mapViewHandler.filterPool, filterHandler.filterObjects);
        string save = JsonUtility.ToJson(preset);

        if (save == "" || save == "{}")
        {
            Debug.Log("Preset save error: Something went wrong converting the Filters to JSON.");
            return false;
        }

        if (id < 0)
        {
            // Save preset in first free slot
            for (int i = 0; i < presetButtonTransform.Length; i++)
            {
                if (!PlayerPrefs.HasKey("FilterPreset" + i))
                {
                    PlayerPrefs.SetString("FilterPreset" + i, save);
                    PlayerPrefs.Save();
                    presetButtonTransform[i].gameObject.SetActive(true);
                    presetButtonScript[i].SetPreset(preset, i);
                    Debug.Log("Preset " + i + "saved.");
                    return true;
                }
            }
            Debug.Log("Preset not saved! Maybe more than " + presetButtonTransform.Length + " are already saved.");
            return false;
        }
        else
        {
            // Save preset in specific slot
            if (id >= presetButtonTransform.Length) { id = presetButtonTransform.Length - 1; }
            PlayerPrefs.SetString("FilterPreset" + id, save);
            PlayerPrefs.Save();
            presetButtonTransform[id].gameObject.SetActive(true);
            presetButtonScript[id].SetPreset(preset, id);
            Debug.Log("Preset " + id + "saved or overwritten.");
            return true;
        }
    }

    public bool removePreset(int id)
    {
        if (PlayerPrefs.HasKey("FilterPreset" + id))
        {
            PlayerPrefs.DeleteKey("FilterPreset" + id);
            presetButtonTransform[id].gameObject.SetActive(false);
            return true;
        }

        Debug.Log("Tried deleting a non existant Preset: " + id);
        return false;
    }

    public bool loadPreset(int id)
    {
        // Load Preset data
        string presetJSON = PlayerPrefs.GetString("FilterPreset" + id, "");
        if (presetJSON == "")
        {
            Debug.Log("FilterPreset" + id + " not Found while loading.");
            return false;
        }
        FilterPreset preset = JsonUtility.FromJson<FilterPreset>(presetJSON);

        // Remove FilterBalls from FilterHandler and scene
        GameObject[] filterObjects = filterHandler.filterObjects.ToArray();
        foreach (GameObject g in filterObjects)
        {
            filterHandler.RemoveFromHandler(g);
            Destroy(g);
        }

        // Create new FilterBalls
        foreach (FilterBall fb in preset.filterBalls)
        {
            GameObject instance = Instantiate(filterBallPrefab);

            FilterBallObject script = instance.GetComponent<FilterBallObject>();
            script.value = fb.value;
            script.display = fb.display;
            script.location = FilterBallObject.FilterBallLocation.Free;
            script.filterType = (FilterHandler.FilterType)fb.filterType;
            script.filterMenu = null; // TODO: test if used
            script.player = player;
            instance.transform.GetChild(0).GetComponent<TextMesh>().text = fb.display;

            // Add icons to EventType filterBall
            if (script.filterType == FilterHandler.FilterType.eventType)
            {
                int baseType = int.Parse(fb.value.ToString().Substring(0, 2));
                if (baseType <= filterHandler.eventTypeObjects.Count)
                {
                    GameObject icon = Instantiate(filterHandler.eventTypeObjects[baseType - 1], instance.transform);
                    icon.transform.localScale = 0.033f * Vector3.one;
                    icon.transform.localPosition = Vector3.zero;
                    icon.transform.localRotation = Quaternion.Euler(Vector3.zero);
                }
            }

            // TODO: Actor icons

            // Set parent and scale inside filterDetectionBox
            instance.transform.parent = filterDetectionBox;
            instance.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.Euler(Vector3.zero);

            // TODO: Combined filterballs
        }

        // TODO: Set Sliders to values
        FilterPool newFilterPool = preset.toFilterPool();
        toneSlider.setTone(preset.minTone, preset.maxTone);
        dateSlider.setDates(newFilterPool.StartDate, newFilterPool.EndDate);
        goldSteinSlider.setGoldstein(preset.minGoldstein, preset.maxGoldstein);

        // Overwrite old Filterpool
        newFilterPool.HasChanged = true;
        mapViewHandler.filterPool = newFilterPool;
        return false;
    }
}
