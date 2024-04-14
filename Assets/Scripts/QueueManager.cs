using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public GameObject QueuePointPrefab;
    public List<Vector3> queuePointsPosition;
    public List<GameObject> queuePointsList;

    void Start()
    {
        SetQueuePointsPosition();
        GenerateQueuePoints();
    }

    void Update()
    {
        
    }

    void SetQueuePointsPosition()
    {
        //queuePointsPosition.Add(new Vector3(100f, 0f, 4.2f));
        //queuePointsPosition.Add(new Vector3(528f, 0f, 12.9f));
        //queuePointsPosition.Add(new Vector3(408f, 0f, 4.2f));
        //queuePointsPosition.Add(new Vector3(210f, 0f, 5.6f));
        //queuePointsPosition.Add(new Vector3(1f, 0f, 763.6f));
        //queuePointsPosition.Add(new Vector3(100.5f, 0f, 999f));
        //queuePointsPosition.Add(new Vector3(606f, 0f, 1.5f));
        queuePointsPosition.Add(new Vector3(165.5f, 0f, 30.3f));
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(Tag.Goal);
        foreach (GameObject building in buildings)
        {
            queuePointsPosition.Add(building.transform.position);
        }
    }

    void GenerateQueuePoints()
    {
        GameObject queuePoint;
        for (int i = 0; i < queuePointsPosition.Count; i++)
        {
            queuePoint = Instantiate(QueuePointPrefab, transform);
            queuePoint.transform.position = queuePointsPosition[i];
            queuePoint.transform.rotation = Quaternion.identity;
            queuePointsList.Add(queuePoint);
        }
    }
}
