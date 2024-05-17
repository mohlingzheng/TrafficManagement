using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class GraphManager : MonoBehaviour
{
    public GraphMode graphMode;
    [Header("Original Time Recorded")]
    List<double> originalTimes = new List<double>();
    List<double> processedOriginalTimes = new List<double>();
    List<double> positionOriginalTimes = new List<double>();

    [Header("Modified Time Recorded")]
    List<double> modifiedTimes = new List<double>();
    List<double> processedModifiedTimes = new List<double>();
    List<double> positionModifiedTimes = new List<double>();

    [Header("Canvas Settings")]
    public Vector3 baseOrigin;
    public LineRenderer originalLineRenderer;
    public LineRenderer modifiedLineRenderer;
    public float Height;
    public float Width;
    public float Extralength = 20f;
    public float TopPadding = 8f;
    int numberOfGroup = 10;
    public GameObject CoordinatePrefab;
    float CoordinateMinorDisplacement = 4f;

    [Header("Result Value")]
    public double OriginalSumWaitingTime;
    public double ModifiedSumWaitingTime;

    void Start()
    {
        baseOrigin = new Vector3(-310, -140, 386);
        Width = 420f;
        Height = 170f;
        for (int i = 0; i < 103; i++)
        {
            originalTimes.Add(UnityEngine.Random.Range(1, 15));
        }

        if (TimeTrackingManager.TimeWaitedPeriod.Count == 0)
        {
            for (int i = 0; i < 103; i++)
            {
                modifiedTimes.Add(UnityEngine.Random.Range(1, 15));
            }
        }
        else
        {
            modifiedTimes = TimeTrackingManager.TimeWaitedPeriod;
        }

        SetupAxis();

        GroupTimesInto10Entries(originalTimes, processedOriginalTimes);
        GroupTimesInto10Entries(modifiedTimes, processedModifiedTimes);

        SetupGraphDimension(processedOriginalTimes, positionOriginalTimes);
        SetupGraphDimension(processedModifiedTimes, positionModifiedTimes);

        DrawPositionTimes(originalLineRenderer, positionOriginalTimes);
        DrawPositionTimes(modifiedLineRenderer, positionModifiedTimes);

        CalculateTotalWaitingTime();

        SetCoordinateLabel(originalLineRenderer, processedOriginalTimes);
        SetCoordinateLabel(modifiedLineRenderer, processedModifiedTimes);

        SetupAxisLabel();

        ExportResult();
    }

    void Update()
    {
        SetupAxis();

    }

    private void ExportResult()
    {
        int i = 1;
        string filePath = "Assets/Resources/Data/data" + i + ".txt";

        while (true)
        {
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new System.IO.StreamWriter(filePath))
                {
                    foreach (var data in modifiedTimes)
                    {
                        writer.WriteLine(data);
                    }
                }

                Debug.Log("Data has been written to the file.");
                break;
            }
            else
            {
                i++;
                filePath = "Assets/Resources/Data/data" + i + ".txt";
            }
        }
    }

    private void OnDrawGizmos()
    {
        //for (int i = 0; i < lineRenderer.positionCount; i++)
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawSphere(lineRenderer.GetPosition(i), 2f);
        //}
    }

    private void SetupAxisLabel()
    {
        int capturePerEntries = 5;
        int NumPerGroup = Mathf.FloorToInt(originalTimes.Count / numberOfGroup);
        float increase = capturePerEntries * NumPerGroup;
        Transform x_axisLabel = transform.Find("X-axis-label");
        for (int i = 0; i < numberOfGroup; i++)
        {
            GameObject coordinate = Instantiate(CoordinatePrefab, x_axisLabel);
            coordinate.name = i.ToString();
            Vector3 position = originalLineRenderer.GetPosition(i);
            position.y = -140 - CoordinateMinorDisplacement;
            coordinate.transform.position = position;
            float value = increase * i;
            coordinate.GetComponent<TextMeshProUGUI>().text = value.ToString("F0");
        }

        //double highestValue = GetHighestFromList(processedModifiedTimes);
        //float valuegap = 30f;
        //if (highestValue <= 100)
        //{
        //    valuegap = 10f;
        //}
        //else if (highestValue <= 200)
        //{
        //    valuegap = 20f;
        //}

        //int count = (int)(highestValue / valuegap);
        //float heightgap = (float)highestValue / count;

        //Transform y_axisLabel = transform.Find("Y-axis-label");
        //for (int i = 0; i < count; i++)
        //{
        //    GameObject coordinate = Instantiate(CoordinatePrefab, y_axisLabel);
        //    coordinate.name = i.ToString();
        //    Vector3 position = new Vector3(-310 - CoordinateMinorDisplacement, -140, 386);
        //    position.y += (heightgap * i);
        //    coordinate.transform.position = position;
        //    float value = valuegap * i;
        //    coordinate.GetComponent<TextMeshProUGUI>().text = value.ToString("F0");
        //}

    }

    private void DrawPositionTimes(LineRenderer lineRenderer, List<double> timeList)
    {
        lineRenderer.positionCount = numberOfGroup;
        Vector3 currentVector = baseOrigin;
        float widthMultiplier = Width / numberOfGroup;
        //lineRenderer.SetPosition(0, currentVector);
        //for (int i = 1; i < numberOfGroup; i++)
        //{
        //    currentVector.x = baseOrigin.x + widthMultiplier * i;
        //    currentVector.y = baseOrigin.y + (float)finalTimes[i];
        //    lineRenderer.SetPosition(i, currentVector);
        //}

        for (int i = 0; i < numberOfGroup; i++)
        {
            currentVector.x = baseOrigin.x + widthMultiplier * i;
            currentVector.y = baseOrigin.y + (float)timeList[i];
            lineRenderer.SetPosition(i, currentVector);
        }
    }

    private void CalculateTotalWaitingTime()
    {
        for (int i = 0; i < processedOriginalTimes.Count; i++)
        {
            OriginalSumWaitingTime += processedOriginalTimes[i];
        }
        for (int i = 0; i < processedModifiedTimes.Count; i++)
        {
            ModifiedSumWaitingTime += processedModifiedTimes[i];
        }
    }

    private void SetupAxis()
    {
        Transform x_axis = transform.Find("X-axis");
        LineRenderer x_renderer = x_axis.GetComponent<LineRenderer>();
        x_renderer.positionCount = 2;
        Vector3 position = baseOrigin;
        position.x -= Extralength;
        x_renderer.SetPosition(0, position);
        Vector3 second = baseOrigin;
        second.x += Width;
        x_renderer.SetPosition(1, second);

        Transform y_axis = transform.Find("Y-axis");
        LineRenderer y_renderer = y_axis.GetComponent<LineRenderer>();
        y_renderer.positionCount = 2;
        position = baseOrigin;
        position.y -= Extralength;
        y_renderer.SetPosition(0, position);
        second = baseOrigin;
        second.y += Height;
        y_renderer.SetPosition(1, second);

    }

    private void SetCoordinateLabel(LineRenderer lineRenderer, List<double> timeList)
    {
        Transform coordinateLabel = transform.Find("Original").Find("CoordinateLabel");
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            GameObject coordinate = Instantiate(CoordinatePrefab, coordinateLabel);
            coordinate.name = i.ToString();
            coordinate.GetComponent<TextMeshProUGUI>().color = lineRenderer.startColor;
            bool Upper;
            if (i == 0)
            {
                Upper = PutAtUpper(null, lineRenderer.GetPosition(i).y, lineRenderer.GetPosition(i + 1).y);
            }
            else if (i ==  (lineRenderer.positionCount - 1))
            {
                Upper = PutAtUpper(lineRenderer.GetPosition(i - 1).y, lineRenderer.GetPosition(i).y, null);
            }
            else
            {
                Upper = PutAtUpper(lineRenderer.GetPosition(i - 1).y, lineRenderer.GetPosition(i).y, lineRenderer.GetPosition(i + 1).y);
            }
            Vector3 position = lineRenderer.GetPosition(i);
            if (Upper)
                position.y += CoordinateMinorDisplacement;
            else
                position.y -= CoordinateMinorDisplacement;
            coordinate.transform.position = position;
            coordinate.GetComponent<TextMeshProUGUI>().text = timeList[i].ToString("F0");
        }
    }

    private bool PutAtUpper(float? f1, float? f2, float? f3)
    {
        if (!f1.HasValue && f2.Value <= f3.Value)
            return false;
        else if (!f1.HasValue && f2.Value >= f3.Value)
            return true;
        else if (!f3.HasValue && f1.Value >= f2.Value)
            return false;
        else if (!f3.HasValue && f1.Value <= f2.Value)
            return true;
        else if (f1.Value >= f2.Value && f2.Value <= f3.Value)
            return false;
        else
            return true;
    }

    private void GroupTimesInto10Entries(List<double> timeList, List<double> processedList)
    {
        int NumPerGroup = Mathf.FloorToInt(timeList.Count / numberOfGroup);
        for (int i=0; i < numberOfGroup; i++)
        {
            double total = 0;
            for (int j = 0; j < NumPerGroup; j++)
            {
                total += timeList[i * NumPerGroup + j];
            }
            processedList.Add(total);
        }
    }

    private void SetupGraphDimension(List<double> processedList, List<double> positionList)
    {
        double highest = GetHighestFromList(processedList);
        for (int i = 0; i < processedList.Count; i++)
        {
            positionList.Add((processedList[i] / highest) * (Height - TopPadding));
        }
    }

    private double GetHighestFromList(List<double> list)
    {
        double highest = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] > highest)
                highest = list[i];
        }
        return highest;
    }
}
