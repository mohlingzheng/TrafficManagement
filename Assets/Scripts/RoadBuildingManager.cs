using Barmetler.RoadSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RoadBuildingManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject RoadSmallPrefab;
    public GameObject RoadLargePrefab;
    public GameObject Intersection3SmallPrefab;
    public GameObject Intersection4SmallPrefab;
    public GameObject Intersection3LargePrefab;
    public GameObject Intersection4LargePrefab;
    public GameObject TransitionPrefab;

    public RoadSystem roadSystem;
    public RoadSystem previewRoadSystem;

    
    public List<Road> roadList = new List<Road>();
    public List<Road> previewRoadList = new List<Road>();

    public Material TransparentMaterial;
    public int count = 1;
    const int maxCount = 3;
    const int minCount = 1;


    void Start()
    {
        
    }

    void FixedUpdate()
    {
        RebuildAffectedRoad();
        MakeTransparent(previewRoadSystem.transform);
    }

    public RoadAnchor CreatePreviewRoad2(GameObject roadSystem, GameObject roadGameObject, Vector3 position, BuildMode buildMode)
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
            first_int = CreateObjectAtPosition(roadSystem, Intersection3SmallPrefab, road1_start.transform.parent.localPosition, road1_start.transform.parent.rotation);
            second_int = CreateObjectAtPosition(roadSystem, Intersection3SmallPrefab, road1_end.transform.parent.localPosition, road1_end.transform.parent.rotation);
            first_int.name = count.ToString();
            second_int.name = count.ToString();
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
        GameObject middle_interesection = CreateObjectAtPosition(roadSystem, Intersection3SmallPrefab, position, rotation);

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
        GameObject first_roadGO = CreateObjectAtPosition(roadSystem, RoadSmallPrefab, first_int.transform.position, Quaternion.identity);

        // create second road
        GameObject second_roadGO = CreateObjectAtPosition(roadSystem, RoadSmallPrefab, second_int.transform.position, Quaternion.identity);


        Road first_roadGO_road = first_roadGO.GetComponent<Road>();
        LinkRoadToStartEndAnchor(first_roadGO_road, road1_start, middle_first);

        Road second_roadGO_road = second_roadGO.GetComponent<Road>();
        LinkRoadToStartEndAnchor(second_roadGO_road, middle_second, road1_end);

        if (buildMode == BuildMode.Preview)
        {
            previewRoadList.Add(first_roadGO_road);
            previewRoadList.Add(second_roadGO_road);
            first_roadGO_road.name = count.ToString();
            second_roadGO_road.name = count.ToString();
            middle_interesection.name = count.ToString();
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

    public RoadAnchor CreatePreviewRoad(GameObject roadSystem, GameObject roadGameObject, Vector3 position, BuildMode buildMode)
    {
        RoadAnchor[] roadAnchors;

        // get start and end Anchors
        Road road = roadGameObject.GetComponent<Road>();
        RoadAnchor road1_start = road.start;
        RoadAnchor road1_end = road.end;
        GameObject gb1_start = road1_start.transform.parent.gameObject;
        GameObject gb1_end = road1_end.transform.parent.gameObject;


        // create first intersection
        GameObject first_int, second_int;

        GameObject intersectionPrefab = null;
        GameObject roadPrefab = null;
        if (Tag.CompareTags(roadGameObject.transform, Tag.Road_Large))
        {
            intersectionPrefab = Intersection3LargePrefab;
            roadPrefab = RoadLargePrefab;
        }
        else if (Tag.CompareTags(roadGameObject.transform, Tag.Road_Small))
        {
            intersectionPrefab = Intersection3SmallPrefab;
            roadPrefab = RoadSmallPrefab;
        }


        if (buildMode == BuildMode.Preview)
        {
            first_int = CreateObjectAtPosition(roadSystem, gb1_start, gb1_start.transform.localPosition, gb1_start.transform.rotation);
            second_int = CreateObjectAtPosition(roadSystem, gb1_end, gb1_end.transform.localPosition, gb1_end.transform.rotation);
            first_int.name = count.ToString();
            second_int.name = count.ToString();
        }
        else
        {
            first_int = gb1_start;
            second_int = gb1_end;
            road.start = null;
            road.end = null;
            Destroy(roadGameObject);
        }

        // create intersection in middle
        Vector3 direction = (road1_start.transform.position - road1_end.transform.position).normalized;
        direction = Vector3.Cross(direction, Vector3.up).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        GameObject middle_interesection = CreateObjectAtPosition(roadSystem, intersectionPrefab, position, rotation);

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
        GameObject first_roadGO = CreateObjectAtPosition(roadSystem, roadPrefab, first_int.transform.position, Quaternion.identity);

        // create second road
        GameObject second_roadGO = CreateObjectAtPosition(roadSystem, roadPrefab, second_int.transform.position, Quaternion.identity);


        Road first_roadGO_road = first_roadGO.GetComponent<Road>();
        LinkRoadToStartEndAnchor(first_roadGO_road, road1_start, middle_first);

        Road second_roadGO_road = second_roadGO.GetComponent<Road>();
        LinkRoadToStartEndAnchor(second_roadGO_road, middle_second, road1_end);

        if (buildMode == BuildMode.Preview)
        {
            previewRoadList.Add(first_roadGO_road);
            previewRoadList.Add(second_roadGO_road);
            first_roadGO_road.name = count.ToString();
            second_roadGO_road.name = count.ToString();
            middle_interesection.name = count.ToString();
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

    public RoadAnchor CreatePreviewIntersection(GameObject roadSystem, GameObject intersectionGameObject, Vector3 position, BuildMode buildMode)
    {
        // get details of clicked intersection
        Quaternion rotation = intersectionGameObject.transform.rotation;
        RoadAnchor[] roadAnchors = intersectionGameObject.GetComponentsInChildren<RoadAnchor>();

        // create intersection
        GameObject intersection;
        intersection = CreateObjectAtPosition(roadSystem, Intersection4SmallPrefab, position, rotation);
        RoadAnchor[] newRoadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();

        // for preview, build the connected roads and intersections
        if (buildMode == BuildMode.Preview)
        {
            foreach (RoadAnchor roadAnchor in roadAnchors)
            {
                if (roadAnchor.GetConnectedRoad() != null)
                {
                    Road road = roadAnchor.GetConnectedRoad();
                    RoadAnchor roadAnchor1 = road.start;
                    RoadAnchor roadAnchor2 = road.end;
                    GameObject roadObject, anotherIntersection;
                    Road newRoad;
                    if (roadAnchor == roadAnchor1)
                    {
                        anotherIntersection = CreateObjectAtPosition(roadSystem, roadAnchor2.transform.parent.gameObject, roadAnchor2.transform.position, roadAnchor2.transform.rotation);
                        RoadAnchor[] anotherRoadAnchors = anotherIntersection.GetComponentsInChildren<RoadAnchor>();
                        roadObject = CreateObjectAtPosition(roadSystem, RoadSmallPrefab, roadAnchor2.transform.position, Quaternion.identity);
                        newRoad = roadObject.GetComponent<Road>();
                        newRoad.start = GetClosetRoadAnchor(anotherRoadAnchors, roadObject);
                        newRoad.end = GetClosetRoadAnchor(newRoadAnchors, roadAnchor.gameObject);
                        
                    }
                    else
                    {
                        anotherIntersection = CreateObjectAtPosition(roadSystem, roadAnchor1.transform.parent.gameObject, roadAnchor1.transform.position, roadAnchor1.transform.rotation);
                        RoadAnchor[] anotherRoadAnchors = anotherIntersection.GetComponentsInChildren<RoadAnchor>();
                        roadObject = CreateObjectAtPosition(roadSystem, RoadSmallPrefab, roadAnchor1.transform.position, Quaternion.identity);
                        newRoad = roadObject.GetComponent<Road>();
                        newRoad.start = GetClosetRoadAnchor(anotherRoadAnchors, roadObject);
                        newRoad.end = GetClosetRoadAnchor(newRoadAnchors, roadAnchor.gameObject);
                    }
                    newRoad.Clear();
                    newRoad.RefreshEndPoints();
                    previewRoadList.Add(newRoad);
                }
            }
        }
        // for actual, disconnect road from intersection3 and connect to new intersection
        else
        {
            foreach (var roadAnchor in roadAnchors)
            {
                if (roadAnchor.GetConnectedRoad() != null)
                {
                    Road road = roadAnchor.GetConnectedRoad();
                    RoadAnchor anchor = GetClosetRoadAnchorFromPosition(newRoadAnchors, roadAnchor.transform.position);
                    if (road.start == roadAnchor)
                    {
                        roadAnchor.Disconnect();
                        road.start = anchor;
                    }
                    else
                    {
                        roadAnchor.Disconnect();
                        road.end = anchor;
                    }
                    road.Clear();
                    road.RefreshEndPoints();
                    roadList.Add(road);
                }
            }
            Destroy(intersectionGameObject);
        }

        return GetRoadAnchorWithoutConnection(newRoadAnchors);

    }
    
    public void ConnectTwoIntersections(GameObject roadSystem, RoadAnchor firstRoadAnchor, RoadAnchor secondRoadAnchor, BuildMode buildMode)
    {
        ValidateRotation(firstRoadAnchor, secondRoadAnchor);
        GameObject road_in_middle = null;
        RoadAnchor anchorLarge = null;
        RoadAnchor anchorSmall = null;
        if (Tag.CompareTags(firstRoadAnchor.transform.parent, Tag.Intersection_3_Large, Tag.Intersection_4_Large) 
            && Tag.CompareTags(secondRoadAnchor.transform.parent, Tag.Intersection_3_Large, Tag.Intersection_4_Large))
        {
            road_in_middle = RoadLargePrefab;
        }
        else if (Tag.CompareTags(firstRoadAnchor.transform.parent, Tag.Intersection_3_Small, Tag.Intersection_4_Small)
            && Tag.CompareTags(secondRoadAnchor.transform.parent, Tag.Intersection_3_Small, Tag.Intersection_4_Small))
        {
            road_in_middle = RoadSmallPrefab;
        }
        else if (Tag.CompareTags(firstRoadAnchor.transform.parent, Tag.Intersection_3_Large, Tag.Intersection_4_Large)
            && Tag.CompareTags(secondRoadAnchor.transform.parent, Tag.Intersection_3_Small, Tag.Intersection_4_Small))
        {
            road_in_middle = RoadSmallPrefab;
            anchorLarge = firstRoadAnchor;
            anchorSmall = secondRoadAnchor;
        }
        else if (Tag.CompareTags(firstRoadAnchor.transform.parent, Tag.Intersection_3_Small, Tag.Intersection_4_Small)
            && Tag.CompareTags(secondRoadAnchor.transform.parent, Tag.Intersection_3_Large, Tag.Intersection_4_Large))
        {
            road_in_middle = RoadSmallPrefab;
            anchorLarge = secondRoadAnchor;
            anchorSmall = firstRoadAnchor;
        }
        else
        {
            anchorLarge = null;
            anchorSmall = null;
        }

        Debug.Log(anchorLarge == null);

        if (anchorLarge == null || anchorSmall == null)
        {
            GameObject bet_intersection_roadGO = CreateObjectAtPosition(roadSystem, RoadSmallPrefab, firstRoadAnchor.transform.parent.position, Quaternion.identity);
            Road bet_intersection_roadGO_road = bet_intersection_roadGO.GetComponent<Road>();
            LinkRoadToStartEndAnchor(bet_intersection_roadGO_road, firstRoadAnchor, secondRoadAnchor);
            if (buildMode == BuildMode.Preview)
            {
                bet_intersection_roadGO.name = count.ToString();
                previewRoadList.Add(bet_intersection_roadGO_road);
            }
            else
            {
                roadList.Add(bet_intersection_roadGO_road);
            }
        }
        else
        {
            GameObject bet_intersection_roadGO = CreateObjectAtPosition(roadSystem, road_in_middle, anchorSmall.transform.parent.position, Quaternion.identity);
            Road bet_intersection_roadGO_road = bet_intersection_roadGO.GetComponent<Road>();

            GameObject bet_transition_raodGO = CreateObjectAtPosition(roadSystem, RoadLargePrefab, anchorLarge.transform.parent.position, Quaternion.identity);
            Road bet_transition_roadGO_road = bet_transition_raodGO.GetComponent<Road>();

            GameObject transition = CreateObjectAtPosition(roadSystem, TransitionPrefab, anchorLarge.transform.parent.position + anchorLarge.transform.forward * 50f, Quaternion.identity);

            Vector3 TransitionDirection = anchorLarge.transform.localScale - transition.transform.position;

            transition.transform.LookAt(TransitionDirection);

            RoadAnchor[] roadAnchors = transition.GetComponentsInChildren<RoadAnchor>();
            RoadAnchor transition_4;
            RoadAnchor transition_2;
            if (roadAnchors[0].transform.name == "Anchor 4 Lanes")
            {
                transition_4 = roadAnchors[0];
                transition_2 = roadAnchors[1];
            }
            else
            {
                transition_2 = roadAnchors[0];
                transition_4 = roadAnchors[1];
            }

            LinkRoadToStartEndAnchor(bet_intersection_roadGO_road, transition_2, anchorSmall);
            LinkRoadToStartEndAnchor(bet_transition_roadGO_road, anchorLarge, transition_4);

            if (buildMode == BuildMode.Preview)
            {
                bet_transition_raodGO.name = count.ToString();
                bet_intersection_roadGO.name = count.ToString();
                transition.name = count.ToString();
                previewRoadList.Add(bet_intersection_roadGO_road);
                previewRoadList.Add(bet_transition_roadGO_road);
            }
            else
            {
                roadList.Add(bet_intersection_roadGO_road);
                roadList.Add(bet_transition_roadGO_road);
            }
        }
    }

    public void ValidateRotation(RoadAnchor firstRoadAnchor, RoadAnchor secondRoadAnchor)
    {
        Vector3 direction = (secondRoadAnchor.transform.position - firstRoadAnchor.transform.position).normalized;
        Quaternion rotation;
        float angle;

        if (!Tag.CompareTags(firstRoadAnchor.transform.parent, Tag.Intersection_4_Small, Tag.Intersection_4_Large))
        {
            rotation = Quaternion.LookRotation(direction);
            angle = Quaternion.Angle(rotation, firstRoadAnchor.transform.rotation);
            firstRoadAnchor.transform.parent.localRotation = rotation;
            if (angle > 90f)
                ExchangeRoadConnected(firstRoadAnchor);
        }

        if (!Tag.CompareTags(secondRoadAnchor.transform.parent, Tag.Intersection_4_Small, Tag.Intersection_4_Large))
        {
            rotation = Quaternion.LookRotation(-direction);
            angle = Quaternion.Angle(rotation, secondRoadAnchor.transform.rotation);
            secondRoadAnchor.transform.parent.localRotation = rotation;
            if (angle > 90f)
                ExchangeRoadConnected(secondRoadAnchor);
        }
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
        if (roadSystem == this.previewRoadSystem)
        {
            createdObject.name = count.ToString();
        }
        return createdObject;
    }

    public RoadAnchor GetClosetRoadAnchor(RoadAnchor[] roadAnchors, GameObject gameObject)
    {
        Vector3 position = gameObject.transform.position;
        RoadAnchor selected = null;
        float closedDistance = float.PositiveInfinity;
        for (int i = 0; i < roadAnchors.Length; i++)
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

    public RoadAnchor GetClosetRoadAnchorFromPosition(RoadAnchor[] roadAnchors, Vector3 position)
    {
        RoadAnchor selected = null;
        float closedDistance = float.PositiveInfinity;
        for (int i = 0; i < roadAnchors.Length; i++)
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
            Debug.Log("reset");
            roadSystem.ConstructGraph();
            foreach (var road in roadList)
            {
                road.OnCurveChanged(true);
            }
            roadSystem.RebuildAllRoads();
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

    public void IncreaseCount()
    {
        count++;
        count = Mathf.Clamp(count, minCount, maxCount);
        Debug.Log("++");
    }

    public void ReduceCount()
    {
        count--;
        count = Mathf.Clamp(count, minCount, maxCount);
        Debug.Log("--");
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
