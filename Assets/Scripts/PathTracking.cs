using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTracking : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public InputManager inputManager;
    void Start()
    {
        
    }

    void Update()
    {
        GameObject highlight = null, selection = null;
        if (inputManager.highlight != null)
            highlight = inputManager.highlight.gameObject;
        if (inputManager.selection != null)
            selection = inputManager.selection.gameObject;

        if (highlight == null && selection == null)
        {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;

        VehicleMovement vehicleMovement;
        Vector3 startPosition;
        if (highlight != null && Tag.CompareTags(highlight.transform, Tag.Vehicle))
        {
            vehicleMovement = highlight.GetComponent<VehicleMovement>();
            startPosition = highlight.transform.position;
        }
        else if (selection != null && Tag.CompareTags(selection.transform, Tag.Vehicle))
        {
            vehicleMovement = selection.GetComponent<VehicleMovement>();
            startPosition = selection.transform.position;
        }
        else
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.positionCount = vehicleMovement.movePoints.Count + 1;
        startPosition.y += 0.5f;
        lineRenderer.SetPosition(0, startPosition);

        for (int i = 0; i < vehicleMovement.movePoints.Count; i++)
        {
            startPosition = vehicleMovement.movePoints[i].position;
            startPosition.y += 0.5f;
            lineRenderer.SetPosition(i + 1, startPosition);
        }
    }
}
