using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightLogic : MonoBehaviour
{
    // Start is called before the first frame update
    RaycastHit hit;
    enum States
    {
        red,
        yellow,
        green
    }
    public string currentState = States.red.ToString();
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
        if (IsRedLight())
            Debug.DrawRay(transform.position, transform.forward, Color.red);
        else
            Debug.DrawRay(transform.position,transform.forward, Color.green);
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            currentState = States.green.ToString();
            yield return new WaitForSeconds(durations[0]);
            //currentState = States.yellow.ToString();
            //yield return new WaitForSeconds(durations[1]);
            currentState = States.red.ToString();
            yield return new WaitForSeconds(durations[2]);
        }
    }

    public bool IsRedLight()
    {
        if (currentState == States.red.ToString())
            return true;
        return false;
    }
    
    public void SetCurrentState(string state)
    {
        currentState = state;
    }
}
