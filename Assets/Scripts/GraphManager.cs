using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    public GraphMode graphMode;
    List<double> originalTimes = new List<double>();
    List<double> processedTimes = new List<double>();
    List<double> finalTimes = new List<double>();
    public Vector3 baseOrigin;
    public LineRenderer lineRenderer;
    public float widthMultiplier = 30f;
    public float heightMultiplier = 10f;
    public float Height = 30 - (-110);
    int numberOfGroup = 10;
    public GameObject CoordinatePrefab;
    public double SumWaitingTime;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (graphMode == GraphMode.Original)
        {
            baseOrigin = new Vector3(-320, -110, 374);
        }
        else if (graphMode == GraphMode.Modified)
        {
            baseOrigin = new Vector3(50, -110, 374);
            //List<double> times = TimeTrackingManager.TimeWaitedPeriod;
        }
        for (int i=0; i < 103; i++)
        {
            originalTimes.Add(UnityEngine.Random.Range(1, 15));
        }
        ProcessFloatList();
        SetupGraphDimension();
        SetTotalWaitingTime();
        SetupAxis();
        DrawFinalTimes();
        SetCoordinateLabel();
        DisplayTotalWaitingTime();
    }

    void Update()
    {

    }

    private void DrawFinalTimes()
    {
        lineRenderer.positionCount = numberOfGroup;
        Vector3 currentVector = baseOrigin;
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
            currentVector.y = baseOrigin.y + (float)finalTimes[i];
            lineRenderer.SetPosition(i, currentVector);
        }
    }

    private void DisplayTotalWaitingTime()
    {
        Transform totalTimes = transform.Find("WaitingTime");
        totalTimes.GetComponent<TextMeshProUGUI>().text = SumWaitingTime.ToString("F2") + " Seconds";
    }

    private void SetTotalWaitingTime()
    {
        for (int i = 0; i < finalTimes.Count; i++)
        {
            SumWaitingTime += finalTimes[i];
        }
    }

    private void SetupAxis()
    {
        Transform x_axis = transform.Find("X-axis");
        LineRenderer x_renderer = x_axis.GetComponent<LineRenderer>();
        x_renderer.positionCount = 2;
        Vector3 position = baseOrigin;
        position.x -= 3f;
        x_renderer.SetPosition(0, position);
        Vector3 second = baseOrigin;
        second.x += 270;
        x_renderer.SetPosition(1, second);

        Transform y_axis = transform.Find("Y-axis");
        LineRenderer y_renderer = y_axis.GetComponent<LineRenderer>();
        y_renderer.positionCount = 2;
        position = baseOrigin;
        position.y -= 3f;
        y_renderer.SetPosition(0, position);
        second = baseOrigin;
        second.y += 140;
        y_renderer.SetPosition(1, second);

    }

    private void SetCoordinateLabel()
    {
        Transform coordinateLabel = transform.Find("CoordinateLabel");
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            GameObject coordinate = Instantiate(CoordinatePrefab, coordinateLabel);
            coordinate.name = i.ToString();
            Vector3 position = lineRenderer.GetPosition(i);
            position.y += 3f;
            coordinate.transform.position = position;
            coordinate.GetComponent<TextMeshProUGUI>().text = finalTimes[i].ToString("F2");
        }
    }

    private void ProcessFloatList()
    {
        int NumPerGroup = Mathf.FloorToInt(originalTimes.Count / numberOfGroup);
        for (int i=0; i < numberOfGroup; i++)
        {
            double total = 0;
            for (int j = 0; j < NumPerGroup; j++)
            {
                total += originalTimes[i * NumPerGroup + j];
            }
            processedTimes.Add(total);
        }
    }

    private void SetupGraphDimension()
    {
        double highest = GetHighestFromList(processedTimes);
        for (int i = 0; i <  processedTimes.Count; i++)
        {
            finalTimes.Add(processedTimes[i] / highest * Height);
        }
        GameObject maximumGB = transform.Find("Maximum").gameObject;
        //maximumGB.GetComponent<TextMeshProUGUI>().text = highest.ToString();
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
