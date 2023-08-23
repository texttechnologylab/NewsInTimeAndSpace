using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePresetButton : UIButton
{
    public PresetButton presetButton;

    public override void onRaycastInteraction()
    {
        presetButton.RemovePreset();
    }
}
