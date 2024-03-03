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
    public string state = States.red.ToString();
    public float maxDistance = 5f;
    Vector3 raycastPosition;
    Vector3 raycastDirection;
    public int[] durations = new int[3];

    void Start()
    {
        raycastPosition = transform.position + new Vector3(0f, 1.5f, 0f);
        raycastDirection = transform.right;
        durations[0] = 5;
        durations[1] = 3;
        durations[2] = 5;

        StartCoroutine(TrafficLightCycle());
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(raycastPosition, raycastDirection, out hit, maxDistance))
        {
            if (hit.collider.gameObject.tag == "Vehicle")
            {
                if (state == States.red.ToString() || state == States.yellow.ToString())
                {
                    hit.collider.gameObject.GetComponent<VehicleMovement>().VehicleStop();
                }
                else if(state == States.green.ToString())
                {
                    hit.collider.gameObject.GetComponent<VehicleMovement>().VehicleMove();
                }
                Debug.DrawRay(raycastPosition, raycastDirection * maxDistance, Color.green);
            }
        }
        else
        {
            Debug.DrawRay(raycastPosition, raycastDirection * maxDistance, Color.red);
        }
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            state = States.green.ToString();
            yield return new WaitForSeconds(durations[0]);
            state = States.yellow.ToString();
            yield return new WaitForSeconds(durations[1]);
            state = States.red.ToString();
            yield return new WaitForSeconds(durations[2]);
        }
    }
}
