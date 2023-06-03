using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToneSlider : MonoBehaviour
{
    public Slider3D slider;
    public MapViewHandler mapViewHandler;
    public SliderGraspable minSlider;
    public GameObject minSliderScalableButton;
    public SliderGraspable maxSlider;
    public GameObject maxSliderScalableButton;

    TMPro.TMP_Text minText;
    TMPro.TMP_Text maxText;

    public float getMaxTone()
    {
        return slider.getMaxValue();
    }

    public float getMinTone()
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
            mapViewHandler.filterPool.setTone(getMinTone(), getMaxTone());
        }

        // Show slider value above slider while grasping
        if (minSlider.Grasped)
        {
            minSliderScalableButton.SetActive(true);
            minText.text = getMinTone().ToString();
        }
        else
        {
            minSliderScalableButton.SetActive(false);
        }

        if (maxSlider.Grasped)
        {
            maxSliderScalableButton.SetActive(true);
            maxText.text = getMaxTone().ToString();
        }
        else
        {
            maxSliderScalableButton.SetActive(false);
        }
    }
}
