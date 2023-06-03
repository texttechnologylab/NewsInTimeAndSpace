using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp.Server;
/// <summary>
/// Class which handles the EventView Object.
/// </summary>
public class EventView : MonoBehaviour
{
    private List<EventVisualizer> eventVisualizers = new List<EventVisualizer>();
    private Texture[] images;
    private int filterCurrentIndex = 0;

    public TextMeshPro eventTitleText;
    public TextMeshPro eventSourceText;
    public TextMeshPro numberText;
    public TextMeshPro eventDateText;
    public TextMeshPro eventLocationText;
    public TextMeshPro eventActor1Text;
    public TextMeshPro eventActor2Text;
    public TextMeshPro toneText;
    public TextMeshPro goldsteinText;
    public Renderer eventPictureRenderer;
    public GameObject filterUpArrow;
    public GameObject filterDownArrow;

    // Start is called before the first frame update
    void Start()
    {
        setArrowVariables();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Sets variables of arrow objects.
    /// </summary>
    private void setArrowVariables()
    {
        filterUpArrow.GetComponent<ArrowElement>().setEventViewScrollBar(this);
        filterDownArrow.GetComponent<ArrowElement>().setEventViewScrollBar(this);
    }

    /// <summary>
    /// Move Index via parameters.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="direction"></param>
    public void moveIndexesByArrow(ArrowElement.Type type, ArrowElement.Direction direction)
    {
        if (type == ArrowElement.Type.Filter && direction == ArrowElement.Direction.Down)
        {
            if (filterCurrentIndex < eventVisualizers.Count - 1)
            {
                filterCurrentIndex += 1;
                changeEvent(eventVisualizers[filterCurrentIndex]);
                Debug.Log(filterCurrentIndex);
            }
        }
        else if (type == ArrowElement.Type.Filter && direction == ArrowElement.Direction.Up)
        {
            if (filterCurrentIndex > 0)
            {
                filterCurrentIndex -= 1;
                changeEvent(eventVisualizers[filterCurrentIndex]);
                Debug.Log(filterCurrentIndex);
            }
        }
    }

    /// <summary>
    /// Give EventVisualizer list and activate attached object.
    /// </summary>
    /// <param name="eventVisualizer"></param>
    public void openEventView(List<EventVisualizer> eventVisualizer)
    {
        this.eventVisualizers.Clear();
        this.images = new Texture[eventVisualizer.Count];
        if (eventVisualizer.Count > 0)
        {
            filterCurrentIndex = 0;
            this.eventVisualizers = new List<EventVisualizer>(eventVisualizer);
            searchPictures(eventVisualizer);
            changeEvent(this.eventVisualizers[0]);
        }
    }

    /// <summary>
    /// Deactivate attached object.
    /// </summary>
    public void closeEventView()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Method to change currently shown event to the one given.
    /// </summary>
    /// <param name="eventVisualizer"></param>
    private void changeEvent(EventVisualizer eventVisualizer)
    {
        if (eventVisualizer != null)
        {
            int index = eventVisualizers.IndexOf(eventVisualizer);
            eventTitleText.text = eventVisualizer.NewsEvent.Title;
            eventSourceText.text = eventVisualizer.NewsEvent.Source;
            eventDateText.text = eventVisualizer.NewsEvent.Date.ToLongDateString();
            eventLocationText.text = "Location:\n" + eventVisualizer.NewsEvent.LocationName;
            toneText.text = "Tone:\n" + eventVisualizer.NewsEvent.Tone;
            goldsteinText.text = "Goldstein score:\n" + eventVisualizer.NewsEvent.GoldsteinScale;

            if (eventVisualizer.NewsEvent.Actors != null && eventVisualizer.NewsEvent.Actors.Length > 0)
                eventActor1Text.text = "Actor 1:\n" + eventVisualizer.NewsEvent.Actors[0].Name;
            else
                eventActor1Text.text = "Actor 1:\n" + "No Actor";
            if (eventVisualizer.NewsEvent.Actors != null && eventVisualizer.NewsEvent.Actors.Length > 1)
                eventActor2Text.text = "Actor 2:\n" + eventVisualizer.NewsEvent.Actors[1].Name;
            else
                eventActor2Text.text = "Actor 2:\n" + "No Actor";

            if (index >= 0)
                eventPictureRenderer.material.mainTexture = images[index];
            numberText.text = (index + 1).ToString() + "/" + eventVisualizers.Count;
        }
    }

    /// <summary>
    /// Method to initiate an http web request for a number of image links contained inside given EventVisualizers.
    /// </summary>
    /// <param name="eventVisualizers"></param>
    private void searchPictures(List<EventVisualizer> eventVisualizers)
    {
        foreach (EventVisualizer eventVisualizer in eventVisualizers)
        {
            Debug.Log("Media list size: " + eventVisualizer.NewsEvent.Media.Length);
            Debug.Log("Type: " + eventVisualizer.NewsEvent.Media[0].Type);
            Debug.Log("Content: " + eventVisualizer.NewsEvent.Media[0].Content);
            StartCoroutine(addPicture(eventVisualizer.NewsEvent.Media[0].Content, eventVisualizers.IndexOf(eventVisualizer)));
        }
    }

    /// <summary>
    /// Coroutine which creates webrequest and adds possible result textures to array.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    IEnumerator addPicture(string url, int index)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("No picture or picture request error");
            images[index] = null;
        }
        else
        {
            Debug.Log("texture received");
            images[index] = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (filterCurrentIndex == index)
            {
                eventPictureRenderer.material.mainTexture = images[index];
            }
        }
    }
}
