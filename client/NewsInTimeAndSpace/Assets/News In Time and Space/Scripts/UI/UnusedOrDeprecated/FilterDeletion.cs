using System;
using System.Collections;
using System.Collections.Generic;
using Ubiq.Logging;
using UnityEngine;

public class FilterDeletion : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        FilterBallObject script = other.gameObject.GetComponent<FilterBallObject>();
        if (script != null && script.Location == FilterBallObject.FilterBallLocation.Free)
        {
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }
}
