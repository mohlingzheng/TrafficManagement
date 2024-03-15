using Barmetler.RoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class IntersectionManager : MonoBehaviour
{
    public GameObject[] intersection3;
    public GameObject[] intersection4;
    string[] currentState = { };
    string[] NorthSouth = { "Anchor North", "Anchor South" };
    string[] EastWest = { "Anchor East", "Anchor West" };
    public GameObject TrafficLightBlockGameObject;

    void Start()
    {
        intersection3 = GameObject.FindGameObjectsWithTag("Intersection3");
        intersection4 = GameObject.FindGameObjectsWithTag("Intersection4");
        StartCoroutine(MoveAndStart());
    }

    void FixedUpdate()
    {
        GenerateTrafficLightBlock();
    }

    private void GenerateTrafficLightBlock()
    {
        foreach (var intersection in intersection4)
        {
            RoadAnchor[] roadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();
            foreach (var roadAnchor in roadAnchors)
            {
                
                GameObject block;
                if (roadAnchor.transform.childCount <= 0)
                    block = Instantiate(TrafficLightBlockGameObject, roadAnchor.transform);
                else
                    block = roadAnchor.transform.GetChild(0).gameObject;

                if (block.transform.position != GetRayPositionFromRoadAnchor(roadAnchor) || block.transform.rotation != roadAnchor.transform.rotation)
                {
                    block.transform.SetPositionAndRotation(GetRayPositionFromRoadAnchor(roadAnchor), roadAnchor.transform.rotation);
                }
                if (IsStringInsideArray(currentState, roadAnchor.name))
                {
                    block.GetComponent<TrafficLightLogic>().SetCurrentState(TrafficLightState.Green);
                }
                else
                {
                    block.GetComponent<TrafficLightLogic>().SetCurrentState(TrafficLightState.Red);
                }

            }
        }

        foreach (var intersection in intersection3)
        {
            RoadAnchor[] roadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();
            foreach (var roadAnchor in roadAnchors)
            {
                GameObject block;
                if (roadAnchor.transform.childCount <= 0)
                    block = Instantiate(TrafficLightBlockGameObject, roadAnchor.transform);
                else
                    block = roadAnchor.transform.GetChild(0).gameObject;

                if (block.transform.position != GetRayPositionFromRoadAnchor(roadAnchor) || block.transform.rotation != roadAnchor.transform.rotation)
                {
                    block.transform.SetPositionAndRotation(GetRayPositionFromRoadAnchor(roadAnchor), roadAnchor.transform.rotation);
                }
                if (IsStringInsideArray(currentState, roadAnchor.name))
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
            yield return new WaitForSeconds(3f);
            currentState = EastWest;
            yield return new WaitForSeconds(3f);
        }
    }

    Vector3 GetRayPositionFromRoadAnchor(Barmetler.RoadSystem.RoadAnchor roadAnchor)
    {
        Vector3 position = roadAnchor.transform.position;
        position = position + roadAnchor.transform.rotation * Vector3.right * 2.2f;
        position.y += 1f;
        return position;
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
}
