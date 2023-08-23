using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPresetButton : UIButton
{
    public PresetManager presetManager;

    public override void onRaycastInteraction()
    {
        // TODO: different structure in PresetManager
        presetManager.createPreset();
    }
}
