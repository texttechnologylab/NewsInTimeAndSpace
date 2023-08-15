using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;

/// <summary>
/// WebRequestHandler
/// 
/// This class handles building of webrequest URIs and asynchrounous webrequests.
/// </summary>
public class WebRequestHandler : MonoBehaviour
{
    public float defaultDelay = 5;
    public static string serverURL; // The base URL of the webserver.
    public Settings settings;

    CultureInfo en = new CultureInfo("en-US", false); // C# english CultureInfo used for converting numbers into strings.
    const int defaultLimit = 100;

    string data;
    bool dataUpdated;
    bool dataUnread; // true if new data available that hasn't been read yet
    Dictionary<string, List<string>> parameters;

    string request;
    string subPath;
    Coroutine requestHandle;
    UnityWebRequest webRequest;

    private void Start()
    {
        parameters = new Dictionary<string, List<string>>();
    }

    public bool DataUpdated { get => dataUpdated; }
    public bool DataUnread { get => dataUnread; }
    /// <summary>
    /// Returns the currently stored data and updates the flag to indicate that data has been read.
    /// </summary>
    public string Data
    {
        get
        {
            dataUnread = false;
            return data;
        }
        set
        {
            data = value;
            dataUpdated = true;
            dataUnread = true;
        }
    }

    public string Request { get => request; set => request = value; }
    public string SubPath { get => subPath; set => subPath = value; }

    /// <summary>
    /// Starts a new asynchronous webrequest and sets flags to indicate when data is ready to be read.
    /// </summary>
    public void startRequest(float delay = 0)
    {
        // Stop previous webrequest if it's still running
        if (requestHandle != null)
        {
            Debug.Log("Aborting running webrequest because a new one is being started.");
            StopCoroutine(requestHandle);
            webRequest.Abort();
        }

        // Start new webrequest
        string requestURI = buildRequestURI();
        dataUpdated = false;
        dataUnread = true;
        Debug.Log("Starting request: " + requestURI);

        requestHandle = StartCoroutine(GetRequest(requestURI, delay, (result) =>
        {
            // Handle webrequest result
            requestHandle = null;
            if (result == null)
            {
                Debug.Log("Webrequest encountered an error. Retrying in " + defaultDelay + " seconds.");
                startRequest(defaultDelay);
            }
            else
            {
                data = result;
                dataUpdated = true;
            }
        }));
    }

    /// <summary>
    /// Adds a parameter to the webrequest being built.
    /// </summary>
    /// <param name="name">Name of the parameter in the webrequest</param>
    /// <param name="value">Value of the parameter in the webrequest</param>
    public void addParameter(string name, string value)
    {
        if (parameters.ContainsKey(name))
        {
            parameters[name].Add(value);
        }
        else
        {
            List<string> newParameter = new List<string>();
            newParameter.Add(value);
            parameters.Add(name, newParameter);
        }
    }

    /// <summary>
    /// Adds multiple parameters to the webrequest being built.
    /// </summary>
    /// <param name="name">Name of the parameter in the webrequest</param>
    /// <param name="value">Value of the parameter in the webrequest</param>
    public void addParameter(string name, IEnumerable<string> value)
    {
        if (parameters.ContainsKey(name))
        {
            parameters[name].AddRange(value);
        }
        else
        {
            List<string> newParameter = new List<string>();
            newParameter.AddRange(value);
            parameters.Add(name, newParameter);
        }
    }

    /// <summary>
    /// Adds an integer parameter to the webrequest being built.
    /// </summary>
    /// <param name="name">Name of the parameter in the webrequest</param>
    /// <param name="value">Value of the parameter in the webrequest</param>
    public void addParameter(string name, int value)
    {
        addParameter(name, value.ToString());
    }

    /// <summary>
    /// Adds a float parameter to the webrequest being built.
    /// </summary>
    /// <param name="name">Name of the parameter in the webrequest</param>
    /// <param name="value">Value of the parameter in the webrequest</param>
    public void addParameter(string name, float value)
    {
        addParameter(name, value.ToString(en.NumberFormat));
    }

    /// <summary>
    /// Adds a geolocation and a radius as a parameter to the webrequest being built.
    /// </summary>
    /// <param name="location">Geolocation to be passed as parameter in {longitude, latitude} order</param>
    /// <param name="radius">Radius in km around the geolocation</param>
    public void addParameter(Vector2 location, float radius)
    {
        addParameter("lat", location.y);
        addParameter("long", location.x);
        addParameter("radius", radius);
    }

