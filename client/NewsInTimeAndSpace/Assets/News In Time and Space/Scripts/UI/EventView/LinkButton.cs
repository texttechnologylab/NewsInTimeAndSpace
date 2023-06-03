using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Class for a button which contains an URL and opens URL outside of application.
/// </summary>
public class LinkButton : UIButton
{
    public TextMeshPro linkText;
    /// <summary>
    /// Method for checking raycast and opening URL.
    /// </summary>
    public override void onRaycastInteraction()
    {
        Debug.Log("Opening " + linkText.text + " in a browser.");
        Application.OpenURL(linkText.text);
    }
}
