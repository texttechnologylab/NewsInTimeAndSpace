using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.XR;

public class SliderGraspable : MonoBehaviour, IGraspable
{
    public Slider3D sliderManager;
    public SliderType sliderType;

    public bool Grasped { get => updateSliderPosition; }

    public enum SliderType
    {
        MinSlider,
        MaxSlider
    }

    bool updateSliderPosition;
    Transform hand;
    Plane plane;

    private void Update()
    {
        if (updateSliderPosition)
        {
            // Construct Plane through hand with normal = slider ray
            Vector3 normal = sliderManager.maxHandle.position - sliderManager.minHandle.position;
            plane = new Plane(normal, hand.position);

            Ray ray = new Ray(sliderManager.minHandle.position, normal);
            if (!plane.Raycast(ray, out float enter))
            {
                enter = 0;
            }

            Vector3 position = ray.GetPoint(enter);

            if (sliderType == SliderType.MinSlider)
            {
                sliderManager.updateMinSliderIndexByPosition(position);
            }
            else if (sliderType == SliderType.MaxSlider)
            {
                sliderManager.updateMaxSliderIndexByPosition(position);
            }
        }
    }

    public void Grasp(Hand controller, Collider collider)
    {
        updateSliderPosition = true;
        hand = controller.transform;
    }

    public void Grasp(Hand controller)
    {
        updateSliderPosition = true;
        hand = controller.transform;
    }

    public void Release(Hand controller)
    {
        updateSliderPosition = false;

        if (sliderType == SliderType.MinSlider)
        {
            sliderManager.MinValueHasChanged = true;
        }
        else if (sliderType == SliderType.MaxSlider)
        {
            sliderManager.MaxValueHasChanged = true;
        }
    }
}
