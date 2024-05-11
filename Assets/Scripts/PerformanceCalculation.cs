using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerformanceCalculation : MonoBehaviour
{
    public GameObject GraphPanel;
    public double originalWaitingTime = 0;
    public double modifiedWaitingTime = 0;
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
        originalWaitingTime = GraphPanel.GetComponent<GraphManager>().OriginalSumWaitingTime;
        TextMeshProUGUI textMeshProUGUI;
        textMeshProUGUI = transform.Find("OriginalValue").GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = originalWaitingTime.ToString("F0") + " Seconds";
        textMeshProUGUI.color = GraphPanel.transform.Find("Original").GetComponent<LineRenderer>().startColor;

        modifiedWaitingTime = GraphPanel.GetComponent<GraphManager>().ModifiedSumWaitingTime;
        textMeshProUGUI = transform.Find("ModifiedValue").GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = modifiedWaitingTime.ToString("F0") + " Seconds";
        textMeshProUGUI.color = GraphPanel.transform.Find("Modified").GetComponent<LineRenderer>().startColor;
    }

    private void DisplayPerformanceResult()
    {
        if (originalWaitingTime == 0 || modifiedWaitingTime == 0)
            return;

        double performance = ((modifiedWaitingTime - originalWaitingTime) / originalWaitingTime) * 100;
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
