using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class GenerateBuilding : MonoBehaviour
{
    public GameObject[] BuildingPrefabs;
    public GameObject GoalSystem;
    public GameObject[] Roads;

    private void Start()
    {
        
    }
    public void GenerateRandomBuilding()
    {
        Roads = GameObject.FindGameObjectsWithTag("Road");
        int random = Random.Range(0, BuildingPrefabs.Length);
        GameObject newBuilding = Instantiate(BuildingPrefabs[random], GoalSystem.transform);
        newBuilding.transform.SetLocalPositionAndRotation(GetPosition(), Quaternion.LookRotation(GetDirection()));
    }

    Vector3 GetPosition()
    {
        Vector3 position = transform.position;
        position.y -= 2f;
        return position;
    }

    Vector3 GetDirection()
    {
        Vector3 position = transform.position;
        position.y -= 2;

        Barmetler.Bezier.OrientedPoint closestPoint = null;
        float closestDistance = float.PositiveInfinity;

        foreach (GameObject Road in Roads)
        {
            Road roadScript = Road.GetComponent<Road>();
            Barmetler.Bezier.OrientedPoint[] newPoints = roadScript.GetEvenlySpacedPoints(1, 1).Select(e => e.ToWorldSpace(roadScript.transform)).ToArray();
            foreach (var point in newPoints)
            {
                float distance = Vector3.Distance(point.position, position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = point;
                }
            }
        }
        
        Vector3 direction = closestPoint.position - position;
        return direction;
    }

}
