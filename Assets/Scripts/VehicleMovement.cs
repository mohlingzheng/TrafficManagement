using Barmetler;
using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{

    public List<Bezier.OrientedPoint> movePoints;
    private int currentPoint = 0;
    public float speed = 50f;
    private RoadSystemNavigator navigator;
    public GameObject goalObject;
    public VehicleGeneration vehicleGeneration;
    private float arriveThreshold = 20f;

    // Start is called before the first frame update
    void Start()
    {
        navigator = GetComponent<RoadSystemNavigator>();
        navigator.currentRoadSystem = FindAnyObjectByType<RoadSystem>();
        vehicleGeneration = FindAnyObjectByType<VehicleGeneration>();
        SetGoal();
    }

    // Update is called once per frame
    void Update()
    {
        SetMovePoints();
        if (currentPoint < movePoints.Count)
        {
            Vector3 direction = (movePoints[currentPoint].position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.Translate(direction * speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, movePoints[currentPoint].position) < 0.1f)
            {
                currentPoint++;
            }

            if (Vector3.Distance(transform.position, goalObject.transform.position) < arriveThreshold)
            {
                vehicleGeneration.ReduceVehicleCount(1);
                Destroy(this.gameObject);
            }
        }
    }

    public void SetMovePoints()
    {
        movePoints = navigator.CurrentPoints;
    }

    public void SetGoal()
    {
        GameObject[] goalObjects = GameObject.FindGameObjectsWithTag("Goal");
        if (goalObjects.Length > 0)
        {
            int randomIndex = Random.Range(0, goalObjects.Length);
            goalObject = goalObjects[randomIndex];
        }
        navigator.Goal = goalObject.transform.position;
    }
}
