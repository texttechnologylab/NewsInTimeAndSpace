using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Move slider objects in their graspable script

/// <summary>
/// Slider3D
/// 
/// This class handles visualization and logic for a slider Element with two sliders for min and max values.
/// It uses float values for the slider so another Script for Conversion is needed to set up this slider and convert float values to the desired types.
/// The slider objects are moved from their own scripts using updateMinSliderIndex() and updateMaxSliderIndex().
/// </summary>
public class Slider3D : MonoBehaviour
{
    [Header("Object references")]
    public Transform minHandle;
    public Transform maxHandle;
    public Transform minSlider;
    public Transform maxSlider;
    public Transform sliderBase;
    public Transform selectedArea;
    [Header("Slider values")]
    [SerializeField] private float minValueLimit;
    [SerializeField] private float maxValueLimit;
    [SerializeField] private float allowedMultiples;
    [SerializeField] private float defaultMin;
    [SerializeField] private float defaultMax;
    [Header("Display values")]
    public float sliderBaseThickness;
    public float selectedAreaThickness;
    public float sliderSize;
    [Range(0f, 0.5f)]
    public float sliderOffset; // Offsets the sliders to avoid clipping

    // These values should always be correct / allowed values
    int allowedValueCount;
    float[] allowedValues;
    int minSliderIndex;
    int maxSliderIndex;

    // Update triggers: true until corresponding update is completed
    bool minSliderIndexChanged;
    bool maxSliderIndexChanged;
    bool allowedValuesParameterChanged;

    // Variables to trigger updates
    Vector3 oldMinHandlePos;
    Vector3 oldMaxHandlePos;
    float oldSliderBaseThickness;
    float oldSelectedAreaThickness;
    float oldSliderSize;

    // Flags to inform outside components about value changes: true on change until values get read
    bool minValueHasChanged;
    bool maxValueHasChanged;

    public float MinValueLimit
    {
        get => minValueLimit;
        set
        {
            minValueLimit = value;
            allowedValuesParameterChanged = true;
        }
    }
    public float MaxValueLimit
    {
        get => maxValueLimit;
        set
        {
            maxValueLimit = value;
            allowedValuesParameterChanged = true;
        }
    }
    public float AllowedMultiples
    {
        get => allowedMultiples;
        set
        {
            allowedMultiples = value;
            allowedValuesParameterChanged = true;
        }
    }
    public float DefaultMin
    {
        get => defaultMin;
        set
        {
            defaultMin = value;
            allowedValuesParameterChanged = true;
        }
    }
    public float DefaultMax
    {
        get => defaultMax;
        set
        {
            defaultMax = value;
            allowedValuesParameterChanged = true;
        }
    }

    public bool MinValueHasChanged
    {
        get => minValueHasChanged;
        set => minValueHasChanged = value;
    }

    public bool MaxValueHasChanged
    {
        get => maxValueHasChanged;
        set => maxValueHasChanged = value;
    }

    public float getMinValue()
    {
        minValueHasChanged = false;
        return allowedValues[minSliderIndex];
    }

    public float getMaxValue()
    {
        maxValueHasChanged = false;
        return allowedValues[maxSliderIndex];
    }

