using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ubiq.Samples;

public class ServerURLButton : MonoBehaviour
{
    public Text serverURLText;
    public Settings settings;
    public TextEntry textEntry;

    public void OnEnable()
    {
        clearTextEntry();
    }

    public void Start()
    {
        clearTextEntry();
    }

    public void setServerURL()
    {
        settings.ServerURL = serverURLText.text.ToLowerInvariant();
        clearTextEntry();
    }

    void clearTextEntry()
    {
        textEntry.defaultText = settings.ServerURL;
        textEntry.Clear();
    }
}
