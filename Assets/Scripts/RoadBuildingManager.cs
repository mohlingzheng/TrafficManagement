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
    public GameObject roadGameObject;
    public GameObject intersectionGB;


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
        Vector3 segmentPosition2 = new Vector3(438f, 0f, 314.2f);
        RoadAnchor start = road.start;
        RoadAnchor end = road.end;
        start.Disconnect();
        end.Disconnect();
        road.Clear();
        Destroy(road1);

        GameObject road1split_1, road1split_2, road1split_intersection;
        road1split_1 = Instantiate(roadGameObject, start.transform.position, Quaternion.identity);
        road1split_1.GetComponent<Road>().start = start;
        road1split_1.name = "road1split_1";

        road1split_intersection = Instantiate(intersectionGB, segmentPosition, Quaternion.identity);
        road1split_intersection.name = "new intersection";
        RoadAnchor[] ras = road1split_intersection.GetComponentsInChildren<RoadAnchor>();
        Debug.Log(ras.Length);
        //ras[1].SetRoad(road1split_1.GetComponent<Road>());


        road1split_2 = Instantiate(roadGameObject, end.transform.position, Quaternion.identity);
        road1split_2.GetComponent<Road>().end = end;
        road1split_2.name = "road1split_2";
        //ras[2].SetRoad(road1split_2.GetComponent<Road>());

        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("Vehicle");
        foreach (GameObject vehicle in vehicles)
        {
            vehicle.GetComponent<RoadSystemNavigator>().CalculateWayPointsSync();
        }

        SceneView.RepaintAll();

    }
}
