using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetButton : UIButton
{
    public PresetManager presetManager;
    public TextMesh text;
    public int maxLineWidth = 30;

    FilterPreset preset;
    int index;

    FilterPreset Preset
    {
        get => preset;
        set
        {
            preset = value;

            // Update Text
            string eventTypes = "Event types: ";
            int lineCount = 0;
            for (int i = 0; i < preset.eventTypes.Count && lineCount < 9; i++)
            {
                if (eventTypes.Length + preset.eventTypes[i].Length < maxLineWidth)
                {
                    eventTypes += preset.eventTypes[i] + " ";
                }
                else
                {
                    eventTypes += "\n";
                    lineCount++;
                }
            }
            if (!eventTypes.EndsWith("\n"))
            {
                eventTypes += "\n";
            }

            string actors = "Actors: ";
            for (int i = 0, lineLength = actors.Length; i < preset.actors.Count && lineCount < 9; i++)
            {
                if (lineLength + preset.actors[i].Length < maxLineWidth)
                {
                    actors += preset.actors[i] + " ";
                    lineLength += preset.actors[i].Length;
                }
                else
                {
                    actors += "\n";
                    lineCount++;
                    lineLength = 0;
                }
            }

            text.text = preset.startDate + " - " + preset.endDate + "\n" + eventTypes + actors;
        }
    }

    public void SetPreset(FilterPreset preset, int index)
    {
        this.Preset = preset;
        this.index = index;
    }

    public void RemovePreset()
    {
        presetManager.removePreset(index);
    }

    public override void onRaycastInteraction()
    {
        presetManager.loadPreset(index);
    }
}
