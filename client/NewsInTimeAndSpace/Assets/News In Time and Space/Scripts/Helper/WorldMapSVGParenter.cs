using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

// This Script is intended to run once
// it parses the map csv file and extract which path objects corrsepond to which country object
// these GameObjects are ordererd in parent (coutry) -> child (region) relationships recursively
public class WorldMapSVGParenter : MonoBehaviour
{
    public string svgFilePath;

    XmlDocument doc;

    // Start is called before the first frame update
    void Start()
    {
        doc = new XmlDocument();
        doc.Load(svgFilePath);
        Debug.Log("SVG loaded");

        XmlNodeList gList = doc.GetElementsByTagName("g");
        for (int i = 0; i < gList.Count; i++)
        {
            Debug.Log(gList[i].Attributes.GetNamedItem("id").Value);
        }
    }

    void parentPathsToGElement(XmlNodeList gList)
    {

    }
}
