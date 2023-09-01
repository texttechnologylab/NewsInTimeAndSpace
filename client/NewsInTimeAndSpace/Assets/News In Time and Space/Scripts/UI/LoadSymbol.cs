using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSymbol : MonoBehaviour
{
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward, Space.Self);
    }
}
