using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.EditorTools;

public class RoadBuildingManager : MonoBehaviour
{
    public GameObject road1;
    public GameObject road2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildRoad()
    {
        
        Road road = road1.GetComponent<Road>();
        //Barmetler.RoadSystem.RoadEditor roadEditor = road1.GetComponent<Barmetler.RoadSystem.RoadEditor>();
        Vector3[] points = road.GetPointsInSegment(1);
        Vector3 point1 = points[0];
        Vector3 segmentPosition = new Vector3(316, 0, 298);
        road.InsertSegment(segmentPosition, 1);


    }
}