    // Start is called before the first frame update
    void Start()
    {
        updateSliderBase();
        updateSliderSize();
        updateAllowedValues();
        updateMaxSliderIndexByValue(DefaultMax);
        updateMinSliderIndexByValue(DefaultMin);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: updateSliderBase() if handle positions or display values change (don't use transform.haschanged bc its almost constantly true)
        if (minHandle.position != oldMinHandlePos || maxHandle.position != oldMaxHandlePos)
        {
            updateSliderBase();
            updateMaxSliderPosition();
            updateMinSliderPosition();
        }

        if (sliderBaseThickness != oldSliderBaseThickness)
        {
            updateSliderBase();
        }

        if (selectedAreaThickness != oldSelectedAreaThickness)
        {
            updateSelectedArea();
        }

        if (sliderSize != oldSliderSize)
        {
            updateSliderSize();
        }

        if (allowedValuesParameterChanged)
        {
            updateAllowedValues();
            minValueHasChanged = true;
            maxValueHasChanged = true;
        }

        if (maxSliderIndexChanged)
        {
            updateMaxSliderPosition();
            maxSliderIndexChanged = false;
        }
        if (minSliderIndexChanged)
        {
            updateMinSliderPosition();
            minSliderIndexChanged = false;
        }

        // Debug
        if (Input.GetKeyDown(KeyCode.O))
        {
            updateMaxSliderIndexByValue(allowedValues[maxSliderIndex] + AllowedMultiples);
            Debug.Log("Minslider: " + allowedValues[minSliderIndex] + " Maxslider: " + allowedValues[maxSliderIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            updateMaxSliderIndexByValue(allowedValues[maxSliderIndex] - AllowedMultiples);
            Debug.Log("Minslider: " + allowedValues[minSliderIndex] + " Maxslider: " + allowedValues[maxSliderIndex]);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            updateMinSliderIndexByValue(allowedValues[minSliderIndex] + AllowedMultiples);
            Debug.Log("Minslider: " + allowedValues[minSliderIndex] + " Maxslider: " + allowedValues[maxSliderIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            updateMinSliderIndexByValue(allowedValues[minSliderIndex] - AllowedMultiples);
            Debug.Log("Minslider: " + allowedValues[minSliderIndex] + " Maxslider: " + allowedValues[maxSliderIndex]);
        }
        // debug end

        oldMinHandlePos = minHandle.position;
        oldMaxHandlePos = maxHandle.position;
        oldSliderBaseThickness = sliderBaseThickness;
        oldSelectedAreaThickness = selectedAreaThickness;
        oldSliderSize = sliderSize;
    }

    void updateAllowedValues()
    {
        float oldMaxSliderValue;
        float oldMinSliderValue;

        if (allowedValues != null)
        {
            oldMaxSliderValue = allowedValues[maxSliderIndex];
            oldMinSliderValue = allowedValues[minSliderIndex];
        }
        else
        {
            oldMaxSliderValue = DefaultMax;
            oldMinSliderValue = DefaultMin;
        }

        // Recalculate allowed values
        allowedValueCount = Mathf.FloorToInt((MaxValueLimit - MinValueLimit) / AllowedMultiples) + 1;
        allowedValues = new float[allowedValueCount];

        for (int i = 0; i < allowedValueCount; i++)
        {
            allowedValues[i] = (MaxValueLimit - MinValueLimit) * ((float)i / (float)(allowedValueCount - 1)) + MinValueLimit;
        }

        // Update slider indexes & positions for new values
        updateMaxSliderIndexByValue(oldMaxSliderValue, ignoreMinSlider: true);
        updateMinSliderIndexByValue(oldMinSliderValue);

        updateMaxSliderPosition();
        updateMinSliderPosition();
    }

    /// <summary>
    /// Sets the minSlider position along the sliderBase according to its index.
    /// </summary>
    void updateMinSliderPosition()
    {
        minSlider.position = Vector3.Lerp(minHandle.position, maxHandle.position, (float)minSliderIndex / (float)(allowedValueCount - 1) - sliderOffset);
        updateSelectedArea();
    }

    /// <summary>
    /// Sets the maxSlider position along the sliderBase according to its index.
    /// </summary>
    void updateMaxSliderPosition()
    {
        maxSlider.position = Vector3.Lerp(minHandle.position, maxHandle.position, (float)maxSliderIndex / (float)(allowedValueCount - 1) + sliderOffset);
        updateSelectedArea();
    }

    /// <summary>
    /// Calculates the index of nearest allowed value for the minSlider between 0 and maxSliderIndex.
    /// 
    /// Assumes a correct maxSliderIndex (call after updateMaxAllowedIndex() if not guaranteed).
    /// </summary>
    /// <param name="value">Percentage (0-1) of the minSlider position along the slider</param>
    void updateMinSliderIndex(float value)
    {
        value = Mathf.Clamp01(value);

        // Calculate the index of the nearest allowed value
        int oldMinSliderIndex = minSliderIndex;
        minSliderIndex = Mathf.RoundToInt(value * (float)(allowedValueCount - 1));

        // Clamp min index to max index
        if (minSliderIndex > maxSliderIndex)
        {
            minSliderIndex = maxSliderIndex;
        }

        // Set flags for updates
        if (minSliderIndex != oldMinSliderIndex)
        {
            minSliderIndexChanged = true;
        }
    }

    /// <summary>
    /// Calculates the fraction that value equals of the range [minValue, maxValue] and updates minSliderIndex.
    /// </summary>
    /// <param name="value">The value that minSliderIndex should be updated to. It will be rounded to the nearest allowed value.</param>
    void updateMinSliderIndexByValue(float value)
    {
        updateMinSliderIndex((value - MinValueLimit) / (MaxValueLimit - MinValueLimit));
    }

    /// <summary>
    /// Calculates the fraction (position - minhandle) / (maxhandle - minhandle) und updates maxSliderIndex.
    /// 
    /// Important! This Method assumes that position is a world space position located along the slider!
    /// </summary>
    /// <param name="position">Position to set maxSlider to. Position assumed to be a point along the slider!</param>
    public void updateMinSliderIndexByPosition(Vector3 position)
    {
        updateMinSliderIndex(Vector3.Distance(minHandle.position, position) / Vector3.Distance(minHandle.position, maxHandle.position));
    }

    /// <summary>
    /// Calculates the index of nearest allowed value for the maxSlider between minSliderIndex and allowedValueCount.
    /// </summary>
    /// <param name="value">Percentage (0-1) of the minSlider position along the slider</param>
    /// <param name="ignoreMinSlider">Ignores the minSliderIndex if true. Used when allowed values are recalculated.</param>
    void updateMaxSliderIndex(float value, bool ignoreMinSlider = false)
    {
        value = Mathf.Clamp01(value);

        // Calculate the index of the nearest allowed value
        int oldMaxSliderIndex = maxSliderIndex;
        maxSliderIndex = Mathf.RoundToInt(value * (float)(allowedValueCount - 1));

        // Clamp min index to max index
        if (!ignoreMinSlider && maxSliderIndex < minSliderIndex)
        {
            maxSliderIndex = minSliderIndex;
        }

        // Set flags for updates
        if (maxSliderIndex != oldMaxSliderIndex)
        {
            maxSliderIndexChanged = true;
        }
    }

    /// <summary>
    /// Calculates the fraction that value equals of the range [minValue, maxValue] and updates minSliderIndex.
    /// </summary>
    /// <param name="value">The value that minSliderIndex should be updated to. It will be rounded to the nearest allowed value.</param>
    void updateMaxSliderIndexByValue(float value, bool ignoreMinSlider = false)
    {
        updateMaxSliderIndex((value - MinValueLimit) / (MaxValueLimit - MinValueLimit), ignoreMinSlider);
    }

    /// <summary>
    /// Calculates the fraction (position - minhandle) / (maxhandle - minhandle) und updates maxSliderIndex.
    /// 
    /// Important! This Method assumes that position is a world space position located along the slider!
    /// </summary>
    /// <param name="position">Position to set maxSlider to. Position assumed to be a point along the slider!</param>
    public void updateMaxSliderIndexByPosition(Vector3 position)
    {
        updateMaxSliderIndex(Vector3.Distance(minHandle.position, position) / Vector3.Distance(minHandle.position, maxHandle.position));
    }

    /// <summary>
    /// Moves, rotates and scales the sliderBase so its ends match up with minHandle and maxHandle
    /// </summary>
    void updateSliderBase()
    {
        sliderBase.position = Vector3.Lerp(minHandle.position, maxHandle.position, 0.5f);
        sliderBase.LookAt(Vector3.Cross(minHandle.position, maxHandle.position), minHandle.position - maxHandle.position);
        float localDistance = Vector3.Distance(minHandle.localPosition, maxHandle.localPosition);
        sliderBase.localScale = new Vector3(sliderBaseThickness, localDistance / 2f, sliderBaseThickness);
    }

    void updateSelectedArea()
    {
        selectedArea.position = Vector3.Lerp(minSlider.position, maxSlider.position, 0.5f);
        selectedArea.LookAt(Vector3.Cross(minSlider.position, maxSlider.position), minSlider.position - maxSlider.position);
        float localDistance = Vector3.Distance(minSlider.localPosition, maxSlider.localPosition);
        selectedArea.localScale = new Vector3(selectedAreaThickness, localDistance / 2f, selectedAreaThickness);
    }

    void updateSliderSize()
    {
        minSlider.localScale = Vector3.one * sliderSize;
        maxSlider.localScale = Vector3.one * sliderSize;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(minHandle.position, Vector3.one * 5);
        Gizmos.DrawCube(maxHandle.position, Vector3.one * 5);
    }
}
