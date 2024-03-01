using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleGeneration : MonoBehaviour
{
    public Vector3[] entryPoints;
    public GameObject vehicle;
    public int vehicleCount = 0;
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
            if (vehicleCount <= 100)
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
}
