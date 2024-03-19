using Barmetler;
using Barmetler.RoadSystem;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [Header("Debugging")]
    public List<Bezier.OrientedPoint> movePoints;
    private int currentPoint = 0;
    private RoadSystemNavigator navigator;
    public GameObject goalObject;
    public VehicleGeneration vehicleGeneration;
    public bool finalGoal = false;
    public bool movePointReady = false;

    [Header("Vehicle Attribute")]
    private float arriveThreshold = 5f;
    public float currentSpeed = 0f;
    public float desiredSpeed;
    public float rotationSpeed = 10f;
    public Vector3 movingDirection;

    [Header("Car Following Model")]
    public float currentAcceleration = 5f;
    public float nextObjectSpeed;
    public float relativeSpeed;
    float maxDistance = 50f;
    public float distanceDetect;

    private TrafficLightState trafficLightState;

    // Start is called before the first frame update
    void Start()
    {
        navigator = GetComponent<RoadSystemNavigator>();
        navigator.currentRoadSystem = GameObject.Find("RoadSystem").GetComponent<RoadSystem>();
        vehicleGeneration = FindAnyObjectByType<VehicleGeneration>();
        desiredSpeed = Random.Range(25, 35);
        SetRandomGoal();
        SetMovePoints();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!movePointReady)
            SetMovePoints();
        DynamicSpeed();
    }

    private void DynamicSpeed()
    {
        RaycastHit? hit = CheckObjectInfront();
        HandleTypeRaycasthit(hit);
        MoveWithCurrentSpeed();
    }

    private void MoveWithCurrentSpeed()
    {
        if (currentPoint < movePoints.Count)
        {
            Debug.Log(currentPoint + " " + movePoints.Count);
            Vector3 forwardDirection;
            if (currentPoint == movePoints.Count - 1)
            {
                forwardDirection = (goalObject.gameObject.transform.position - movePoints[currentPoint].position).normalized;
                //finalGoal = true;
            }
            else
                forwardDirection = (movePoints[currentPoint + 1].position - movePoints[currentPoint].position).normalized;
            Vector3 target = movePoints[currentPoint].position + Vector3.Cross(Vector3.up, forwardDirection).normalized * -1.5f;
            movingDirection = (target - transform.position).normalized;

            transform.position = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);

            float distance = Vector3.Distance(transform.position, movePoints[currentPoint].position);
            if (distance < 2f)
                movePoints.RemoveAt(0);

            if (movingDirection != Vector3.zero && !NearToZero())
            {
                Quaternion rotation = Quaternion.LookRotation(movingDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, goalObject.transform.position) < arriveThreshold)
            {
                if (gameObject.name != "Specific Vehicle 2" && gameObject.name != "Specific Vehicle")
                {
                    vehicleGeneration.ReduceVehicleCount(1);
                }
                Destroy(gameObject);
            }
        }
    }

    private void CarFollowingStimulusResponseModel(float distanceBetween, float nextObjectSpeed)
    {
        distanceDetect = distanceBetween;
        float c1 = 1f;
        float c2 = 1f;
        float gamma = 1f;
        float safeDistance = 0f;

        distanceBetween = distanceBetween - safeDistance;
        relativeSpeed = nextObjectSpeed - currentSpeed;


        currentAcceleration = gamma * Mathf.Pow(desiredSpeed, c1) / Mathf.Pow(distanceBetween, c2) * relativeSpeed;

        //currentAcceleration = desiredSpeed / distanceBetween * relativeSpeed;

        currentSpeed += currentAcceleration * Time.fixedDeltaTime;
    }

    public RaycastHit? CheckObjectInfront()
    {
        RaycastHit hit;
        Vector3 raycastPosition = transform.position;
        raycastPosition.y += 1f;
        Physics.Raycast(raycastPosition, transform.forward, out hit, maxDistance);
        Debug.DrawRay(raycastPosition, transform.forward * maxDistance, Color.yellow);
        if (Physics.Raycast(raycastPosition, transform.forward, out hit, maxDistance))
        {
            return hit;
        }
        else
        {
            return null;
        }
    }

    public void HandleTypeRaycasthit(RaycastHit? hit)
    {
        float distanceBetween;

        if (hit.HasValue && DoHitHaveTag(hit.Value, "Vehicle") && IsVehicleSameLane(hit.Value))
        {
            distanceBetween = hit.Value.distance;
            nextObjectSpeed = hit.Value.collider.gameObject.GetComponent<VehicleMovement>().currentSpeed;
        }
        else if (hit.HasValue && DoHitHaveLayer(hit.Value, LayerMask.NameToLayer("Invisible")) && hit.Value.collider.gameObject.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Red))
        {
            distanceBetween = hit.Value.distance;
            nextObjectSpeed = 0f;
        }
        else
        {
            distanceBetween = 10f;
            nextObjectSpeed = desiredSpeed;
        }
        CarFollowingStimulusResponseModel(distanceBetween, nextObjectSpeed);
    }

    private bool IsVehicleSameLane(RaycastHit hit)
    {
        float dotProduct = Vector3.Dot(this.movingDirection.normalized, hit.collider.gameObject.GetComponent<VehicleMovement>().movingDirection.normalized);
        if (dotProduct > 0)
            return true;
        else
            return false;
    }

    public bool DoHitHaveTag(RaycastHit hit, string tag)
    {
        return hit.collider.gameObject.CompareTag(tag);
    }

    public bool DoHitHaveLayer(RaycastHit hit, int layer)
    {
        return hit.collider.gameObject.layer == layer;
    }

    public bool NearToZero()
    {
        if (currentSpeed < 0.0001f && currentSpeed > -0.00001f)
            return true;
        return false;
    }

    public void SetMovePoints()
    {
        if (!finalGoal)
        {
            movePoints = new List<Bezier.OrientedPoint>(navigator.CurrentPoints);
            Bezier.OrientedPoint goal = new Bezier.OrientedPoint(goalObject.transform.position, Vector3.forward, Vector3.up);
            movePoints.Add(goal);
        }
        else
        {
            movePoints.Clear();
            Bezier.OrientedPoint goal = new Bezier.OrientedPoint(goalObject.transform.position, Vector3.forward, Vector3.up);
            movePoints.Add(goal);
        }

        if (movePoints.Count > 1)
        {
            movePointReady = true;
        }

    }

    public void SetRandomGoal()
    {
        GameObject[] goalObjects = GameObject.FindGameObjectsWithTag("Goal");
        if (gameObject.name == "Specific Vehicle 2" || gameObject.name == "Specific Vehicle")
        {
            goalObject = goalObjects[5];
        }
        else if (goalObjects.Length > 0)
        {
            int randomIndex = Random.Range(0, goalObjects.Length);
            goalObject = goalObjects[randomIndex];
        }
        navigator.Goal = goalObject.transform.position;
    }
}
