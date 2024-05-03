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
        SetupAxis();
        DrawFinalTimes();
    }

    void Update()
    {

    }

    private void DrawFinalTimes()
    {
        lineRenderer.positionCount = numberOfGroup;
        Vector3 currentVector = baseOrigin;
        lineRenderer.SetPosition(0, currentVector);
        for (int i = 1; i < numberOfGroup; i++)
        {
            currentVector.x = baseOrigin.x + widthMultiplier * i;
            currentVector.y = baseOrigin.y + (float)finalTimes[i];
            lineRenderer.SetPosition(i, currentVector);
        }
    }

    private void SetupAxis()
    {
        Transform x_axis = transform.Find("X-axis");
        LineRenderer x_renderer = x_axis.GetComponent<LineRenderer>();
        x_renderer.positionCount = 2;
        x_renderer.SetPosition(0, baseOrigin);
        Vector3 second = baseOrigin;
        second.x += 270;
        x_renderer.SetPosition(1, second);

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
        maximumGB.GetComponent<TextMeshProUGUI>().text = highest.ToString();
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
