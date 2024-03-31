using Barmetler;
using Barmetler.RoadSystem;
using System.Collections;
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
    public bool movePointReady = false;

    [Header("Pathfinding")]
    public bool isSelected = false;

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

    // Start is called before the first frame update
    void Start()
    {
        navigator = GetComponent<RoadSystemNavigator>();
        navigator.currentRoadSystem = GameObject.Find("RoadSystem").GetComponent<RoadSystem>();
        vehicleGeneration = FindAnyObjectByType<VehicleGeneration>();
        desiredSpeed = Random.Range(10, 20);
        SetRandomGoal();
        StartCoroutine(SetMovePointLoop());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DynamicSpeed();
        RemoveGameObject();
    }

    private void DynamicSpeed()
    {
        RaycastHit? hit = CheckObjectInfront();
        //DynamicRaycastDistance(hit);
        //if (!special)
        HandleTypeRaycasthit2(hit);
        //else
        //    FunctionCalledByTrafficLight(hit, );
        MoveWithCurrentSpeed();
    }

    private void DynamicRaycastDistance(RaycastHit? hit)
    {
        if (hit.HasValue)
        {
            if ((hit.Value.collider.CompareTag("Vehicle") || DoHitHaveLayer(hit.Value, "Invisible")))
            {
                if (hit.Value.collider.gameObject == rayHitObject)
                {
                    rayDistance = hit.Value.distance;
                }
                else
                {
                    rayDistance = MaximumRayDistance;
                }
            }
            else
            {
                rayDistance = MaximumRayDistance;
            }
            rayHitObject = hit.Value.collider.gameObject;
        }
        else
        {
            rayDistance = MaximumRayDistance;
        }
    }

    private void MoveWithCurrentSpeed()
    {
        if (currentPoint < movePoints.Count)
        {
            Vector3 forwardDirection;
            if (currentPoint == movePoints.Count - 1)
                forwardDirection = (goalObject.gameObject.transform.position - movePoints[currentPoint].position).normalized;
            else
                forwardDirection = (movePoints[currentPoint + 1].position - movePoints[currentPoint].position).normalized;
            Vector3 target = movePoints[currentPoint].position + Vector3.Cross(Vector3.up, forwardDirection).normalized * -1.5f;
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
        Debug.DrawRay(raycastPosition, transform.forward * rayDistance, Color.yellow);
        if (Physics.Raycast(raycastPosition, transform.forward, out hit, rayDistance))
        {
            return hit;
        }
        else
        {
            return null;
        }
    }

    //public void HandleTypeRaycasthit(RaycastHit? hit)
    //{
    //    float distanceBetween;

    //    // if vehicle in front
    //    if (hit.HasValue && DoHitHaveTag(hit.Value, "Vehicle") && IsVehicleSameLane(hit.Value))
    //    {
    //        distanceBetween = hit.Value.distance;
    //        nextObjectSpeed = hit.Value.collider.gameObject.GetComponent<VehicleMovement>().currentSpeed;
    //    }
    //    // at intersection, if red light
    //    else if (hit.HasValue && DoHitHaveLayer(hit.Value, LayerMask.NameToLayer("Invisible")) && hit.Value.collider.gameObject.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Red))
    //    {
    //        // if intersection3 and at outer lane
    //        if (hit.Value.collider.gameObject.transform.parent.parent.CompareTag("Intersection3") && hit.Value.collider.gameObject.transform.parent.name == "Anchor East")
    //        {
    //            // if at outer lane and want to turn right
    //            if (WantToRight())
    //            {
    //                if (ThereisCar(hit.Value.collider.gameObject))
    //                {
    //                    distanceBetween = hit.Value.distance;
    //                    nextObjectSpeed = 0f;
    //                }
    //                else
    //                {
    //                    distanceBetween = 10f;
    //                    nextObjectSpeed = desiredSpeed;
    //                }
    //            }
    //            // if want to go straight, no need stop
    //            else
    //            {
    //                distanceBetween = 10f;
    //                nextObjectSpeed = desiredSpeed;
    //            }
    //        }
    //        // if very close to goal, no need stop
    //        else if (hit.Value.collider.gameObject.transform.parent.parent.CompareTag("Intersection3") && hit.Value.collider.gameObject.transform.parent.name == "Anchor North")
    //        {
    //            // fix
    //            GameObject currentWall = hit.Value.collider.gameObject;
    //            GameObject nextWall = GetNextWallGameObject(currentWall, 1);
    //            GameObject nextWall2 = GetNextWallGameObject(currentWall, 2);
    //            if (WantToRight())
    //            {
    //                distanceBetween = 10f;
    //                nextObjectSpeed = desiredSpeed;
    //            }
    //            else
    //            {
    //                distanceBetween = 10f;
    //                nextObjectSpeed = desiredSpeed;
    //            }
    //            // fix
    //        }
    //        else if (movePoints.Count < 4)
    //        {
    //            distanceBetween = 10f;
    //            nextObjectSpeed = desiredSpeed;
    //        }
    //        // if red light, stop
    //        else
    //        {
    //            distanceBetween = hit.Value.distance;
    //            nextObjectSpeed = 0f;
    //        }
    //    }
    //    else if (hit.HasValue && DoHitHaveLayer(hit.Value, LayerMask.NameToLayer("Invisible")) && hit.Value.collider.gameObject.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Green))
    //    {
    //        distanceBetween = 10f;
    //        nextObjectSpeed = desiredSpeed;
    //    }
    //    else
    //    {
    //        distanceBetween = 10f;
    //        nextObjectSpeed = desiredSpeed;
    //    }
    //    CarFollowingStimulusResponseModel(distanceBetween, nextObjectSpeed);
    //}
    // if vehicle
            // algorithm


        // if intersection4
            // if red light
                // stop

            // if green light
                // if turn right
                    // if opposite road opposite road no car
                        // normal
                    // if opposite road got car
                        // stop
                        

                // if no turn right (turn left or straight)
                    // normal

        // if intersection3
            // if turn right
                // if from East
                    // if West no car
                        // normal
                    // if West got car
                        // stop

                // if from North
                    // if East no car
                        // normal
                    // if East got car
                        // stop

                //


            // if turn left
                // if from West
                    // normal
                // if from North
                    // if West no car
                        // normal
                    // if West got car
                        // stop

            // if straight
                // normal

        // if no car
            // normal
    public void HandleTypeRaycasthit2(RaycastHit? hit)
    {
        if (hit.HasValue && DoHitHaveTag(hit.Value, "Vehicle") && IsVehicleSameLane(hit.Value))
        {
            UpdateStopAttribute(hit.Value);
        }
        else if (
            hit.HasValue
            && DoHitHaveLayer(hit.Value, "Invisible")
            && hit.Value.collider.transform.parent.parent.CompareTag("Intersection4")
            )
        {
            if (hit.Value.collider.gameObject.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Red))
            {
                UpdateStopAttribute(hit.Value);
            }
            else if (hit.Value.collider.gameObject.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Green))
            {
                if (WantToRight())
                {
                    if (CheckIfThereIsCar(GetNextWallGameObject(hit.Value.collider.gameObject, 2)))
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
            && hit.Value.collider.transform.parent.parent.CompareTag("Intersection3")
            )
        {
            if (WantToRight())
            {
                if (hit.Value.collider.gameObject.transform.parent.name == "Anchor East")
                {
                    // Check Anchor West
                    if (CheckIfThereIsCar(GetNextWallGameObject(hit.Value.collider.gameObject, 1)))
                    {
                        UpdateStopAttribute(hit.Value);
                    }
                    else
                    {
                        UpdateNormalAttribute();
                    }
                }
                else if (hit.Value.collider.gameObject.transform.parent.name == "Anchor North")
                {
                    if (CheckIfThereIsCar(GetNextWallGameObject(hit.Value.collider.gameObject, 1)) || CheckIfThereIsCar(GetNextWallGameObject(hit.Value.collider.gameObject, 2)))
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
                if (hit.Value.collider.gameObject.transform.parent.name == "Anchor West")
                {
                    UpdateNormalAttribute();
                }
                else if (hit.Value.collider.gameObject.transform.parent.name == "Anchor North")
                {
                    // Check Anchor West
                    if (CheckIfThereIsCar(GetNextWallGameObject(hit.Value.collider.gameObject, 2)))
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

    private GameObject GetNextWallGameObject(GameObject wall, int num)
    {
        GameObject intersection = wall.transform.parent.parent.gameObject;
        GameObject[] anchors;
        anchors = new GameObject[intersection.transform.childCount - 1];
        int index = 0;
        for (int i = 0; i < anchors.Length + 1; i++)
        {
            GameObject tmpChild = intersection.transform.GetChild(i).gameObject;
            if (tmpChild.name != "IntersectionT" && tmpChild.name != "Intersection")
            {
                anchors[index] = tmpChild;
                index++;
            }
        }


        int next = 0;
        for (int i = 0; i < anchors.Length; i++)
        {
            if (anchors[i].transform.GetChild(0) == wall.transform)
            {
                next = (i + num) % anchors.Length;
                break;
            }
        }

        GameObject nextWall = anchors[next].transform.GetChild(0).gameObject;
        return nextWall;
    }

    private bool CheckIfThereIsCar(GameObject wall)
    {
        RaycastHit hit;
        Vector3 position = wall.transform.position - wall.transform.forward * 10f;
        position.y -= 0.5f;
        Debug.DrawRay(position, wall.transform.forward * 30f, Color.red);
        if (Physics.Raycast(position, wall.transform.forward, out hit, 30f))
        {
            if (hit.collider.transform.CompareTag("Vehicle"))
                return true;
        }
        return false;
    }

    private void UpdateNormalAttribute()
    {
        distanceBetween = 10f;
        nextObjectSpeed = desiredSpeed;
    }

    private void UpdateUsingAlgorithm(RaycastHit hit)
    {
        distanceBetween = hit.distance;
        nextObjectSpeed = hit.collider.gameObject.GetComponent<VehicleMovement>().currentSpeed;
    }

    private void UpdateStopAttribute(RaycastHit hit)
    {
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

    public bool NearToZero()
    {
        if (currentSpeed < 0.0001f && currentSpeed > -0.00001f)
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
            yield return new WaitForSeconds(0.5f);
            SetMovePoints();
            yield return new WaitForSeconds(0.5f);
            SetMovePoints();
            yield return new WaitForSeconds(0.5f);
            SetMovePoints();
            yield return new WaitForSeconds(0.5f);
            SetMovePoints();
            yield return new WaitForSeconds(0.5f);
            if (movePoints.Count > 2)
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
        Bezier.OrientedPoint goal = new Bezier.OrientedPoint(goalObject.transform.position, Vector3.forward, Vector3.up);
        movePoints.Add(goal);
    }

    public void SetRandomGoal()
    {
        GameObject[] goalObjects = GameObject.FindGameObjectsWithTag("Goal");
        if (gameObject.name == "Specific Vehicle 2" || gameObject.name == "Specific Vehicle")
        {
            goalObject = goalObjects[3];
        }
        else if (goalObjects.Length > 0)
        {
            int randomIndex = Random.Range(0, goalObjects.Length);
            goalObject = goalObjects[randomIndex];
        }
        navigator.Goal = goalObject.transform.position;
    }

    public void RecalculatePath()
    {
        navigator.CalculateWayPointsSync();
        movePointReady = false;
    }

    private void OnDrawGizmos()
    {
        if (isSelected)
        {
            for (int i = 0; i < movePoints.Count - 1; i++)
            {
                Debug.DrawLine(movePoints[i].position, movePoints[i + 1].position, Color.blue);
                Gizmos.DrawSphere(movePoints[i].position, 1f);
            }
        }
    }

    private void RemoveGameObject()
    {
        Vector3 center = new Vector3(500f, 0f, 500f);
        float distance = Vector3.Distance(center, gameObject.transform.position);
        if (distance > 800f)
        {
            Destroy(gameObject);
        }
    }

    public void FunctionCalledByTrafficLight(RaycastHit hit, GameObject wall)
    {
        if (wall.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Red) && wall.transform.parent.parent.CompareTag("Intersection4"))
        {
            if (wall.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Red))
            {
                UpdateStopAttribute(hit);
            }
            else if (wall.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Green))
            {
                if (WantToRight())
                {
                    if (CheckIfThereIsCar(GetNextWallGameObject(wall, 2)))
                    {
                        UpdateStopAttribute(hit);
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
        else if (wall.GetComponent<TrafficLightLogic>().IsSameLight(TrafficLightState.Red) && wall.transform.parent.parent.CompareTag("Intersection3"))
        {
            if (WantToRight())
            {
                if (wall.transform.parent.name == "Anchor East" && false)
                {
                    // Check Anchor West
                    if (CheckIfThereIsCar(GetNextWallGameObject(wall, 1)))
                    {
                        UpdateStopAttribute(hit);
                    }
                    else
                    {
                        UpdateNormalAttribute();
                    }
                }
                else if (wall.transform.parent.name == "Anchor North")
                {
                    if (CheckIfThereIsCar(GetNextWallGameObject(wall, 1)))
                    {
                        UpdateStopAttribute(hit);
                    }
                    else
                    {
                        UpdateNormalAttribute();
                    }
                }
            }
            else if (WantToLeft())
            {
                if (wall.transform.parent.name == "Anchor West")
                {
                    UpdateNormalAttribute();
                }
                else if (wall.transform.parent.name == "Anchor North")
                {
                    // Check Anchor West
                    if (CheckIfThereIsCar(GetNextWallGameObject(wall, 2)))
                    {
                        UpdateStopAttribute(hit);
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.CompareTag("Vehicle"))
        {
            currentSpeed = 0;
        }
    }

}
