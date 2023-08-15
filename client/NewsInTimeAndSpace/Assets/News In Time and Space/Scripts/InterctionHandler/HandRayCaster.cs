using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRayCaster : MonoBehaviour
{
    public LayerMask raycastBlocker;
    public LayerMask visualizerLayer;
    public LayerMask uiLayer;
    public LineRenderer lineRenderer;
    public Transform player;

    RaycastHit rayHit;
    float baseDistance;

    public RaycastHit RayHit { get => rayHit; set => rayHit = value; }

    // Start is called before the first frame update
    void Start()
    {
        baseDistance = player.position.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out rayHit, 1000f, visualizerLayer | uiLayer | raycastBlocker))
        {
            Vector3[] positions = { transform.position, rayHit.point };
            lineRenderer.widthMultiplier = player.position.magnitude / baseDistance;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(positions);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
