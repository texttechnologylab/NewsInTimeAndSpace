using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;

    string serverURL;
    Vector3 handRotationOffset;

    public string ServerURL
    {
        get => serverURL;
        set
        {
            serverURL = value;
            if (!serverURL.StartsWith("http://"))
            {
                serverURL = "http://" + serverURL;
            }
            if (!serverURL.EndsWith("/news/"))
            {
                serverURL = serverURL + "/news/";
            }

            PlayerPrefs.SetString("serverURL", serverURL);
            Debug.Log("Updated ServerURL to " + serverURL);

            // Trigger update in all WebrequestHandlers in the scene
            WebRequestHandler[] webRequestHandler = Resources.FindObjectsOfTypeAll(typeof(WebRequestHandler)) as WebRequestHandler[];
            foreach (WebRequestHandler wh in webRequestHandler)
            {
                WebRequestHandler.serverURL = serverURL;
            }
        }
    }
    public Vector3 HandRotationOffset
    {
        get => handRotationOffset;
        set
        {
            handRotationOffset = value;
            PlayerPrefs.SetFloat("handRotationOffsetX", handRotationOffset.x);
            PlayerPrefs.SetFloat("handRotationOffsetY", handRotationOffset.y);
            PlayerPrefs.SetFloat("handRotationOffsetZ", handRotationOffset.z);

            leftHand.transform.localRotation = Quaternion.Euler(handRotationOffset);
            rightHand.transform.localRotation = Quaternion.Euler(handRotationOffset);
        }
    }

    void Awake()
    {
        // Load saved settings or default
        ServerURL = PlayerPrefs.GetString("serverURL", "http://localhost:8080/news/");
        WebRequestHandler.serverURL = ServerURL;
        float x = PlayerPrefs.GetFloat("handRotationOffsetX", 80);
        float y = PlayerPrefs.GetFloat("handRotationOffsetY", 0);
        float z = PlayerPrefs.GetFloat("handRotationOffsetZ", 0);

        HandRotationOffset = new Vector3(x, y, z);
    }
}
