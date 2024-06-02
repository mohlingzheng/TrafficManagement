using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

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
        string filePath = "Assets/Resources/Database/queuepoints.txt";

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] split = lines[i].Split(",");
                float x = float.Parse(split[0].Trim());
                float y = float.Parse(split[1].Trim());
                float z = float.Parse(split[2].Trim());
                queuePointsPosition.Add(new Vector3(x, y, z));
            }
        }
        else
        {
            for (int i = 0; i < FixedQueuePoints.QueuePoints.Count; i++)
            {
                queuePointsPosition.Add(FixedQueuePoints.QueuePoints[i]);
            }
        }

        //queuePointsPosition.Add(new Vector3(165.5f, 0f, 30.3f));
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
