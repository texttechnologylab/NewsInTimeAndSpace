using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldsteinScaleSlider : MonoBehaviour
{
    public Slider3D slider;
    public MapViewHandler mapViewHandler;
    public SliderGraspable minSlider;
    public GameObject minSliderScalableButton;
    public SliderGraspable maxSlider;
    public GameObject maxSliderScalableButton;

    TMPro.TMP_Text minText;
    TMPro.TMP_Text maxText;

    public float getMaxGoldstein()
    {
        return slider.getMaxValue();
    }

    public float getMinGoldstein()
    {
        return slider.getMinValue();
    }

    private void Start()
    {
        minText = minSliderScalableButton.GetComponentInChildren<TMPro.TMP_Text>();
        minSliderScalableButton.SetActive(false);
        maxText = maxSliderScalableButton.GetComponentInChildren<TMPro.TMP_Text>();
        maxSliderScalableButton.SetActive(false);
    }

    void Update()
    {
        if (slider.MinValueHasChanged || slider.MaxValueHasChanged)
        {
            mapViewHandler.filterPool.setGoldstein(getMinGoldstein(), getMaxGoldstein());
        }

        // Show slider value above slider while grasping
        if (minSlider.Grasped)
        {
            minSliderScalableButton.SetActive(true);
            minText.text = getMinGoldstein().ToString();
        }
        else
        {
            minSliderScalableButton.SetActive(false);
        }

        if (maxSlider.Grasped)
        {
            maxSliderScalableButton.SetActive(true);
            maxText.text = getMaxGoldstein().ToString();
        }
        else
        {
            maxSliderScalableButton.SetActive(false);
        }
    }
}
