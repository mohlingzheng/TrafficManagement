using Barmetler;
using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class VehicleMovement : MonoBehaviour
{
    [Header("Debugging")]
    public List<Bezier.OrientedPoint> movePoints;
    private int currentPoint = 0;
    private RoadSystemNavigator navigator;
    public GameObject goalObject;
    public VehicleGeneration vehicleGeneration;
    public bool movePointReady = false;
    //public GameObject SpecificGoal;

    [Header("Pathfinding")]
    public bool isSelected = false;

    [Header("Vehicle Attribute")]
    private float arriveThreshold = 5f;
    public float currentSpeed = 0f;
    public float desiredSpeed;
    public float FixedDesiredSpeed;
    public float rotationSpeed = 10f;
    public Vector3 movingDirection;
    public VehicleType vehicleType;

    [Header("Car Following Model")]
    public float currentAcceleration = 5f;
    public float nextObjectSpeed;
    public float relativeSpeed;

    [Header("Lane Changing Model")]
    public string currentRoadType;
    public float distanceFromCentre = -4f;
    public bool highSpeed;
    public bool stopDueToTraffic;
    public bool laneJustChanged = false;
    public float laneChangingClock = 0f;
    public string goingToTurn = "";
    public Vector3 pointToTurn;
    public bool SeeingTransitionWall = false;

    [Header("Analysis")]
    public double timeWaited = 0f;
    public TimeTrackingManager timeTrackingManager;

    [Header("Weighted Heuristic")]
    GameObject previousRoad = null;

    [Header("Others")]
    public float rayDistance = 10f;
    const float MaximumRayDistance = 10f;
    public GameObject rayHitObject = null;

    public float distanceDetect;
    public float distanceBetween;
    public float dotProduct;

    private TrafficLightState trafficLightState;
    public bool special = false;

    public float CountDistance = 0f;
    public bool Count = false;

    void Start()
    {
        navigator = GetComponent<RoadSystemNavigator>();
        navigator.currentRoadSystem = GameObject.Find("RoadSystem").GetComponent<RoadSystem>();
        vehicleGeneration = FindAnyObjectByType<VehicleGeneration>();
        timeTrackingManager = FindAnyObjectByType<TimeTrackingManager>();
        SetDesiredSpeed();
        SetRandomGoal();
        StartCoroutine(SetMovePointLoop());
    }

    void FixedUpdate()
    {
        RemoveMovePointsWith90Degree();
        DynamicSpeedLogic();
        LaneChangingLogic();
        MoveWithCurrentSpeed();
        TimeTracking();
        RemoveOnUnexpectedPosition();
        UpdateMovePointReady();
        //ReachDestination();
    }

    private void SetDesiredSpeed()
    {
        if (vehicleType == VehicleType.Light)
            desiredSpeed = Random.Range(15, 25);
        else if (vehicleType == VehicleType.Heavy)
            desiredSpeed = Random.Range(10, 15);
        else
            Debug.Log("No Vehicle Type Specified");
        if (transform.name == "Specific Vehicle")
        {
            desiredSpeed = 50f;
            highSpeed = true;
        }
        FixedDesiredSpeed = desiredSpeed;
    }

    private void UpdateMovePointReady()
    {
        movePointReady = navigator.enabled == false;
    }

    //private void OnDrawGizmos()
    //{
    //    if (movePoints.Count <= 3 && !movePointReady)
    //        return;
    //    for (int i = 1; i < movePoints.Count - 1; i++)
    //    {
    //        float dotResult = GetDotResult(movePoints[i - 1].position, movePoints[i].position, movePoints[i + 1].position);
    //        Vector3 forwardDirection = (movePoints[i + 1].position - movePoints[i].position).normalized;
    //        Vector3 testdirection = Vector3.Cross(Vector3.up, forwardDirection).normalized;
    //        Vector3 target = movePoints[i].position + testdirection * distanceFromCentre;
    //        if (ValueNearToZero(dotResult))
    //        {
    //            Gizmos.DrawSphere(target, 2f);
    //        }
    //        else
    //        {
    //            Gizmos.DrawSphere(target, 0.5f);
    //        }
    //    }
    //}


    private void TimeTracking()
    {
        if (ValueNearToZero(currentSpeed))
            timeWaited += Time.deltaTime;
    }

    public void LaneChangingLogic()
    {
        CheckRoadTypeOn();
        HandleRoadTypeCondition();
        LaneChangingClockCounter();
    }

    private void HandleRoadTypeCondition()
    {
        if (currentRoadType == OnLanes.Small || currentRoadType == OnLanes.Transition_2_4)
        {
            UpdateHighSpeed(true);
        }
        else if (currentRoadType == OnLanes.Large && GoingToTurnLeft())
        {
            if (highSpeed)
            {
                // find chance to go to high speed lane
                // if no chance, stop and wait
                if (CheckLaneFeasible("Left"))
                {
                    UpdateHighSpeed(false);
                }
            }
        }
        else if (currentRoadType == OnLanes.Large && GoingToTurnRight())
        {
            if (!highSpeed)
            {
                // find chance to go to high speed lane
                // if no chance, stop and wait
                if (CheckLaneFeasible("Right"))
                {
                    UpdateHighSpeed(true);
                }
            }
        }
        else if (currentRoadType == OnLanes.Large && !stopDueToTraffic)
        {
            ChangeLane();
        }
        else if (currentRoadType == OnLanes.Transition_4_2)
        {
            if (!highSpeed)
            {
                // find chance to go to high speed lane
                // if no chance, stop and wait
                if (CheckLaneFeasible("Right"))
                {
                    UpdateHighSpeed(true);
                }
            }
        }
        distanceFromCentre = highSpeed ? OnLanes.GetValue(OnLanes.High) : OnLanes.GetValue(OnLanes.Low);
    }

    private bool GoingToTurnLeft()
    {
        int gap = 5;
        for (int i = 0; i < movePoints.Count; i++)
        {
            if (i + gap > movePoints.Count - 1)
                return false;
            else if (goingToTurn != "" && CheckPositionInMovePoints())
                return true;

            Vector3 delta = movePoints[i + gap].position - movePoints[i].position;
            Vector3 crossProduct = Vector3.Cross(delta, movePoints[i + 1].position - movePoints[i].position);
            dotProduct = Vector3.Dot(crossProduct, transform.up);
            if (dotProduct > 3.0f)
            {
                float distance = Vector3.Distance(movePoints[i].position, transform.position);
                if (distance < 60f)
                {
                    pointToTurn = movePoints[i].position;
                    goingToTurn = "Left";
                    return true;
                }
                else
                {
                    pointToTurn = new Vector3();
                    goingToTurn = "";
                    return false;
                }
            }
        }
        return false;
    }

    private bool GoingToTurnRight()
    {
        int gap = 5;
        for (int i = 0; i < movePoints.Count; i++)
        {
            if (i + gap > movePoints.Count - 1)
                return false;
            else if (goingToTurn != "" && CheckPositionInMovePoints())
                return true;

            Vector3 delta = movePoints[i + gap].position - movePoints[i].position;
            Vector3 crossProduct = Vector3.Cross(delta, movePoints[i + 1].position - movePoints[i].position);
            dotProduct = Vector3.Dot(crossProduct, transform.up);
            if (dotProduct < -3.0f)
            {
                float distance = Vector3.Distance(movePoints[i].position, transform.position);
                if (distance < 60f)
                {
                    pointToTurn = movePoints[i].position;
                    goingToTurn = "Right";
                    return true;
                }
                else 
                {
                    pointToTurn = new Vector3();
                    goingToTurn = "";
                    return false; 
                }
            }
        }
        return false;
    }

    private bool CheckPositionInMovePoints()
    {
        for (int i = 0; i < movePoints.Count; i++)
        {
            if (movePoints[i].position == pointToTurn)
                return true;
        }
        return false;
    }

    private void LaneChangingClockCounter()
    {
        if (laneJustChanged)
        {
            laneChangingClock = laneChangingClock + Time.deltaTime;
            if (laneChangingClock > 3)
            {
                laneChangingClock = 0;
                laneJustChanged = false;
            }
        }
    }

    private void CheckRoadTypeOn()
    {
        RaycastHit hit;
        Vector3 position = transform.position;
        position.y += 0.5f;
        if (Physics.Raycast(position, Vector3.down, out hit, 2f))
        {
            if (Tag.CompareTags(hit.collider.transform, Tag.Road_Small))
            {
                currentRoadType = OnLanes.Small;
                UpdateRoadTrafficDensityCount(hit.collider.transform.gameObject);

            }
            else if (Tag.CompareTags(hit.collider.transform, Tag.Road_Large))
            {
                currentRoadType = OnLanes.Large;
                UpdateRoadTrafficDensityCount(hit.collider.transform.gameObject);
            }
            else if (Tag.CompareTags(hit.collider.transform, Tag.Intersection_3_Small, Tag.Intersection_4_Small))
                currentRoadType = OnLanes.Small;
            else if (Tag.CompareTags(hit.collider.transform, Tag.Intersection_3_Large, Tag.Intersection_4_Large))
                currentRoadType = OnLanes.Large;
            else if (Tag.CompareTags(hit.collider.transform, Tag.Transition))
            {
                if (currentRoadType == OnLanes.Small)
                {
                    currentRoadType = OnLanes.Transition_2_4;
                }
                else if (currentRoadType == OnLanes.Large)
                {
                    currentRoadType = OnLanes.Transition_4_2;
                }
                else
                {
                    // remain the value of currentRoadType
                }
            }
        }
    }

    private void ChangeLane()
    {
        if (!highSpeed && !stopDueToTraffic)
        {
            RaycastHit? hit = CheckObjectInfront();
            if (hit.HasValue)
            {
                if (Tag.CompareTags(hit.Value.collider.transform, Tag.Vehicle) && !hit.Value.collider.gameObject.GetComponent<VehicleMovement>().stopDueToTraffic)
                {
                    float nextCarSpeed = hit.Value.collider.gameObject.GetComponent<VehicleMovement>().currentSpeed;
                    if (desiredSpeed - nextCarSpeed > 3f && CheckLaneFeasible("Right"))
                    {
                        UpdateHighSpeed(true);
                    }
                }
            }
        }
        else if (highSpeed && !SeeingTransitionWall)
        {
            RaycastHit? hit = CheckObjectInBehind();
            if (hit.HasValue)
            {
                if (Tag.CompareTags(hit.Value.collider.transform, Tag.Vehicle))
                {
                    float lastCarSpeed = hit.Value.collider.gameObject.GetComponent<VehicleMovement>().desiredSpeed;
                    if (lastCarSpeed - desiredSpeed > 1f && CheckLaneFeasible("Left"))
                    {
                        UpdateHighSpeed(false);
                    }
                }
            }
        }
        
    }

    public void UpdateHighSpeed(bool result)
    {
        if (!laneJustChanged)
        {
            highSpeed = result;
            laneJustChanged = true;
        }
    }

    public bool CheckLaneFeasible(string laneToTurnTo)
    {
        if (laneToTurnTo == "Right")
        {
            // Get the car's position and rotation
            Vector3 carPosition = transform.position;
            Quaternion carRotation = transform.rotation;

            // Calculate the offset vector representing the right direction of the car
            Vector3 rightOffset = carRotation * Vector3.right * 4f;

            // Calculate the position on the right side of the car
            Vector3 rightPosition = carPosition + rightOffset;

            // Use rightPosition for whatever purpose you need
            Debug.DrawLine(carPosition, rightPosition, UnityEngine.Color.red);

            float raycastDistance = 12f;

            float forwardDistance = 10f;
            float backwardDistance = 10f;
            RaycastHit hitForward;
            Debug.DrawRay(rightPosition, transform.forward * raycastDistance, UnityEngine.Color.green);

            if (Physics.Raycast(rightPosition, transform.forward, out hitForward, raycastDistance))
            {
                if (hitForward.collider.CompareTag(Tag.Vehicle))
                    forwardDistance = hitForward.distance;
            }

            // Cast a ray backward from the right position
            Debug.DrawRay(rightPosition, -transform.forward * raycastDistance, UnityEngine.Color.blue);

            RaycastHit hitBackward;
            if (Physics.Raycast(rightPosition, -transform.forward, out hitBackward, raycastDistance))
            {
                if (hitBackward.collider.CompareTag(Tag.Vehicle))
                    backwardDistance = hitBackward.distance;
            }

            if (forwardDistance + backwardDistance > 16f)
            {
                return true;
            }
        }   
        else if (laneToTurnTo == "Left")
        {
            // Get the car's position and rotation
            Vector3 carPosition = transform.position;
            Quaternion carRotation = transform.rotation;

            // Calculate the offset vector representing the right direction of the car
            Vector3 rightOffset = carRotation * -Vector3.right * 4f;

            // Calculate the position on the right side of the car
            Vector3 rightPosition = carPosition + rightOffset;

            // Use rightPosition for whatever purpose you need
            Debug.DrawLine(carPosition, rightPosition, UnityEngine.Color.red);

            float raycastDistance = 12f;

            float forwardDistance = 10f;
            float backwardDistance = 10f;
            RaycastHit hitForward;
            Debug.DrawRay(rightPosition, transform.forward * raycastDistance, UnityEngine.Color.green);

            if (Physics.Raycast(rightPosition, transform.forward, out hitForward, raycastDistance))
            {
                if (hitForward.collider.CompareTag("Vehicle"))
                    forwardDistance = hitForward.distance;
            }

            // Cast a ray backward from the right position
            Debug.DrawRay(rightPosition, -transform.forward * raycastDistance, UnityEngine.Color.blue);

            RaycastHit hitBackward;
            if (Physics.Raycast(rightPosition, -transform.forward, out hitBackward, raycastDistance))
            {
                {
                    if (hitBackward.collider.CompareTag("Vehicle"))
                        backwardDistance = hitBackward.distance;
                }
            }

            if (forwardDistance + backwardDistance > 10f)
            {
                return true;
            }
        }
        return false;
    }

    private void DynamicSpeedLogic()
    {
        RaycastHit? hit = CheckObjectInfront();
        HandleTypeRaycasthit(hit);
    }

    private void MoveWithCurrentSpeed()
    {
        if (currentPoint < movePoints.Count)
        {
            Vector3 forwardDirection;
            if (currentPoint == movePoints.Count - 1)
            {
                Vector3 position = goalObject.gameObject.transform.position;
                position.y = 0;
                forwardDirection = (position - movePoints[currentPoint].position).normalized;
            }
            else
                forwardDirection = (movePoints[currentPoint + 1].position - movePoints[currentPoint].position).normalized;
            Vector3 testdirection = Vector3.Cross(Vector3.up, forwardDirection).normalized;
            Debug.DrawRay(testdirection, Vector3.up * 100f, UnityEngine.Color.red);
            Vector3 target = movePoints[currentPoint].position + testdirection * distanceFromCentre;
            movingDirection = (target - transform.position).normalized;

            float moveDistance = currentSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, target, moveDistance);

            if (Count)
            {
                CountDistance += moveDistance;
                if (CountDistance >= 40f)
                {
                    CountDistance = 0;
                    Count = false;
                }

            }

            float distance = Vector3.Distance(transform.position, movePoints[currentPoint].position);
            if (distance < Mathf.Abs(distanceFromCentre) + 0.5f)
                movePoints.RemoveAt(0);

            if (movingDirection != Vector3.zero && !ValueNearToZero(currentSpeed))
            {
                Quaternion rotation = Quaternion.LookRotation(movingDirection);
                rotation.x = 0f;
                rotation.z = 0f;
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }

            //if (Vector3.Distance(transform.position, goalObject.transform.position) < arriveThreshold)
            //{
            //    if (gameObject.name != "Specific Vehicle 2" && gameObject.name != "Specific Vehicle")
            //    {
            //        vehicleGeneration.ReduceVehicleCount(1);
            //    }
            //    Destroy(gameObject);
            //}
        }
    }

    private void CarFollowingStimulusResponseModel(float distanceBetween, float nextObjectSpeed)
    {
        distanceDetect = distanceBetween;
        float c1 = 1.0f;
        float c2 = 1.0f;
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
        raycastPosition.y += 0.5f;
        //Physics.Raycast(raycastPosition, transform.forward, out hit, maxDistance);
        Debug.DrawRay(raycastPosition, transform.forward * rayDistance, UnityEngine.Color.yellow);
        if (Physics.Raycast(raycastPosition, transform.forward, out hit, rayDistance))
        {
            //if (hit.collider.name == goalObject.name)
            //{
            //    Debug.Log("obs");
            //}
            return hit;
        }
        else
        {
            return null;
        }
    }

    public RaycastHit? CheckObjectInBehind()
    {
        RaycastHit hit;
        Vector3 raycastPosition = transform.position;
        raycastPosition.y += 0.5f;
        //Physics.Raycast(raycastPosition, transform.forward, out hit, maxDistance);
        Debug.DrawRay(raycastPosition, -transform.forward * rayDistance, UnityEngine.Color.yellow);
        if (Physics.Raycast(raycastPosition, -transform.forward, out hit, rayDistance))
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
        if (hit.HasValue && Tag.CompareTags(hit.Value.collider.transform, Tag.Bump))
        {
            currentSpeed = FixedDesiredSpeed / 2;
        }

        if (hit.HasValue && Tag.CompareTags(hit.Value.collider.transform, Tag.Transition_Wall) && !highSpeed)
        {
            SeeingTransitionWall = true;
            if (CheckLaneFeasible("Right"))
            {
                UpdateHighSpeed(true);
            }
            UpdateStopAttribute(hit.Value);
        }
        else
        {
            SeeingTransitionWall = false;
            if (hit.HasValue && Tag.CompareTags(hit.Value.collider.transform, Tag.Vehicle) && IsVehicleSameLane(hit.Value))
            {
                UpdateStopAttribute(hit.Value);
            }
            else if (
                hit.HasValue
                && DoHitHaveLayer(hit.Value, "Invisible")
                && Tag.CompareTags(hit.Value.collider.transform.parent.parent, Tag.Intersection_4_Small, Tag.Intersection_4_Large)
                )
            {
                if (hit.Value.collider.gameObject.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Red))
                {
                    UpdateStopAttribute(hit.Value);
                }
                else if (hit.Value.collider.gameObject.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Green))
                {
                    if (WantToRight() && goingToTurn == "Right")
                    {
                        if (CheckIfThereIsCar(GetWallAcross(hit.Value.collider.gameObject)))
                        {
                            UpdateStopAttribute(hit.Value);
                        }
                        else
                        {
                            UpdateNormalAttribute();
                        }
                    }
                    else
                    {
                        UpdateNormalAttribute();
                    }
                }
                else
                {
                    Debug.Log("Intersection4 Wrong Condition");
                }
            }
            else if (
                hit.HasValue
                && DoHitHaveLayer(hit.Value, "Invisible")
                && Tag.CompareTags(hit.Value.collider.transform.parent.parent, Tag.Intersection_3_Small, Tag.Intersection_3_Large)
                )
            {
                if (WantToRight())
                {
                    if (hit.Value.collider.gameObject.transform.parent.name == AnchorType.anchor_east)
                    {
                        // Check Anchor West
                        if (CheckIfThereIsCar(GetSpecificWall(hit.Value.collider.gameObject, AnchorType.anchor_west)))
                        {
                            UpdateStopAttribute(hit.Value);
                        }
                        else
                        {
                            UpdateNormalAttribute();
                        }
                    }
                    else if (hit.Value.collider.gameObject.transform.parent.name == AnchorType.anchor_north)
                    {
                        if (CheckIfThereIsCar(GetSpecificWall(hit.Value.collider.gameObject, AnchorType.anchor_west)) || CheckIfThereIsCar(GetSpecificWall(hit.Value.collider.gameObject, AnchorType.anchor_east)))
                        {
                            UpdateStopAttribute(hit.Value);
                        }
                        else
                        {
                            UpdateNormalAttribute();
                        }
                    }
                    else
                    {
                        UpdateNormalAttribute();
                    }
                }
                else if (WantToLeft())
                {
                    if (hit.Value.collider.gameObject.transform.parent.name == AnchorType.anchor_west)
                    {
                        UpdateNormalAttribute();
                    }
                    else if (hit.Value.collider.gameObject.transform.parent.name == AnchorType.anchor_north)
                    {
                        // Check Anchor West
                        if (CheckIfThereIsCar(GetSpecificWall(hit.Value.collider.gameObject, AnchorType.anchor_west)))
                        {
                            UpdateStopAttribute(hit.Value);
                        }
                        else
                        {
                            UpdateNormalAttribute();
                        }
                    }
                }
                else
                {
                    UpdateNormalAttribute();
                }
            }
            else
            {
                UpdateNormalAttribute();
            }
        }
        

        CarFollowingStimulusResponseModel(distanceBetween, nextObjectSpeed);

    }

    private bool WantToRight()
    {
        if (Count)
            return true;
        int index = 3;
        index = Mathf.Clamp(index, 0, movePoints.Count);
        if (movePoints.Count < index + 1)
            return false;
        Vector3 delta = movePoints[index].position - transform.position;
        Vector3 crossProduct = Vector3.Cross(delta, transform.forward);
        dotProduct = Vector3.Dot(crossProduct, transform.up);
        if (dotProduct < -3.0f)
        {
            if (!Count)
                Count = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool WantToLeft()
    {
        int index = 3;
        index = Mathf.Clamp(index, 0, movePoints.Count);
        if (movePoints.Count < index + 1)
            return false;
        Vector3 delta = movePoints[index].position - transform.position;
        Vector3 crossProduct = Vector3.Cross(delta, transform.forward);
        float dotProduct = Vector3.Dot(crossProduct, transform.up);
        if (dotProduct > 3.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private GameObject GetWallAcross(GameObject wall)
    {
        string name = wall.transform.parent.name;
        if (name == AnchorType.anchor_north)
        {
            return wall.transform.parent.parent.Find(AnchorType.anchor_south).Find("TrafficLightBlock").gameObject;
        }
        else if (name == AnchorType.anchor_south)
        {
            return wall.transform.parent.parent.Find(AnchorType.anchor_north).Find("TrafficLightBlock").gameObject;
        }
        else if (name == AnchorType.anchor_east)
        {
            return wall.transform.parent.parent.Find(AnchorType.anchor_west).Find("TrafficLightBlock").gameObject;
        }
        else if (name == AnchorType.anchor_west)
        {
            return wall.transform.parent.parent.Find(AnchorType.anchor_east).Find("TrafficLightBlock").gameObject;
        }
        else
        {
            return null;
        }
    }

    private GameObject GetSpecificWall(GameObject wall, string anchor)
    {
        return wall.transform.parent.parent.Find(anchor).Find("TrafficLightBlock").gameObject;
    }

    private bool CheckIfThereIsCar(GameObject wall, bool intersection4 = false)
    {
        if (ProbabilityNotFollow())
            return false;
        if (Tag.CompareTags(wall.transform.parent.parent.transform, Tag.Intersection_3_Large, Tag.Intersection_4_Large))
        {
            RaycastHit hitLeft, hitRight;
            Vector3 positionLeft = wall.transform.position - wall.transform.forward * 10f;
            positionLeft.y -= 0.5f;
            positionLeft -= wall.transform.right * 2f;
            Debug.DrawRay(positionLeft, wall.transform.forward * 30f, UnityEngine.Color.red);

            if (Physics.Raycast(positionLeft, wall.transform.forward, out hitLeft, 30f))
            {
                if (hitLeft.collider.transform.CompareTag("Vehicle"))
                {
                    if (intersection4)
                    {
                        if (hitLeft.collider.transform.GetComponent<VehicleMovement>().goingToTurn != "Right")
                            return true;
                    }
                    else
                        return true;
                }

            }

            Vector3 positionRight = wall.transform.position - wall.transform.forward * 10f;
            positionRight.y -= 0.5f;
            positionRight += wall.transform.right * 2f;
            Debug.DrawRay(positionRight, wall.transform.forward * 30f, UnityEngine.Color.blue);

            if (Physics.Raycast(positionRight, wall.transform.forward, out hitRight, 30f))
            {
                if (hitRight.collider.transform.CompareTag("Vehicle"))
                {
                    if (intersection4)
                    {
                        if (hitRight.collider.transform.GetComponent<VehicleMovement>().goingToTurn != "Right")
                            return true;
                    }
                    else
                        return true;
                }
            }

            return false;
        }
        else if (Tag.CompareTags(wall.transform.parent.parent.transform, Tag.Intersection_3_Small, Tag.Intersection_4_Small))
        {
            RaycastHit hit;
            Vector3 position = wall.transform.position - wall.transform.forward * 10f;
            position.y -= 0.5f;
            Debug.DrawRay(position, wall.transform.forward * 30f, UnityEngine.Color.red);
            if (Physics.Raycast(position, wall.transform.forward, out hit, 30f))
            {
                if (hit.collider.transform.CompareTag("Vehicle"))
                    return true;
            }
            return false;
        }
        else
            return false;
    }

    private void UpdateNormalAttribute()
    {
        distanceBetween = 10f;
        nextObjectSpeed = desiredSpeed;
    }

    private void UpdateStopAttribute(RaycastHit hit)
    {
        if (!Tag.CompareTags(hit.collider.transform, Tag.Vehicle))
            stopDueToTraffic = true;
        else
            stopDueToTraffic = hit.collider.gameObject.GetComponent<VehicleMovement>().stopDueToTraffic;

        distanceBetween = hit.distance;
        nextObjectSpeed = 0f;
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

    public bool DoHitHaveLayer(RaycastHit hit, string layer)
    {
        return hit.collider.gameObject.layer == LayerMask.NameToLayer(layer);
    }

    private bool ValueNearToZero(float value, float threshold = 0.001f)
    {
        if (value < threshold && value > -threshold)
            return true;
        return false;
    }

    public IEnumerator SetMovePointLoop()
    {
        bool movePointReady = false;
        navigator.enabled = true;
        while (movePointReady == false)
        {
            SetMovePoints();
            yield return new WaitForSeconds(0.1f);
            //SetMovePoints();
            //yield return new WaitForSeconds(1.0f);
            //SetMovePoints();
            //yield return new WaitForSeconds(1.0f);
            //SetMovePoints();
            //yield return new WaitForSeconds(0.5f);
            //SetMovePoints();
            //yield return new WaitForSeconds(0.5f);
            //SetMovePoints();
            //yield return new WaitForSeconds(0.5f);
            //SetMovePoints();
            //yield return new WaitForSeconds(0.5f);
            //SetMovePoints();
            //yield return new WaitForSeconds(0.5f);
            if (movePoints.Count > 3)
            {
                movePointReady = true;
            }
        }
        navigator.enabled = false;
    }

    public void SetMovePoints()
    {
        navigator.CalculateWayPointsSync();
        movePoints = new List<Bezier.OrientedPoint>(navigator.CurrentPoints);
        Vector3 position = goalObject.gameObject.transform.position;
        position.y = 0;
        Bezier.OrientedPoint goal = new Bezier.OrientedPoint(position, Vector3.forward, Vector3.up);
        movePoints.Add(goal);
        //FilterMovePoints();
    }

    public void RemoveMovePointsWith90Degree()
    {
        if (!movePointReady)
            return;
        else if (movePoints.Count <= 3)
            return;
        for (int i = 1; i < movePoints.Count - 3; i++)
        {
            float dotResult = GetDotResult(movePoints[i - 1].position, movePoints[i].position, movePoints[i + 1].position);
            if (ValueNearToZero(dotResult))
            {
                RemoveIfExist(i + 5);
                RemoveIfExist(i + 4);
                RemoveIfExist(i + 3);
                RemoveIfExist(i + 2);
                RemoveIfExist(i + 1);

                RemoveIfExist(i);

                RemoveIfExist(i - 1);
                RemoveIfExist(i - 2);
                RemoveIfExist(i - 3);
                RemoveIfExist(i - 4);
                RemoveIfExist(i - 5);
            }
        }
    }

    private void RemoveIfExist(int i)
    {
        if (i <= 0 || i >= movePoints.Count)
            return;
        else
            movePoints.RemoveAt(i);
    }

    public float GetDotResult (Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 v1 = (p2 - p1).normalized;
        Vector3 v2 = (p3 - p2).normalized;
        return Vector3.Dot(v1, v2);
    }

    public void SetRandomGoal()
    {
        GameObject[] goalObjects = GameObject.FindGameObjectsWithTag("Goal");
        if (gameObject.name == "Specific Vehicle" || gameObject.name == "Specific Vehicle 2")
        {
            goalObject = GameObject.Find("NBL Money Transfer Sdn Bhd");
        }
        else if (goalObjects.Length > 0)
        {
            int randomIndex = Random.Range(0, goalObjects.Length);
            goalObject = goalObjects[randomIndex];
        }
        goalObject = GameObject.Find("Pejabat Pos KOMTAR");
        navigator.Goal = goalObject.transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == goalObject)
        {
            UpdateTimeAndDestroy(true);
        }
    }

    //private void OnTriggerEnter(Collider collision)
    //{
    //    Debug.Log("bs");
    //    if (collision.gameObject == goalObject)
    //    {
    //        Debug.Log("b");
    //        UpdateTimeAndDestroy(true);
    //    }
    //}

    private void RemoveOnUnexpectedPosition()
    {
        if (transform.position.y < -10f)
        {
            UpdateTimeAndDestroy(false);
        }
    }

    private void UpdateTimeAndDestroy(bool normal = true)
    {
        if (normal)
        {
            if (previousRoad != null)
            {
                if (previousRoad.TryGetComponent<RoadTrafficDensity>(out RoadTrafficDensity roadTrafficDensity))
                {
                    roadTrafficDensity.DecreaseCount();
                }
            }
            timeTrackingManager.currentTotalTime += timeWaited;
        }
        vehicleGeneration.ReduceVehicleCount(1);
        Destroy(gameObject);
    }

    private void UpdateRoadTrafficDensityCount(GameObject currentRoad)
    {
        if (previousRoad == null)
        {
            previousRoad = currentRoad;
            previousRoad.GetComponent<RoadTrafficDensity>().IncreaseCount();
        }
        else if (currentRoad != previousRoad)
        {
            previousRoad.GetComponent<RoadTrafficDensity>().DecreaseCount();
            currentRoad.GetComponent<RoadTrafficDensity>().IncreaseCount();
            previousRoad = currentRoad;
        }
    }

    private void ReachDestination()
    {
        if (ProbabilityNotFollow())
        {
            if (ValueNearToZero(Vector3.Distance(transform.position, goalObject.transform.position), 1f))
            {
                UpdateTimeAndDestroy(true);
            }
        }
    }

    private bool ProbabilityNotFollow(int threshold = 5)
    {
        int num = Random.Range(0, 100);
        if (num <= threshold)
            return true;
        else
            return false;
    }
}
