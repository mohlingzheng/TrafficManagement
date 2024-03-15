using Barmetler.RoadSystem;
using UnityEditor;
using UnityEngine;

public class RoadBuildingManager : MonoBehaviour
{
    public RoadSystem roadSystem;
    public GameObject road1GO;
    public GameObject road2GO;
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
        Vector3 firstPositionClicked = new Vector3(316, 0, 298);
        Vector3 secondPositionClicked = new Vector3(438f, 0f, 314.2f);
        Road road1_road = road1GO.GetComponent<Road>();
        RoadAnchor road1_start = road1_road.start;
        RoadAnchor road1_end = road1_road.end;
        road1_road.start.Disconnect();
        road1_road.end.Disconnect();
        road1_road.Clear();
        road1_road.RefreshEndPoints();
        Destroy(road1GO.GetComponent<RoadMeshGenerator>());
        Destroy(road1_road);
        Destroy(road1GO);


        GameObject first_roadGO;
        first_roadGO = Instantiate(roadGameObject, roadSystem.transform);
        first_roadGO.transform.localPosition = road1_start.transform.position;
        first_roadGO.transform.localRotation = Quaternion.identity;
        first_roadGO.name = "road 1 first split";

        GameObject first_interesection;
        first_interesection = Instantiate(intersectionGB, roadSystem.transform);
        first_interesection.transform.localPosition = firstPositionClicked;
        first_interesection.transform.localRotation = Quaternion.Euler(0, 50f, 0);
        first_interesection.name = "first intersection";
        RoadAnchor[] roadAnchors = first_interesection.GetComponentsInChildren<RoadAnchor>();

        Road first_roadGO_road = first_roadGO.GetComponent<Road>();
        first_roadGO_road.end = roadAnchors[1];
        first_roadGO_road.start = road1_start;
        first_roadGO_road.AutoSetAllControlPoints();

        GameObject second_roadGO;
        second_roadGO = Instantiate(roadGameObject, roadSystem.transform);
        second_roadGO.transform.localPosition = road1_end.transform.position;
        second_roadGO.transform.localRotation = Quaternion.identity;
        second_roadGO.name = "road 1 second split";

        Road second_roadGO_road = second_roadGO.GetComponent<Road>();
        second_roadGO_road.end = road1_end;
        second_roadGO_road.start = roadAnchors[2];

        second_roadGO_road.AutoSetAllControlPoints();

        // 

        Road road2_road = road2GO.GetComponent<Road>();
        RoadAnchor road2_end = road2_road.end;
        RoadAnchor road2_start = road2_road.start;
        road2_road.end.Disconnect();
        road2_road.Clear();
        road2_road.RefreshEndPoints();
        Destroy(road2GO.GetComponent<RoadMeshGenerator>());
        Destroy(road2_road);
        Destroy(road2GO);

        GameObject first_roadGO2;
        first_roadGO2 = Instantiate(roadGameObject, roadSystem.transform);
        first_roadGO2.transform.localPosition = road2_end.transform.position;
        first_roadGO2.transform.localRotation = Quaternion.identity;
        first_roadGO2.name = "road 2 first split";

        GameObject second_intersection;
        second_intersection = Instantiate(intersectionGB, roadSystem.transform);
        second_intersection.transform.localPosition = secondPositionClicked;
        second_intersection.transform.localRotation = Quaternion.Euler(0, -90f, 0);
        second_intersection.name = "second intersection";
        RoadAnchor[] roadAnchors2 = second_intersection.GetComponentsInChildren<RoadAnchor>();

        Road first_roadGO2_road = first_roadGO2.GetComponent<Road>();
        first_roadGO2_road.start = road2_start;
        first_roadGO2_road.end = roadAnchors2[2];
        first_roadGO2_road.AutoSetAllControlPoints();

        GameObject second_roadGO2;
        second_roadGO2 = Instantiate(roadGameObject, roadSystem.transform);
        second_roadGO2.transform.localPosition = road2_end.transform.position;
        second_roadGO2.transform.localRotation = Quaternion.identity;
        second_roadGO2.name = "road 2 second split";

        Road second_roadGO2_road = second_roadGO2.GetComponent<Road>();
        second_roadGO2_road.end = road2_end;
        second_roadGO2_road.start = roadAnchors2[1];

        second_roadGO2_road.AutoSetAllControlPoints();

        //

        GameObject bet_intersection_roadGO;
        bet_intersection_roadGO = Instantiate(roadGameObject, roadSystem.transform);
        bet_intersection_roadGO.transform.localPosition = firstPositionClicked;
        bet_intersection_roadGO.transform.localRotation = Quaternion.identity;
        bet_intersection_roadGO.name = "link";
        Road bet_intersection_roadGO_road = bet_intersection_roadGO.GetComponent<Road>();
        bet_intersection_roadGO_road.start = roadAnchors[0];
        bet_intersection_roadGO_road.end = roadAnchors2[0];
        second_roadGO2_road.AutoSetAllControlPoints();

        road1GO.GetComponent<RoadMeshGenerator>().GenerateRoadMesh();



        //GameObject road1split_1, road1split_2, road1split_intersection;


        //ras[1].SetRoad(road1.GetComponent<Road>());
        //road.end = ras[1];


        //road1split_2 = Instantiate(roadGameObject, end.transform.position, Quaternion.identity);
        //road1split_2.GetComponent<Road>().end = end;
        //road1split_2.name = "road1split_2";
        //Road road1split_2_Road = road1split_2.GetComponent<Road>();
        //ras[2].SetRoad(road1split_2.GetComponent<Road>());
        //road1split_2_Road.

        //GameObject[] vehicles = GameObject.FindGameObjectsWithTag("Vehicle");
        //foreach (GameObject vehicle in vehicles)
        //{
        //    vehicle.GetComponent<RoadSystemNavigator>().CalculateWayPointsSync();
        //}

        roadSystem.RebuildAllRoads();

        SceneView.RepaintAll();

    }
}
