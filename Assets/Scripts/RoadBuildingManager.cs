using Barmetler.RoadSystem;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class RoadBuildingManager : MonoBehaviour
{
    public RoadSystem roadSystem;
    public RoadSystem previewRoadSystem;
    public GameObject RoadPrefab;
    public GameObject Intersection3Prefab;
    public List<Road> roadList = new List<Road>();
    public List<Road> previewRoadList = new List<Road>();

    public Material TransparentMaterial;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RebuildAffectedRoad();
        MakeTransparent(previewRoadSystem.transform);
    }

    public RoadAnchor CreatePreviewRoad(GameObject roadSystem, GameObject roadGameObject, Vector3 position, BuildMode buildMode)
    {
        RoadAnchor[] roadAnchors;

        // get start and end Anchors
        Road road = roadGameObject.GetComponent<Road>();
        RoadAnchor road1_start = road.start;
        RoadAnchor road1_end = road.end;

        // create first intersection
        GameObject first_int, second_int;
        if (buildMode == BuildMode.Preview)
        {
            first_int = CreateObjectAtPosition(roadSystem, Intersection3Prefab, road1_start.transform.parent.localPosition, road1_start.transform.parent.rotation);
            second_int = CreateObjectAtPosition(roadSystem, Intersection3Prefab, road1_end.transform.parent.localPosition, road1_end.transform.parent.rotation);
        }

        else
        {
            first_int = road1_start.transform.parent.gameObject;
            second_int = road1_end.transform.parent.gameObject;
            road.start = null;
            road.end = null;
            Destroy(roadGameObject);
        }

        // create intersection in middle
        Vector3 direction = (road1_start.transform.position - road1_end.transform.position).normalized;
        direction = Vector3.Cross(direction, Vector3.up).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        GameObject middle_interesection = CreateObjectAtPosition(roadSystem, Intersection3Prefab, position, rotation);

        // get correct Anchor from first intersection
        roadAnchors = first_int.GetComponentsInChildren<RoadAnchor>();
        road1_start = GetClosetRoadAnchor(roadAnchors, middle_interesection);

        // get correct Anchor from second intersection
        roadAnchors = second_int.GetComponentsInChildren<RoadAnchor>();
        road1_end = GetClosetRoadAnchor(roadAnchors, middle_interesection);

        // get correct Anchor from middle intersection to first and second
        roadAnchors = middle_interesection.GetComponentsInChildren<RoadAnchor>();
        RoadAnchor middle_first = GetClosetRoadAnchor(roadAnchors, first_int);
        RoadAnchor middle_second = GetClosetRoadAnchor(roadAnchors, second_int);

        // create first road
        GameObject first_roadGO = CreateObjectAtPosition(roadSystem, RoadPrefab, first_int.transform.position, Quaternion.identity);

        // create second road
        GameObject second_roadGO = CreateObjectAtPosition(roadSystem, RoadPrefab, second_int.transform.position, Quaternion.identity);


        Road first_roadGO_road = first_roadGO.GetComponent<Road>();
        LinkRoadToStartEndAnchor(first_roadGO_road, road1_start, middle_first);

        Road second_roadGO_road = second_roadGO.GetComponent<Road>();
        LinkRoadToStartEndAnchor(second_roadGO_road, middle_second, road1_end);

        if (buildMode == BuildMode.Preview)
        {
            previewRoadList.Add(first_roadGO_road);
            previewRoadList.Add(second_roadGO_road);
        }
        else
        {
            roadList.Add(first_roadGO_road);
            roadList.Add(second_roadGO_road);
        }

        first_roadGO_road.Clear();
        first_roadGO_road.RefreshEndPoints();

        second_roadGO_road.Clear();
        second_roadGO_road.RefreshEndPoints();

        return GetRoadAnchorWithoutConnection(roadAnchors);
    }

    public void ConnectTwoIntersections(GameObject roadSystem, RoadAnchor firstRoadAnchor, RoadAnchor secondRoadAnchor, BuildMode buildMode)
    {
        ValidateRotation(firstRoadAnchor, secondRoadAnchor);
        GameObject bet_intersection_roadGO = CreateObjectAtPosition(roadSystem, RoadPrefab, firstRoadAnchor.transform.parent.position, Quaternion.identity);
        Road bet_intersection_roadGO_road = bet_intersection_roadGO.GetComponent<Road>();
        LinkRoadToStartEndAnchor(bet_intersection_roadGO_road, firstRoadAnchor, secondRoadAnchor);
        if (buildMode == BuildMode.Preview)
        {
            previewRoadList.Add(bet_intersection_roadGO_road);
        }
        else
        {
            roadList.Add(bet_intersection_roadGO_road);
        }
    }

    public void ValidateRotation(RoadAnchor firstRoadAnchor, RoadAnchor secondRoadAnchor)
    {
        Vector3 direction = (secondRoadAnchor.transform.position - firstRoadAnchor.transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(rotation, firstRoadAnchor.transform.rotation);
        firstRoadAnchor.transform.parent.localRotation = rotation;
        if (angle > 90f)
            ExchangeRoadConnected(firstRoadAnchor);

        rotation = Quaternion.LookRotation(-direction);
        angle = Quaternion.Angle(rotation, secondRoadAnchor.transform.rotation);
        secondRoadAnchor.transform.parent.localRotation = rotation;
        if (angle > 90f)
            ExchangeRoadConnected(secondRoadAnchor);
    }

    public void ExchangeRoadConnected(RoadAnchor originalRoadAnchor)
    {
        RoadAnchor[] roadAnchors = originalRoadAnchor.transform.parent.GetComponentsInChildren<RoadAnchor>();
        RoadAnchor first = null;
        RoadAnchor second = null;
        foreach (var roadAnchor in roadAnchors)
        {
            if (roadAnchor.GetConnectedRoad() != null)
            {
                if (first == null)
                    first = roadAnchor;
                else
                    second = roadAnchor;
            }
        }
        Road firstRoad = first.GetConnectedRoad();
        Road secondRoad = second.GetConnectedRoad();
        first.Disconnect();
        second.Disconnect();
        if (firstRoad.start == null)
            firstRoad.start = second;
        else
            firstRoad.end = second;

        if (secondRoad.start == null)
            secondRoad.start = first;
        else
            secondRoad.end = first;
    }

    public RoadAnchor GetRoadAnchorWithoutConnection(RoadAnchor[] roadAnchors)
    {
        foreach (var roadAnchor in roadAnchors)
        {
            if (roadAnchor.GetConnectedRoad() == null)
                return roadAnchor;
        }
        return null;
    }

    GameObject CreateObjectAtPosition(GameObject roadSystem, GameObject ObjectPrefab, Vector3 position, Quaternion rotation)
    {
        GameObject createdObject;
        createdObject = Instantiate(ObjectPrefab, roadSystem.transform);
        createdObject.transform.localPosition = position;
        createdObject.transform.localRotation = rotation;
        return createdObject;
    }

    public RoadAnchor GetClosetRoadAnchor(RoadAnchor[] roadAnchors, GameObject interection)
    {
        Vector3 position = interection.transform.position;
        RoadAnchor selected = roadAnchors[0];
        float closedDistance = Vector3.Distance(roadAnchors[0].transform.position, position);
        for (int i = 1; i < roadAnchors.Length; i++)
        {
            float distance = Vector3.Distance(roadAnchors[i].transform.position, position);
            if (distance < closedDistance)
            {
                selected = roadAnchors[i];
                closedDistance = distance;
            }
        }
        return selected;
    }

    void LinkRoadToStartEndAnchor(Road road, RoadAnchor start, RoadAnchor end)
    {
        road.start = start;
        road.end = end;
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

        if (previewRoadList.Count > 0)
        {
            previewRoadSystem.ConstructGraph();
            foreach (var road in previewRoadList)
            {
                road.OnCurveChanged(true);
            }
            previewRoadList.Clear();
        }
    }

    private void MakeTransparent(Transform parent)
    {
        Renderer renderer = parent.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != TransparentMaterial)
                    materials[i] = TransparentMaterial;
            }
            renderer.materials = materials;
        }

        foreach (Transform child in parent)
        {
            MakeTransparent(child);
        }
    }
}
