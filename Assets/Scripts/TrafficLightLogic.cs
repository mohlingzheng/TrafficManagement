using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightLogic : MonoBehaviour
{
    // Start is called before the first frame update
    RaycastHit hit;
    public TrafficLightState currentState = TrafficLightState.Red;
    public int[] durations = new int[3];

    void Start()
    {
        durations[0] = 3;
        durations[1] = 3;
        durations[2] = 3;
    }

    private void FixedUpdate()
    {
        if (!Tag.CompareTags(transform.parent.parent, Tag.Intersection_3_Small))
        {
            DisplayTrafficLightColor();
        }

        AskVehicleToCheck();
    }

    private void AskVehicleToCheck()
    {
        Vector3 raycastPosition = transform.position;
        float maxDistance = 10f;
        Ray ray = new Ray(raycastPosition, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance);
        Debug.DrawRay(raycastPosition, transform.forward * maxDistance, Color.white);
        foreach (RaycastHit hit in  hits)
        {
            if (hit.collider.gameObject.transform.tag == "Vehicle")
            {
                VehicleMovement vehicleMovement = hit.collider.gameObject.GetComponent<VehicleMovement>();
                //vehicleMovement.special = true;
                //vehicleMovement.FunctionCalledByTrafficLight(hit, gameObject);
                //vehicleMovement.special = false;
            }
        }
    }

    public bool IsSameLight(TrafficLightState trafficLightState)
    {
        if (currentState == trafficLightState)
            return true;
        return false;
    }
    
    public void SetCurrentState(TrafficLightState trafficLightState)
    {
        currentState = trafficLightState;
    }

    public void DisplayTrafficLightColor()
    {
        float length = 3f;
        if (IsSameLight(TrafficLightState.Red))
            Debug.DrawRay(transform.position, transform.up * length, Color.red);
        else
            Debug.DrawRay(transform.position, transform.up * length, Color.green);
    }
}
