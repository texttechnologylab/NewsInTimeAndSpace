using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * VisualizerManager
 * 
 * This class manages (de-)activation and instantiation of visualizer Objects.
 * Objects are deactivated so they don't consume resources but aren't destroyed and reinstantiated to save performance.
 */
public class VisualizerManager : MonoBehaviour
{
    public GameObject prefab;

    public List<GameObject> active;
    public List<GameObject> inactive;

    private void Start()
    {
        this.active = new List<GameObject>();
        this.inactive = new List<GameObject>();
    }

    /*
     * Sets a given number of visualizers to active GameObjects and instantiates more if necessary.
     * If less visualizers are necessary than are present the rest will be deactivated for later use.
     * @param count     The number of visualizers to be active after execution
     */
    public void setActiveVisualizers(int count)
    {
        if (count < active.Count)
        {
            deactivateVisualizers(active.Count - count);
        }
        else if (count > active.Count)
        {
            activateVisualizers(count - active.Count);
        }
    }

    /*
     * (Re-)acivates a given number of visualizers and instantiates new ones if necessary
     * @param count     The number of visualizers to activate
     */
    void activateVisualizers(int count)
    {
        // Reactivate inactive visualizers
        while (inactive.Count > 0)
        {
            if (count > 0)
            {
                GameObject v = inactive[0];
                v.SetActive(true);
                inactive.Remove(v);
                active.Add(v);

                count--;
            }
            else
            {
                break;
            }
        }

        // Instantiate remaining visualizers if necessary
        for (int i = 0; i < count; i++)
        {
            GameObject v = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            active.Add(v);
        }
    }

    /*
     * Deacitvates a given number of visualizers.
     * @param count     The number of visualizers to deactivate
     */
    void deactivateVisualizers(int count)
    {
        while (active.Count > 0)
        {
            if (count > 0)
            {
                GameObject v = active[0];
                v.SetActive(false);
                active.Remove(v);
                inactive.Add(v);

                count--;
            }
            else
            {
                break;
            }
        }
    }
}
