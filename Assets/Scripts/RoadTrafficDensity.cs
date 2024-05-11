using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTrafficDensity : MonoBehaviour
{
    public int NumberOfCars = 0;
    public float RoadLength = 0;
    public float TrafficDensity = 0;
    float divisor = 0;
    void Start()
    {
        RoadLength = gameObject.GetComponent<Road>().GetLength();
        //StartCoroutine(CalculateTrafficDensity());
        divisor = Tag.CompareTags(transform, Tag.Road_Large) ? 4 : 2;
    }

    void Update()
    {
        TrafficDensity = NumberOfCars / RoadLength;
    }

    private IEnumerator CalculateTrafficDensity()
    {
        while (true)
        {
            TrafficDensity = NumberOfCars / RoadLength / divisor;
            yield return new WaitForSeconds(5f);
        }
    }

    public void IncreaseCount()
    {
        NumberOfCars++;
    }

    public void DecreaseCount()
    {
        NumberOfCars--;
    }
}
