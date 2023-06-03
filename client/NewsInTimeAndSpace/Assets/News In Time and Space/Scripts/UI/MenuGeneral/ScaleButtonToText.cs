using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// Class for a Button or interface object with a background which scales to the text size.
/// </summary>
public class ScaleButtonToText: MonoBehaviour
{
    private string text;
    
    public GameObject textObject;
    public GameObject containerObject;

    public string Text { get => text; set => text = value; }

    // Update is called once per frame
    void Update()
    {
        float xSize = textObject.GetComponent<TextMeshPro>().mesh.bounds.size.x + textObject.GetComponent<TextMeshPro>().mesh.bounds.size.x * 0.1f;
        float ySize = textObject.GetComponent<TextMeshPro>().mesh.bounds.size.y + textObject.GetComponent<TextMeshPro>().mesh.bounds.size.y * 0.1f;
        containerObject.transform.localScale = new Vector3(xSize, ySize, containerObject.transform.localScale.z);
    }
}
