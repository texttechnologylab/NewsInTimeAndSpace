using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : UIButton
{
    public GameObject settingsMenu;

    public void Start()
    {
        settingsMenu.SetActive(false);
    }

    public override void onRaycastInteraction()
    {
        if (settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
        }
        else
        {
            settingsMenu.SetActive(true);
        }
    }
}
