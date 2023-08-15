using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPresetButton : UIButton
{
    public PresetManager presetManager;
    public MapViewHandler mapViewHandler;

    public override void onRaycastInteraction()
    {
        presetManager.createPreset(mapViewHandler.filterPool);
    }
}
