using Barmetler.RoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleGeneration : MonoBehaviour
{
    public Vector3[] entryPoints;
    public GameObject vehicle;
    public int vehicleCount = 0;
    public int carLimit = 100;
    GameObject specificVehicle;
    // Start is called before the first frame update
    void Start()
    {
        SetEntryPoints();
        SetVehicle();
        StartCoroutine(GenerateVehicle(2.5f));
    }

    // Update is called once per frame
    void Update()
    {
        GenerateSpecificVehicle();
    }

    public void SetEntryPoints()
    {
        entryPoints = new Vector3[7];
        entryPoints[0] = new Vector3(100f, 0.5f, 4.2f);
        entryPoints[1] = new Vector3(528f, 0.5f, 12.9f);
        entryPoints[2] = new Vector3(408f, 0.5f, 4.2f);
        entryPoints[3] = new Vector3(210f, 0.5f, 5.6f);
        entryPoints[4] = new Vector3(1f, 0.5f, 763.6f);
        entryPoints[5] = new Vector3(100.5f, 0.5f, 999f);
        entryPoints[6] = new Vector3(606f, 0.5f, 1.5f);
    }

    public void SetVehicle()
    {
        
    }

    IEnumerator GenerateVehicle(float waitTime)
    {
        while (true)
        {
            if (vehicleCount <= carLimit)
            {
                for (int i = 0; i < 4; i++)
                {
                    int random = Random.Range(0, entryPoints.Length);
                    Instantiate(vehicle, entryPoints[random], Quaternion.identity);
                    vehicleCount++;
                }
            } 
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void ReduceVehicleCount(int count)
    {
        vehicleCount = vehicleCount - count;
    }

    private void GenerateSpecificVehicle()
    {
        if (!specificVehicle)
        {
            specificVehicle = Instantiate(vehicle, entryPoints[3], Quaternion.identity);
            specificVehicle.name = "Specific Vehicle";
        }
        else
        {
            Vector3 goal = new Vector3(542f, 0f, 425.3f);
            if (specificVehicle.GetComponent<RoadSystemNavigator>().Goal != goal)
                specificVehicle.GetComponent<RoadSystemNavigator>().Goal = goal;
        }
    }
}
