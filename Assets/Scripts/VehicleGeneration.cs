using Barmetler.RoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleGeneration : MonoBehaviour
{
    public List<GameObject> queuePoints;
    public QueueingLogic[] queueLogics;
    public GameObject VehiclePrefab;
    public GameObject QueueSystem;
    public int vehicleCount = 0;
    public int carLimit = 100;
    GameObject specificVehicle;
    GameObject specificVehicle2;
    public bool GenerateFixVehicle = true;

    void Start()
    {
        SetEntryPoints();
    }

    void FixedUpdate()
    {
        if (GenerateFixVehicle)
        {
            GenerateSpecificVehicle();
            //GenerateSpecificVehicle2();
        }
        if (vehicleCount < carLimit)
        {
            int random = Random.Range(0, queuePoints.Count);
            QueueingLogic queueingLogic = queuePoints[random].GetComponent<QueueingLogic>();
            queueingLogic.queue.Enqueue(VehiclePrefab);
            vehicleCount++;
        }
    }

    public void SetEntryPoints()
    {
        queuePoints = QueueSystem.GetComponent<QueueManager>().queuePointsList;

        //while(queuePoints.Length <= 0 )
        //queuePoints = GameObject.FindGameObjectsWithTag("Queue");
        //entryPoints = new Vector3[7];
        //entryPoints[0] = new Vector3(100f, 0.5f, 4.2f);
        //entryPoints[1] = new Vector3(528f, 0.5f, 12.9f);
        //entryPoints[2] = new Vector3(408f, 0.5f, 4.2f);
        //entryPoints[3] = new Vector3(210f, 0.5f, 5.6f);
        //entryPoints[4] = new Vector3(1f, 0.5f, 763.6f);
        //entryPoints[5] = new Vector3(100.5f, 0.5f, 999f);
        //entryPoints[6] = new Vector3(606f, 0.5f, 1.5f);

    }

    public void ReduceVehicleCount(int count)
    {
        vehicleCount = vehicleCount - count;
    }

    private void GenerateSpecificVehicle()
    {
        Vector3 spawnPosition = new Vector3(226.8f, 0f, 28.5f);
        if (!specificVehicle)
        {
            specificVehicle = Instantiate(VehiclePrefab, spawnPosition, Quaternion.identity);
            specificVehicle.name = "Specific Vehicle";
            specificVehicle.GetComponent<VehicleMovement>().desiredSpeed = 1f;
        }
        //else
        //{
        //    Vector3 goal = new Vector3(542f, 0f, 425.3f);
        //    if (specificVehicle.GetComponent<RoadSystemNavigator>().Goal != goal) { }
        //        specificVehicle.GetComponent<RoadSystemNavigator>().Goal = goal;
        //}
    }

    private void GenerateSpecificVehicle2()
    {
        if (!specificVehicle2)
        {
            //specificVehicle2 = Instantiate(VehiclePrefab, entryPoints[3], Quaternion.identity);
            specificVehicle2.name = "Specific Vehicle 2";
            specificVehicle2.GetComponent<VehicleMovement>().desiredSpeed = 1f;
        }
        //else
        //{
        //    Vector3 goal = new Vector3(542f, 0f, 425.3f);
        //    if (specificVehicle2.GetComponent<RoadSystemNavigator>().Goal != goal)
        //    {
        //        specificVehicle2.GetComponent<RoadSystemNavigator>().Goal = goal;
        //    }
        //}
    }
}
