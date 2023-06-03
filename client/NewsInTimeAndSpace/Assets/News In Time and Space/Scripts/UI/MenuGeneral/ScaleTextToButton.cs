using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class for a button or interface object which scales its text to the background size.
/// </summary>
public class ScaleTextToButton : MonoBehaviour
{
    private string text;

    public GameObject textObject;
    public GameObject containerObject;

    public string Text { get => text; set => text = value; }

    // Update is called once per frame
    void Update()
    {
        textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(containerObject.transform.localScale.x * 9, containerObject.transform.localScale.y * 9);
    }
}
