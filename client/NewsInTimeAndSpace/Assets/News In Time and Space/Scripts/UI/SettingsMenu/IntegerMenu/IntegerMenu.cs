using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntegerMenu : MonoBehaviour
{
    public HandRotationMenu HandRotationMenu;
    public TextMesh text;
    public int max;
    public int min;
    public int step;
    int value;

    public int Value
    {
        get => value;
        set
        {
            value = (value > max) ? min : value;
            value = (value < min) ? max : value;
            this.value = value;
            text.text = value.ToString();
        }
    }

    public void increaseValue()
    {
        Value += step;
        HandRotationMenu.updateHandRotation();
    }

    public void decreaseValue()
    {
        Value -= step;
        HandRotationMenu.updateHandRotation();
    }
}
