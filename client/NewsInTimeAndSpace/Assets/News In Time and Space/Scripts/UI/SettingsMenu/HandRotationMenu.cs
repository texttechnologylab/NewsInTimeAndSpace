using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRotationMenu : MonoBehaviour
{
    public Settings settings;
    public IntegerMenu xMenu;
    public IntegerMenu yMenu;
    public IntegerMenu zMenu;

    public void Start()
    {
        Vector3 rotation = settings.HandRotationOffset;
        xMenu.Value = Mathf.RoundToInt(rotation.x);
        yMenu.Value = Mathf.RoundToInt(rotation.y);
        zMenu.Value = Mathf.RoundToInt(rotation.z);
    }

    public void updateHandRotation()
    {
        settings.HandRotationOffset = new Vector3(xMenu.Value, yMenu.Value, zMenu.Value);
    }
}
