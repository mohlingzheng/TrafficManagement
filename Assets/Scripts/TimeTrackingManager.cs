using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrackingManager : MonoBehaviour
{
    [Header("Time")]
    public double TotalTimeWaited = 0f;
    public static List<double> TimeWaitedPeriod = new List<double>();
    public double currentTotalTime = 0f;
    public static int VehicleReached = 0;

    void Start()
    {
        TimeWaitedPeriod.Clear();
        VehicleReached = 0;
        StartCoroutine(TimeWaitedLoop(5f));
    }

    void Update()
    {

    }

    public IEnumerator TimeWaitedLoop(float duration)
    {
        while(true)
        {
            yield return new WaitForSeconds(duration);
            VehicleMovement[] vehicleMovements = FindObjectsOfType<VehicleMovement>();
            foreach (VehicleMovement vehicleMovement in vehicleMovements)
            {
                currentTotalTime += vehicleMovement.timeWaited;
            }
            TimeWaitedPeriod.Add(currentTotalTime);
            TotalTimeWaited += currentTotalTime;
            currentTotalTime = 0f;
        }
    }
}
