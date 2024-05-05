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
        transform.Find("OriginalValue").GetComponent<TextMeshProUGUI>().text = originalWaitingTime.ToString("F2") + " Seconds";

        modifiedWaitingTime = GraphPanel.GetComponent<GraphManager>().ModifiedSumWaitingTime;
        transform.Find("ModifiedValue").GetComponent<TextMeshProUGUI>().text = modifiedWaitingTime.ToString("F2") + " Seconds";
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
