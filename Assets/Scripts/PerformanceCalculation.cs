using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerformanceCalculation : MonoBehaviour
{
    public GameObject GraphPanel;
    public int originalVehicle = 0;
    public int modifiedVehicle = 0;
    public double originalWaitingTime = 0;
    public double modifiedWaitingTime = 0;
    public double originalAverageWaitingTime = 0;
    public double modifiedAverageWaitingTime = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DisplayTotalWaitingTime();
        DisplayPerformanceResult();
    }

    private void DisplayTotalWaitingTime()
    {
        TextMeshProUGUI textMeshProUGUI;

        originalWaitingTime = GraphPanel.GetComponent<GraphManager>().OriginalSumWaitingTime;
        textMeshProUGUI = transform.Find("OriginalTimeValue").GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = originalWaitingTime.ToString("F0") + " Seconds";
        textMeshProUGUI.color = GraphPanel.transform.Find("Original").GetComponent<LineRenderer>().startColor;

        modifiedWaitingTime = GraphPanel.GetComponent<GraphManager>().ModifiedSumWaitingTime;
        textMeshProUGUI = transform.Find("ModifiedTimeValue").GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = modifiedWaitingTime.ToString("F0") + " Seconds";
        textMeshProUGUI.color = GraphPanel.transform.Find("Modified").GetComponent<LineRenderer>().startColor;

        originalVehicle = GraphPanel.GetComponent<GraphManager>().originalVehicle;
        textMeshProUGUI = transform.Find("OriginalVehicleValue").GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = originalVehicle.ToString("F0") + " Vehicles";
        textMeshProUGUI.color = GraphPanel.transform.Find("Original").GetComponent<LineRenderer>().startColor;

        modifiedVehicle = GraphPanel.GetComponent<GraphManager>().modifiedVehicle;
        textMeshProUGUI = transform.Find("ModifiedVehicleValue").GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = modifiedVehicle.ToString("F0") + " Vehicles";
        textMeshProUGUI.color = GraphPanel.transform.Find("Modified").GetComponent<LineRenderer>().startColor;

        originalAverageWaitingTime = originalWaitingTime / originalVehicle;
        textMeshProUGUI = transform.Find("OriginalAverageValue").GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = originalAverageWaitingTime.ToString("F2") + " Seconds";
        textMeshProUGUI.color = GraphPanel.transform.Find("Original").GetComponent<LineRenderer>().startColor;

        modifiedAverageWaitingTime = modifiedWaitingTime / modifiedVehicle;
        textMeshProUGUI = transform.Find("ModifiedAverageValue").GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = modifiedAverageWaitingTime.ToString("F2") + " Seconds";
        textMeshProUGUI.color = GraphPanel.transform.Find("Modified").GetComponent<LineRenderer>().startColor;
    }

    private void DisplayPerformanceResult()
    {
        if (originalAverageWaitingTime == 0 || modifiedAverageWaitingTime == 0)
            return;

        double performance = ((modifiedAverageWaitingTime - originalAverageWaitingTime) / originalAverageWaitingTime) * 100;
        TextMeshProUGUI textMeshProUGUI = transform.Find("PerformanceValue").GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = performance.ToString("F2") + "%";
        if (performance < 0)
        {
            textMeshProUGUI.color = Color.green;
        }
        else
        {
            textMeshProUGUI.color = Color.red;
        }
    }
}
