using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionManager : MonoBehaviour
{
    public GameObject[] intersection_3_small;
    public GameObject[] intersection_4_small;
    public GameObject[] intersection_3_large;
    public GameObject[] intersection_4_large;
    string[] currentState = { };
    string[] NorthSouth = { "Anchor North", "Anchor South" };
    string[] EastWest = { "Anchor East", "Anchor West" };
    string[] Empty = { };
    public GameObject TrafficLightBlockGameObject;
    public GameObject TrafficLightPrefab;

    void Start()
    {
        GetLatestIntersection();
        StartCoroutine(MoveAndStart());
    }

    void FixedUpdate()
    {
        GenerateTrafficLightBlock();
    }

    public void GetLatestIntersection()
    {
        intersection_3_small = GameObject.FindGameObjectsWithTag(Tag.Intersection_3_Small);
        intersection_4_small = GameObject.FindGameObjectsWithTag(Tag.Intersection_4_Small);
        intersection_3_large = GameObject.FindGameObjectsWithTag(Tag.Intersection_3_Large);
        intersection_4_large = GameObject.FindGameObjectsWithTag(Tag.Intersection_4_Large);
    }

    private void GenerateTrafficLightBlock()
    {
        foreach (var intersection in intersection_4_small)
        {
            if (intersection == null)
                continue;
            RoadAnchor[] roadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();
            foreach (var roadAnchor in roadAnchors)
            {
                
                GameObject block;
                block = (roadAnchor.transform.childCount <= 0) ? Instantiate(TrafficLightBlockGameObject, roadAnchor.transform) : roadAnchor.transform.GetChild(0).gameObject;

                if (block.transform.position != GetRayPositionFromRoadAnchor(roadAnchor) || block.transform.rotation != roadAnchor.transform.rotation)
                {
                    block.transform.SetPositionAndRotation(GetRayPositionFromRoadAnchor(roadAnchor), roadAnchor.transform.rotation);
                }
                TrafficLightStatusUpdate(block, currentState, roadAnchor.name);
                

                GameObject trafficLight;
                if (block.transform.childCount <= 0)
                {
                    trafficLight = Instantiate(TrafficLightPrefab, block.transform);
                    Vector3 position = block.transform.localPosition;
                    position.x -= 1.5f;
                    position.y -= 1.5f;
                    trafficLight.transform.localPosition = position;
                    trafficLight.transform.rotation = block.transform.rotation;
                    Vector3 parentScale = block.transform.localScale;
                    Vector3 oriScale = trafficLight.transform.localScale;
                    Vector3 newScale = new Vector3(oriScale.x/parentScale.x, oriScale.y/parentScale.y, oriScale.z/parentScale.z);
                    trafficLight.transform.localScale = newScale;
                }
                else
                {
                    trafficLight = block.transform.GetChild(0).gameObject;
                }
                TrafficLightSwitch(trafficLight, block);
                

            }
        }

        foreach (var intersection in intersection_3_small)
        {
            if (intersection == null)
                continue;
            RoadAnchor[] roadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();
            foreach (var roadAnchor in roadAnchors)
            {
                GameObject block;
                block = (roadAnchor.transform.childCount <= 0) ? Instantiate(TrafficLightBlockGameObject, roadAnchor.transform) : roadAnchor.transform.GetChild(0).gameObject;

                if (block.transform.position != GetRayPositionFromRoadAnchor(roadAnchor) || block.transform.rotation != roadAnchor.transform.rotation)
                {
                    block.transform.SetPositionAndRotation(GetRayPositionFromRoadAnchor(roadAnchor), roadAnchor.transform.rotation);
                }
                if (roadAnchor.name == "Anchor West")
                {
                    block.GetComponent<TrafficLightLogic>().SetCurrentState(TrafficLightState.Green);
                }
                else if (IsStringInsideArray(currentState, roadAnchor.name))
                {
                    block.GetComponent<TrafficLightLogic>().SetCurrentState(TrafficLightState.Green);
                }
                else
                {
                    block.GetComponent<TrafficLightLogic>().SetCurrentState(TrafficLightState.Red);
                }

            }
        }

        foreach (var intersection in intersection_4_large)
        {
            if (intersection == null)
                continue;
            RoadAnchor[] roadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();
            foreach (var roadAnchor in roadAnchors)
            {
                GameObject block;
                block = (roadAnchor.transform.childCount <= 0) ? Instantiate(TrafficLightBlockGameObject, roadAnchor.transform) : roadAnchor.transform.GetChild(0).gameObject;

                if (block.transform.position != GetRayPositionFromRoadAnchorLarge(roadAnchor) || block.transform.rotation != roadAnchor.transform.rotation)
                {
                    block.transform.SetPositionAndRotation(GetRayPositionFromRoadAnchorLarge(roadAnchor), roadAnchor.transform.rotation);
                }
                TrafficLightStatusUpdate(block, currentState, roadAnchor.name);

                GameObject trafficLight;
                if (block.transform.childCount <= 0)
                {
                    trafficLight = Instantiate(TrafficLightPrefab, block.transform);
                    Vector3 position = block.transform.localPosition;
                    position.x -= 0.8f;
                    position.y -= 1.5f;
                    trafficLight.transform.localPosition = position;
                    trafficLight.transform.rotation = block.transform.rotation;
                    Vector3 parentScale = block.transform.localScale;
                    Vector3 oriScale = trafficLight.transform.localScale;
                    Vector3 newScale = new Vector3(oriScale.x / parentScale.x, oriScale.y / parentScale.y, oriScale.z / parentScale.z);
                    trafficLight.transform.localScale = newScale;
                }
                else
                {
                    trafficLight = block.transform.GetChild(0).gameObject;
                }
                TrafficLightSwitch(trafficLight, block);
            }
        }

        foreach (var intersection in intersection_3_large)
        {
            if (intersection == null)
                continue;
            RoadAnchor[] roadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();
            foreach (var roadAnchor in roadAnchors)
            {
                GameObject block;
                block = (roadAnchor.transform.childCount <= 0) ? Instantiate(TrafficLightBlockGameObject, roadAnchor.transform) : roadAnchor.transform.GetChild(0).gameObject;

                if (block.transform.position != GetRayPositionFromRoadAnchorLarge(roadAnchor) || block.transform.rotation != roadAnchor.transform.rotation)
                {
                    block.transform.SetPositionAndRotation(GetRayPositionFromRoadAnchorLarge(roadAnchor), roadAnchor.transform.rotation);
                }
                if (roadAnchor.name == "Anchor West")
                {
                    block.GetComponent<TrafficLightLogic>().SetCurrentState(TrafficLightState.Green);
                }
                else if (IsStringInsideArray(currentState, roadAnchor.name))
                {
                    block.GetComponent<TrafficLightLogic>().SetCurrentState(TrafficLightState.Green);
                }
                else
                {
                    block.GetComponent<TrafficLightLogic>().SetCurrentState(TrafficLightState.Red);
                }

            }
        }
    }

    IEnumerator MoveAndStart()
    {
        while (true)
        {
            currentState = NorthSouth;
            yield return new WaitForSeconds(6f);

            currentState = Empty;
            yield return new WaitForSeconds(3f);

            currentState = EastWest;
            yield return new WaitForSeconds(6f);
        }
    }

    Vector3 GetRayPositionFromRoadAnchor(Barmetler.RoadSystem.RoadAnchor roadAnchor)
    {
        Vector3 position = roadAnchor.transform.position;
        position = position + roadAnchor.transform.rotation * Vector3.right * 2.2f;
        position.y += 1f;
        position = position - roadAnchor.transform.forward * 2.5f;
        return position;
    }
    Vector3 GetRayPositionFromRoadAnchorLarge(Barmetler.RoadSystem.RoadAnchor roadAnchor)
    {
        Vector3 position = roadAnchor.transform.position;
        position = position + roadAnchor.transform.rotation * Vector3.right * 3.5f;
        position.y += 1f;
        position = position - roadAnchor.transform.forward * 2.5f;
        return position;
    }

    void TrafficLightStatusUpdate(GameObject block, string[] currentState, string name)
    {
        if (IsStringInsideArray(currentState, name))
        {
            block.GetComponent<TrafficLightLogic>().SetCurrentState(TrafficLightState.Green);
        }
        else
        {
            block.GetComponent<TrafficLightLogic>().SetCurrentState(TrafficLightState.Red);
        }
    }

    bool IsStringInsideArray(string[] current, string name)
    {
        foreach (var item in current)
        {
            if (item == name)
            {
                return true;
            }
        }
        return false;
    }

    private void TrafficLightSwitch(GameObject trafficLight, GameObject block)
    {
        TrafficLightState state = block.GetComponent<TrafficLightLogic>().currentState;
        List <GameObject> TrafficLightPoints = new List<GameObject>();
        for (int i = 0; i < trafficLight.transform.childCount; i++)
        {
            if (state == TrafficLightState.Red)
            {
                Transform point = trafficLight.transform.GetChild(i);
                if (point.name == "Red")
                    point.gameObject.SetActive(true);
                else
                    point.gameObject.SetActive(false);
            }
            else if (state == TrafficLightState.Green)
            {
                Transform point = trafficLight.transform.GetChild(i);
                if (point.name == "Green")
                    point.gameObject.SetActive(true);
                else
                    point.gameObject.SetActive(false);
            }
        }
    }
}
