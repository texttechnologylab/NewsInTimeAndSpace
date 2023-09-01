using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearFilterButton : UIButton
{
    public PresetManager presetManager;

    public override void onRaycastInteraction()
    {
        presetManager.clearFilters();
    }
}
