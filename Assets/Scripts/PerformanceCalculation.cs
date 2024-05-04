using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerformanceCalculation : MonoBehaviour
{
    public GraphManager GraphBefore;
    public GraphManager GraphAfter;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DisplayPerformanceResult();
    }

    private void DisplayPerformanceResult()
    {
        double performance = (GraphBefore.SumWaitingTime / GraphAfter.SumWaitingTime) * 100;
        transform.Find("PerformanceValue").GetComponent<TextMeshProUGUI>().text = performance.ToString("F2") + "%";
    }
}
