using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class IntersectionManager : MonoBehaviour
{
    public GameObject[] intersection3;
    public GameObject[] intersection4;
    float rayLength = 2f;
    string[] currentState = { };
    string[] NorthSouth = { "Anchor North", "Anchor South" };
    string[] EastWest = { "Anchor East", "Anchor West" };

    void Start()
    {
        intersection3 = GameObject.FindGameObjectsWithTag("Intersection3");
        intersection4 = GameObject.FindGameObjectsWithTag("Intersection4");
        StartCoroutine(MoveAndStart());
    }

    void Update()
    {
        SetupTrafficLight();
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

    public void SetupTrafficLight()
    {
        foreach (var intersection in intersection4)
        {
            RoadAnchor[] roadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();
            foreach (var roadAnchor in roadAnchors)
            {
                RaycastHit hit;
                Vector3 position = GetRayPositionFromRoadAnchor(roadAnchor);
                Vector3 direction = -roadAnchor.transform.right;
                bool isInside = IsStringInsideArray(currentState, roadAnchor.name);
                
                DebugRay(position, direction, isInside);

                if (Physics.Raycast(position, direction, out hit, rayLength))
                {
                    if (hit.collider.gameObject.CompareTag("Vehicle"))
                    {
                        if (isInside)
                        {
                            hit.collider.gameObject.GetComponent<VehicleMovement>().VehicleMove();
                        }
                        else
                        {
                            hit.collider.gameObject.GetComponent<VehicleMovement>().VehicleStop();
                        }
                    }
                }
            }
        }

        foreach (var intersection in intersection3)
        {
            RoadAnchor[] roadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();
            foreach (var roadAnchor in roadAnchors)
            {
                RaycastHit hit;
                Vector3 position = GetRayPositionFromRoadAnchor(roadAnchor);
                Vector3 direction = -roadAnchor.transform.right;
                bool isInside = IsStringInsideArray(currentState, roadAnchor.name);

                DebugRay(position, direction, isInside);

                if (Physics.Raycast(position, direction, out hit, rayLength))
                {
                    if (hit.collider.gameObject.CompareTag("Vehicle"))
                    {
                        if (isInside)
                        {
                            hit.collider.gameObject.GetComponent<VehicleMovement>().VehicleMove();
                        }
                        else
                        {
                            hit.collider.gameObject.GetComponent<VehicleMovement>().VehicleStop();
                        }
                    }
                }
            }
        }
    }

    Vector3 GetRayPositionFromRoadAnchor(Barmetler.RoadSystem.RoadAnchor roadAnchor)
    {
        Vector3 position = roadAnchor.transform.position;
        position = position + roadAnchor.transform.rotation * Vector3.right * 4f;
        position.y += 1f;
        return position;
    }

    public void DebugRay(Vector3 position, Vector3 dir, bool isInside)
    {
        if (isInside)
        {
            Debug.DrawRay(position, dir * rayLength, Color.green);
        }
        else
        {
            Debug.DrawRay(position, dir * rayLength, Color.red);
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
}