    /// <summary>
    /// Adds a start and end date as a parameter to the webrequest being built.
    /// </summary>
    /// <param name="startDate">Beginning of the wanted timespan</param>
    /// <param name="endDate">End of the wanted timespan</param>
    public void addParameter(DateTime startDate, DateTime endDate)
    {
        addParameter("startTime", startDate.ToString("yyyy-MM-dd"));
        addParameter("endTime", endDate.ToString("yyyy-MM-dd"));
    }

    /// <summary>
    /// Adds all parameters from a filterPool object to the parameters dictionary.
    /// </summary>
    /// <param name="filterPool">FilterPool object containing the filters</param>
    public void addParameter(FilterPool filterPool)
    {
        if (filterPool.DatesSet)
        {
            addParameter(filterPool.StartDate, filterPool.EndDate);
        }
        if (filterPool.LocationSet)
        {
            addParameter(filterPool.Location, filterPool.Radius);
        }
        if (filterPool.Actors.Count > 0)
        {
            addParameter("actor", filterPool.Actors);
        }
        if (filterPool.EventTypes.Count > 0)
        {
            addParameter("type", filterPool.EventTypes);
        }
        if (filterPool.GoldsteinSet)
        {
            addParameter("GoldsteinScaleLow", filterPool.MinGoldstein);
            addParameter("GoldsteinScaleHigh", filterPool.MaxGoldstein);
        }
        if (filterPool.ToneSet)
        {
            addParameter("ToneMin", filterPool.MinTone);
            addParameter("ToneMax", filterPool.MaxTone);
        }
        if (filterPool.LimitSet)
        {
            addParameter("limit", filterPool.Limit);
        }
    }

    /// <summary>
    /// Removes a previously added parameter from the dictionary.
    /// </summary>
    /// <param name="name">Exact name of the parameter to remove</param>
    public void removeParameter(string name)
    {
        parameters.Remove(name);
    }

    /// <summary>
    /// Removes all parameters from the parameters dictionary.
    /// </summary>
    public void clearParameters()
    {
        parameters.Clear();
    }

    /// <summary>
    /// Removes the stored subpath.
    /// </summary>
    public void clearSubPath()
    {
        this.SubPath = null;
    }

    /// <summary>
    /// Clears all parameters and resets this WebRequestHandler to build a new request.
    /// </summary>
    public void clearAll()
    {
        clearParameters();
        clearSubPath();
    }

    /// <summary>
    /// Clears stored data without setting the dataUpdated or dataUnread flags.
    /// </summary>
    public void clearData()
    {
        this.data = "";
    }

    /// <summary>
    /// Creates the requestURI from the serverURL, the subpath and all stored parameters.
    /// </summary>
    /// <returns>The requestURI containing the subpath and all parameters</returns>
    public string buildRequestURI()
    {
        string requestURI = serverURL + Request;

        if (SubPath != null)
        {
            if (requestURI[requestURI.Length - 1] != '/' && SubPath[0] != '/')
            {
                requestURI += "/";
            }

            requestURI += SubPath;
        }

        bool andNeeded = false;

        if (parameters.Count == 0)
        {
            return requestURI;
        }

        requestURI += "?";

        foreach (string name in parameters.Keys)
        {
            foreach (string value in parameters[name])
            {
                if (andNeeded)
                {
                    requestURI += "&";
                }
                else
                {
                    andNeeded = true;
                }

                requestURI += name + "=" + value;
            }
        }

        return requestURI;
    }

    /// <summary>
    /// This Coroutine executes the webrequest and waits for a server response.
    /// It returns a callback that is null if there was an error, otherwise it will return the callback with the JSON response from the server.
    /// </summary>
    /// <param name="requestURI">The URI of the GET request</param>
    /// <param name="callback">The callback object, defaults to null</param>
    /// <returns>Callback with invoked value. Is null if there was an error, otherwise it invokes the JSON response string</returns>
    IEnumerator GetRequest(string requestURI, float delay = 0, System.Action<string> callback = null)
    {
        // Wait if delay is set
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        // Send request and yield until response
        webRequest = UnityWebRequest.Get(requestURI);
        webRequest.timeout = 10000;
        yield return webRequest.SendWebRequest();

        // Process received data
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                Debug.LogError("WebRequest: Connection error");
                callback.Invoke(null);
                break;
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("WebRequest: Data processing error on: " + requestURI);
                callback.Invoke(null);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("WebRequest: Protocol error on: " + requestURI);
                callback.Invoke(null);
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log("WebRequest: Received");
                callback.Invoke(webRequest.downloadHandler.text);
                break;
        }
    }
}
