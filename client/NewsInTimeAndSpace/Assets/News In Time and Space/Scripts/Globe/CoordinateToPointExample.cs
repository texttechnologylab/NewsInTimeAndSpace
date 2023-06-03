using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateToPointExample : MonoBehaviour
{
    public float longitude;
    public float latitude;
    public GameObject obj;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Vector3 position = GeoMaths.CoordinateToPoint(new CoordinateDegrees(longitude, latitude).ConvertToRadians(), 150f);
            Instantiate(obj, position, Quaternion.LookRotation(position - Vector3.zero));
        }
    }
}
