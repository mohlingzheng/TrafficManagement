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

    // Start is called before the first frame update
    void Start()
    {
        navigator = GetComponent<RoadSystemNavigator>();
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

/*            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);*/
        }
    }

    public void SetMovePoints()
    {
        movePoints = navigator.CurrentPoints;
    }
}
