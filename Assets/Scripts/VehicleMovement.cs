using Barmetler;
using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{

    public List<Bezier.OrientedPoint> movePoints;
    private int currentPoint = 0;
    public float speed = 10f;
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
        UpdateMovement();
        CheckVehicleInfront();
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

    public void UpdateMovement()
    {
        if (currentPoint < movePoints.Count)
        {
            Vector3 forwardDirection = (movePoints[currentPoint + 1].position - movePoints[currentPoint].position).normalized;
            Vector3 target = movePoints[currentPoint].position + Vector3.Cross(Vector3.up, forwardDirection).normalized * -1.5f;

            //Vector3 direction = (movePoints[currentPoint].position - transform.position).normalized;
            //Quaternion targetRotation = Quaternion.LookRotation(direction);
            //transform.Translate(direction * speed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            Vector3 direcion = target - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direcion);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1f * Time.deltaTime);
            //transform.LookAt(target);
            

            if (Vector3.Distance(transform.position, goalObject.transform.position) < arriveThreshold)
            {
                vehicleGeneration.ReduceVehicleCount(1);
                Destroy(this.gameObject);
            }
        }
    }

    public void CheckVehicleInfront()
    {
        float maxDistance = 100f;
        RaycastHit hit;
        Vector3 raycastPosition = transform.position;
        raycastPosition.y += 1f;
        if (Physics.Raycast(raycastPosition, transform.forward, out hit, maxDistance))
        {
            if (hit.collider.gameObject.tag == "Vehicle")
            {
                Debug.DrawRay(raycastPosition, transform.forward * maxDistance, Color.green);
            }
            else
            {
                Debug.DrawRay(raycastPosition, transform.forward * maxDistance, Color.yellow);
            }
        }
        else
        {
            Debug.DrawRay(raycastPosition, transform.forward * maxDistance, Color.red);
        }
    }
}
