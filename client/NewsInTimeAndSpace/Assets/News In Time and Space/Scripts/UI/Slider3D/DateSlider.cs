using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Converts slider values (floats) into DateTimes by subtracting the number of days from today.
/// Requires MaxValueLimit to be 0 (otherwise dates could be in the future).
/// Allowed multiples should be 1 because this class uses the sliders values as integers.
/// </summary>
public class DateSlider : MonoBehaviour
{
    public Slider3D slider;
    public MapViewHandler mapViewHandler;
    public SliderGraspable minSlider;
    public GameObject minSliderScalableButton;
    public SliderGraspable maxSlider;
    public GameObject maxSliderScalableButton;

    TMPro.TMP_Text minText;
    TMPro.TMP_Text maxText;

    public DateTime getEndDate()
    {
        int days = (int)slider.getMaxValue();
        return DateTime.Today.AddDays(days);
    }

    public DateTime getStartDate()
    {
        int days = (int)slider.getMinValue();
        return DateTime.Today.AddDays(days);
    }

    private void Start()
    {
        minText = minSliderScalableButton.GetComponentInChildren<TMPro.TMP_Text>();
        minSliderScalableButton.SetActive(false);
        maxText = maxSliderScalableButton.GetComponentInChildren<TMPro.TMP_Text>();
        maxSliderScalableButton.SetActive(false);
    }

    private void Update()
    {
        // Trigger data update on release of sliders
        if (slider.MinValueHasChanged || slider.MaxValueHasChanged)
        {
            mapViewHandler.filterPool.setDates(getStartDate(), getEndDate());
        }

        // Show slider value above slider while grasping
        if (minSlider.Grasped)
        {
            minSliderScalableButton.SetActive(true);
            minText.text = getStartDate().ToString("dd.MM.yyyy");
        }
        else
        {
            minSliderScalableButton.SetActive(false);
        }

        if (maxSlider.Grasped)
        {
            maxSliderScalableButton.SetActive(true);
            maxText.text = getEndDate().Date.ToString("dd.MM.yyyy");
        }
        else
        {
            maxSliderScalableButton.SetActive(false);
        }
    }
}
