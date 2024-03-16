using Barmetler.RoadSystem;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class RoadBuildingManager : MonoBehaviour
{
    public RoadSystem roadSystem;
    public GameObject roadPrefab;
    public GameObject intersectionGB;
    public List<Road> roadList = new List<Road>();
    public List<Road> roadList2 = new List<Road>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RebuildAffectedRoad();
    }

    //public void BuildRoad()
    //{
    //    Vector3 firstPositionClicked = new Vector3(316, 0, 298);
    //    Vector3 secondPositionClicked = new Vector3(438f, 0f, 314.2f);
    //    Road road1_road = roadGameObject.GetComponent<Road>();
    //    RoadAnchor road1_start = road1_road.start;
    //    RoadAnchor road1_end = road1_road.end;
    //    road1_road.start.Disconnect();
    //    road1_road.end.Disconnect();
    //    road1_road.Clear();
    //    road1_road.RefreshEndPoints();
    //    Destroy(roadGameObject.GetComponent<RoadMeshGenerator>());
    //    Destroy(road1_road);
    //    Destroy(roadGameObject);


    //    GameObject first_roadGO;
    //    first_roadGO = Instantiate(roadPrefab, roadSystem.transform);
    //    first_roadGO.transform.localPosition = road1_start.transform.position;
    //    first_roadGO.transform.localRotation = Quaternion.identity;
    //    first_roadGO.name = "road 1 first split";

    //    GameObject first_interesection;
    //    first_interesection = Instantiate(intersectionGB, roadSystem.transform);
    //    first_interesection.transform.localPosition = firstPositionClicked;
    //    first_interesection.transform.localRotation = Quaternion.Euler(0, 50f, 0);
    //    first_interesection.name = "first intersection";
    //    first_interesection.tag = Tag.Intersection3.ToString();
    //    RoadAnchor[] roadAnchors = first_interesection.GetComponentsInChildren<RoadAnchor>();

    //    Road first_roadGO_road = first_roadGO.GetComponent<Road>();
    //    first_roadGO_road.end = roadAnchors[1];
    //    first_roadGO_road.start = road1_start;
    //    first_roadGO_road.AutoSetAllControlPoints();

    //    roadList.Add(first_roadGO_road);

    //    GameObject second_roadGO;
    //    second_roadGO = Instantiate(roadPrefab, roadSystem.transform);
    //    second_roadGO.transform.localPosition = road1_end.transform.position;
    //    second_roadGO.transform.localRotation = Quaternion.identity;
    //    second_roadGO.name = "road 1 second split";

    //    Road second_roadGO_road = second_roadGO.GetComponent<Road>();
    //    second_roadGO_road.end = road1_end;
    //    second_roadGO_road.start = roadAnchors[2];
    //    second_roadGO_road.AutoSetAllControlPoints();

    //    roadList.Add(second_roadGO_road);

    //    // 

    //    Road road2_road = road2GO.GetComponent<Road>();
    //    RoadAnchor road2_end = road2_road.end;
    //    RoadAnchor road2_start = road2_road.start;
    //    road2_road.end.Disconnect();
    //    road2_road.Clear();
    //    road2_road.RefreshEndPoints();
    //    Destroy(road2GO.GetComponent<RoadMeshGenerator>());
    //    Destroy(road2_road);
    //    Destroy(road2GO);

    //    GameObject first_roadGO2;
    //    first_roadGO2 = Instantiate(roadPrefab, roadSystem.transform);
    //    first_roadGO2.transform.localPosition = road2_end.transform.position;
    //    first_roadGO2.transform.localRotation = Quaternion.identity;
    //    first_roadGO2.name = "road 2 first split";

    //    GameObject second_intersection;
    //    second_intersection = Instantiate(intersectionGB, roadSystem.transform);
    //    second_intersection.transform.localPosition = secondPositionClicked;
    //    second_intersection.transform.localRotation = Quaternion.Euler(0, -90f, 0);
    //    second_intersection.name = "second intersection";
    //    second_intersection.tag = Tag.Intersection3.ToString();
    //    RoadAnchor[] roadAnchors2 = second_intersection.GetComponentsInChildren<RoadAnchor>();

    //    Road first_roadGO2_road = first_roadGO2.GetComponent<Road>();
    //    first_roadGO2_road.start = road2_start;
    //    first_roadGO2_road.end = roadAnchors2[2];
    //    first_roadGO2_road.AutoSetAllControlPoints();

    //    roadList.Add(first_roadGO2_road);

    //    GameObject second_roadGO2;
    //    second_roadGO2 = Instantiate(roadPrefab, roadSystem.transform);
    //    second_roadGO2.transform.localPosition = road2_end.transform.position;
    //    second_roadGO2.transform.localRotation = Quaternion.identity;
    //    second_roadGO2.name = "road 2 second split";

    //    Road second_roadGO2_road = second_roadGO2.GetComponent<Road>();
    //    second_roadGO2_road.end = road2_end;
    //    second_roadGO2_road.start = roadAnchors2[1];
    //    second_roadGO2_road.AutoSetAllControlPoints();

    //    roadList.Add(second_roadGO2_road);

    //    //

    //    GameObject bet_intersection_roadGO;
    //    bet_intersection_roadGO = Instantiate(roadPrefab, roadSystem.transform);
    //    bet_intersection_roadGO.transform.localPosition = firstPositionClicked;
    //    bet_intersection_roadGO.transform.localRotation = Quaternion.identity;
    //    bet_intersection_roadGO.name = "link";
    //    Road bet_intersection_roadGO_road = bet_intersection_roadGO.GetComponent<Road>();
    //    bet_intersection_roadGO_road.start = roadAnchors[0];
    //    bet_intersection_roadGO_road.end = roadAnchors2[0];
    //    second_roadGO2_road.AutoSetAllControlPoints();

    //    roadList.Add(bet_intersection_roadGO_road);


    //    roadSystem.RebuildAllRoads();

    //    SceneView.RepaintAll();

    //}

    public RoadAnchor AddIntersectionToSingleRoad(GameObject roadGameObject, Vector3 position)
    {
        // Get the connected Anchor, then remove this gameobject
        Road road = roadGameObject.GetComponent<Road>();
        RoadAnchor road1_start = road.start;
        RoadAnchor road1_end = road.end;
        road.start.Disconnect();
        road.end.Disconnect();
        road.Clear();
        road.RefreshEndPoints();
        Destroy(roadGameObject.GetComponent<RoadMeshGenerator>());
        Destroy(road);
        Destroy(roadGameObject);

        // create first road
        GameObject first_roadGO;
        first_roadGO = Instantiate(roadGameObject, roadSystem.transform);
        first_roadGO.transform.localPosition = road1_start.transform.position;
        first_roadGO.transform.localRotation = Quaternion.identity;
        first_roadGO.name = "road 1 first split";

        Road first_roadGO_road = first_roadGO.GetComponent<Road>();
        first_roadGO_road.start = road1_start;

        // create second road
        GameObject second_roadGO;
        second_roadGO = Instantiate(roadGameObject, roadSystem.transform);
        second_roadGO.transform.localPosition = road1_end.transform.position;
        second_roadGO.transform.localRotation = Quaternion.identity;
        second_roadGO.name = "road 1 second split";

        Road second_roadGO_road = second_roadGO.GetComponent<Road>();
        second_roadGO_road.end = road1_end;

        // create intersection
        Vector3 direction = (road1_start.transform.position - road1_end.transform.position).normalized;
        direction = Vector3.Cross(direction, Vector3.up).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject first_interesection;
        first_interesection = Instantiate(intersectionGB, roadSystem.transform);
        first_interesection.transform.localPosition = position;
        first_interesection.transform.localRotation = rotation;
        first_interesection.name = "first intersection";
        first_interesection.tag = Tag.Intersection3.ToString();
        
        RoadAnchor[] roadAnchors = first_interesection.GetComponentsInChildren<RoadAnchor>();
        first_roadGO_road.end = roadAnchors[1];
        second_roadGO_road.start = roadAnchors[2];


        roadList.Add(first_roadGO_road);
        roadList.Add(second_roadGO_road);

        return roadAnchors[0];
    }

    public RoadAnchor AddIntersectionToSingleRoad2(GameObject roadGameObject, Vector3 position, Quaternion rotation)
    {
        // Get the connected Anchor, then remove this gameobject
        Road road = roadGameObject.GetComponent<Road>();
        RoadAnchor road1_start = road.start;
        RoadAnchor road1_end = road.end;
        road.start.Disconnect();
        road.end.Disconnect();
        road.Clear();
        road.RefreshEndPoints();
        Destroy(roadGameObject.GetComponent<RoadMeshGenerator>());
        Destroy(road);
        Destroy(roadGameObject);


        GameObject first_roadGO;
        first_roadGO = Instantiate(roadGameObject, roadSystem.transform);
        first_roadGO.transform.localPosition = road1_start.transform.position;
        first_roadGO.transform.localRotation = Quaternion.identity;
        first_roadGO.name = "road 1 first split";

        GameObject first_interesection;
        first_interesection = Instantiate(intersectionGB, roadSystem.transform);
        first_interesection.transform.localPosition = position;
        first_interesection.transform.localRotation = rotation;
        first_interesection.name = "first intersection";
        first_interesection.tag = Tag.Intersection3.ToString();
        RoadAnchor[] roadAnchors = first_interesection.GetComponentsInChildren<RoadAnchor>();

        Road first_roadGO_road = first_roadGO.GetComponent<Road>();
        first_roadGO_road.end = roadAnchors[2];
        first_roadGO_road.start = road1_start;
        first_roadGO_road.AutoSetAllControlPoints();

        roadList.Add(first_roadGO_road);

        GameObject second_roadGO;
        second_roadGO = Instantiate(roadGameObject, roadSystem.transform);
        second_roadGO.transform.localPosition = road1_end.transform.position;
        second_roadGO.transform.localRotation = Quaternion.identity;
        second_roadGO.name = "road 1 second split";

        Road second_roadGO_road = second_roadGO.GetComponent<Road>();
        second_roadGO_road.end = road1_end;
        second_roadGO_road.start = roadAnchors[1];
        second_roadGO_road.AutoSetAllControlPoints();

        roadList.Add(second_roadGO_road);

        return roadAnchors[0];
    }

    public void ConnectTwoIntersections(Vector3 firstPosition, RoadAnchor firstNorth, RoadAnchor secondNorth)
    {
        GameObject bet_intersection_roadGO;
        bet_intersection_roadGO = Instantiate(roadPrefab, roadSystem.transform);
        bet_intersection_roadGO.transform.localPosition = firstPosition;
        bet_intersection_roadGO.transform.localRotation = Quaternion.identity;
        bet_intersection_roadGO.name = "link";
        Road bet_intersection_roadGO_road = bet_intersection_roadGO.GetComponent<Road>();
        bet_intersection_roadGO_road.start = firstNorth;
        bet_intersection_roadGO_road.end = secondNorth;
        bet_intersection_roadGO_road.AutoSetAllControlPoints();

        roadList.Add(bet_intersection_roadGO_road);
    }

    public void RebuildAffectedRoad()
    {
        if (roadList.Count > 0)
        {
            roadSystem.ConstructGraph();
            foreach (var road in roadList)
            {
                road.OnCurveChanged(true);
            }
            roadList.Clear();
        }
    }
}
