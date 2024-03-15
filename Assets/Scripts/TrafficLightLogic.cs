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

        //StartCoroutine(TrafficLightCycle());
    }

    private void FixedUpdate()
    {
        if (IsSameLight(TrafficLightState.Red))
            Debug.DrawRay(transform.position, transform.forward, Color.red);
        else
            Debug.DrawRay(transform.position,transform.forward, Color.green);
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            currentState = TrafficLightState.Green;
            yield return new WaitForSeconds(durations[0]);
            //currentState = States.yellow.ToString();
            //yield return new WaitForSeconds(durations[1]);
            currentState = TrafficLightState.Red;
            yield return new WaitForSeconds(durations[2]);
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
}
