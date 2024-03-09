using Barmetler;
using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{

    public List<Bezier.OrientedPoint> movePoints;
    private int currentPoint = 0;
    public float speed = 0f;
    private RoadSystemNavigator navigator;
    public GameObject goalObject;
    public VehicleGeneration vehicleGeneration;
    private float arriveThreshold = 20f;
    public float acceleration = 30f;
    public float maximumAcceleration = 30f;
    public float maximumSpeed = 30f;
    public float currentSpeed = 0f;
    public float desiredSpeed = 30f;
    public float rotationSpeed = 10f;
    public Vector3 movingDirection;
    private Rigidbody rb;
    public bool move = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navigator = GetComponent<RoadSystemNavigator>();
        navigator.currentRoadSystem = FindAnyObjectByType<RoadSystem>();
        vehicleGeneration = FindAnyObjectByType<VehicleGeneration>();
        SetRandomGoal();
    }

    // Update is called once per frame
    void Update()
    {
        SetMovePoints();
        CheckVehicleInfront();
        CheckVehicleInfront();
        if (move)
        {
            SetVelocity();
        }
        //UpdateMovement();

    }

    public void SetMovePoints()
    {
        movePoints = navigator.CurrentPoints;
    }

    public void SetRandomGoal()
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
            Vector3 direcion = (target - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direcion);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            //transform.LookAt(target);
            

            if (Vector3.Distance(transform.position, goalObject.transform.position) < arriveThreshold)
            {
                vehicleGeneration.ReduceVehicleCount(1);
                Destroy(this.gameObject);
            }
        }
    }

    public void SetVelocity()
    {
        if (currentPoint < movePoints.Count)
        {
            Vector3 forwardDirection;
            if (currentPoint == movePoints.Count - 1)
                forwardDirection = (goalObject.gameObject.transform.position - movePoints[currentPoint].position).normalized;
            else
                forwardDirection = (movePoints[currentPoint + 1].position - movePoints[currentPoint].position).normalized;

            Vector3 target = movePoints[currentPoint].position + Vector3.Cross(Vector3.up, forwardDirection).normalized * -1.5f;
            float speedDifference = desiredSpeed - currentSpeed;
            acceleration = Mathf.Clamp(speedDifference / desiredSpeed, -1f, 1f) * maximumAcceleration;
            currentSpeed += acceleration * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);
            movingDirection = (target - transform.position).normalized;
            if (movingDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(movingDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }

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
                float dotProduct = Vector3.Dot(this.movingDirection.normalized, hit.collider.gameObject.GetComponent<VehicleMovement>().movingDirection.normalized);
                if (dotProduct > 0)
                {
                    desiredSpeed = Mathf.Lerp(maximumSpeed, 0f, hit.distance / maxDistance);
                    Debug.DrawRay(raycastPosition, transform.forward * maxDistance, Color.green);
                }
                else
                {
                    Debug.DrawRay(raycastPosition, transform.forward * maxDistance, Color.blue);
                }
            }
            else
            {
                Debug.DrawRay(raycastPosition, transform.forward * maxDistance, Color.yellow);
            }
        }
        else
        {
            desiredSpeed = maximumSpeed;
            Debug.DrawRay(raycastPosition, transform.forward * maxDistance, Color.red);
        }
    }

    public void VehicleStop()
    {
        move = false;
    }

    public void VehicleMove()
    {
        move = true;
    }
}
