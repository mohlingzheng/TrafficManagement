using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IntersectionManager : MonoBehaviour
{
    public GameObject[] intersection3;
    public GameObject[] intersection4;
    float rayLength = 3.5f;
    bool state = false;
    // Start is called before the first frame update
    void Start()
    {
        intersection3 = GameObject.FindGameObjectsWithTag("Intersection3");
        intersection4 = GameObject.FindGameObjectsWithTag("Intersection4");
        StartCoroutine(MoveAndStart());
    }

    // Update is called once per frame
    void Update()
    {
        SetupTrafficLight();
    }

    IEnumerator MoveAndStart()
    {
        while (true)
        {
            state = !state;
            yield return new WaitForSeconds(2f);
        }
    }

    public void SetupTrafficLight()
    {
        foreach (var intersection in intersection4)
        {
            RoadAnchor[] roadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();
            foreach (var roadAchor in roadAnchors)
            {
                RaycastHit hit;
                Vector3 position = roadAchor.transform.position;
                position = position + roadAchor.transform.rotation * Vector3.right * 4f;
                position.y += 1f;
                if (Physics.Raycast(position, -roadAchor.transform.right, out hit, rayLength))
                {
                    if(hit.collider.gameObject.tag == "Vehicle")
                    {
                        if (state)
                        {
                            hit.collider.gameObject.GetComponent<VehicleMovement>().VehicleMove();
                        }
                        else
                        {
                            hit.collider.gameObject.GetComponent<VehicleMovement>().VehicleStop();
                        }
                    }
                }
                DebugRay(position, -roadAchor.transform.right);
            }
        }

        foreach (var intersection in intersection3)
        {
            RaycastHit hit;
            RoadAnchor[] roadAnchors = intersection.GetComponentsInChildren<RoadAnchor>();
            foreach (var roadAchor in roadAnchors)
            {
                Vector3 position = roadAchor.transform.position;
                position = position + roadAchor.transform.rotation * Vector3.right * 4f;
                position.y += 1f;
                if (Physics.Raycast(position, -roadAchor.transform.right, out hit, rayLength))
                {
                    if (hit.collider.gameObject.tag == "Vehicle")
                    {
                        if (state)
                        {
                            hit.collider.gameObject.GetComponent<VehicleMovement>().VehicleMove();
                        }
                        else
                        {
                            hit.collider.gameObject.GetComponent<VehicleMovement>().VehicleStop();
                        }
                    }
                }
                DebugRay(position, -roadAchor.transform.right);
            }
        }
    }

    void RayCastVehicleAction()
    {

    }

    public void DebugRay(Vector3 position, Vector3 dir)
    {
        if (state)
            Debug.DrawRay(position, dir * rayLength, Color.green);
        else
            Debug.DrawRay(position, dir * rayLength, Color.red);
    }
}
