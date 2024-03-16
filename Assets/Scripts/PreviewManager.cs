using Barmetler.RoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject RoadPrefab;
    public GameObject Intersection3Prefab;
    public GameObject Intersection4Prefab;

    public RoadSystem previewRoadSystem;
    public RoadSystem actualRoadSystem;

    [Header("Copy of Roads")]
    public Vector3 firstPosition;
    public GameObject firstRoad;

    [Header("Refresh")]
    public List<Road> roadList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MakeTransparent();
        RebuildAffectedRoad();
    }

    private void MakeTransparent()
    {
        
    }

    public void CreatePreviewRoad(GameObject roadGameObject, Vector3 position)
    {
        // Get the connected Anchor, then remove this gameobject
        Road road = roadGameObject.GetComponent<Road>();
        RoadAnchor road1_start = road.start;
        RoadAnchor road1_end = road.end;

        // create first intersection
        GameObject first_int;
        first_int = Instantiate(Intersection3Prefab, previewRoadSystem.transform);
        first_int.transform.localPosition = road1_start.transform.parent.localPosition;
        first_int.transform.localRotation = road1_start.transform.parent.localRotation;
        RoadAnchor[] first_int_Anchors = first_int.GetComponentsInChildren<RoadAnchor>();

        // create first road
        GameObject first_roadGO;
        first_roadGO = Instantiate(roadGameObject, previewRoadSystem.transform);
        first_roadGO.transform.localPosition = first_int_Anchors[0].transform.position;
        first_roadGO.transform.localRotation = Quaternion.identity;
        first_roadGO.name = "road 1 first split";

        Road first_roadGO_road = first_roadGO.GetComponent<Road>();
        first_roadGO_road.start = first_int_Anchors[0];

        // create second intersection
        GameObject second_int;
        second_int = Instantiate(Intersection3Prefab, previewRoadSystem.transform);
        second_int.transform.localPosition = road1_end.transform.parent.localPosition;
        second_int.transform.localRotation = road1_end.transform.parent.localRotation;
        RoadAnchor[] second_int_Anchors = second_int.GetComponentsInChildren<RoadAnchor>();

        // create second road
        GameObject second_roadGO;
        second_roadGO = Instantiate(roadGameObject, previewRoadSystem.transform);
        second_roadGO.transform.localPosition = second_int_Anchors[1].transform.position;
        second_roadGO.transform.localRotation = Quaternion.identity;
        second_roadGO.name = "road 1 second split";

        Road second_roadGO_road = second_roadGO.GetComponent<Road>();
        second_roadGO_road.end = second_int_Anchors[1];

        // create intersection
        Vector3 direction = (road1_start.transform.position - road1_end.transform.position).normalized;
        direction = Vector3.Cross(direction, Vector3.up).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject first_interesection;
        first_interesection = Instantiate(Intersection3Prefab, previewRoadSystem.transform);
        first_interesection.transform.localPosition = position;
        first_interesection.transform.localRotation = rotation;
        first_interesection.name = "first intersection";
        first_interesection.tag = Tag.Intersection3.ToString();

        RoadAnchor[] roadAnchors = first_interesection.GetComponentsInChildren<RoadAnchor>();
        first_roadGO_road.end = roadAnchors[1];
        second_roadGO_road.start = roadAnchors[2];


        roadList.Add(first_roadGO_road);
        roadList.Add(second_roadGO_road);

        first_roadGO_road.Clear();
        first_roadGO_road.RefreshEndPoints();

        second_roadGO_road.Clear();
        second_roadGO_road.RefreshEndPoints();

        //return roadAnchors[0];
    }

    public void RebuildAffectedRoad()
    {
        if (roadList.Count > 0)
        {
            previewRoadSystem.ConstructGraph();
            foreach (var road in roadList)
            {
                road.OnCurveChanged(true);
            }
            roadList.Clear();
        }
    }

}
