using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowPerformanceButton : UIButton
{
    public GameObject lowPerformanceGlobe;
    public GameObject globeLoadingManager;
    public GameObject globeGameObject;

    bool lowPerformanceMode;

    private void Start()
    {
        lowPerformanceMode = false;
        globeGameObject.SetActive(true);
        globeLoadingManager.SetActive(true);
        lowPerformanceGlobe.SetActive(false);
    }

    public override void onRaycastInteraction()
    {
        lowPerformanceMode = !lowPerformanceMode;
        if (lowPerformanceMode)
        {
            globeGameObject.SetActive(false);
            globeLoadingManager.SetActive(false);
            lowPerformanceGlobe.SetActive(true);
        } else
        {
            globeGameObject.SetActive(true);
            globeLoadingManager.SetActive(true);
            lowPerformanceGlobe.SetActive(false);
        }
    }
}
