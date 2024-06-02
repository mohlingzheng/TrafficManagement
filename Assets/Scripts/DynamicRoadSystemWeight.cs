using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicRoadSystemWeight : MonoBehaviour
{
    public RoadSystem roadSystem;
    public InputManager inputManager;
    public bool IsDynamic = false;
    public float timer = 0f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDynamic)
            timer += Time.deltaTime;
    }

    public void ToggleDynamic()
    {
        IsDynamic = !IsDynamic;
        if (IsDynamic)
            StartCoroutine(UpdateRoadSystemWeight());
        else
            StopCoroutine(UpdateRoadSystemWeight());
        timer = 0f;
    }

    IEnumerator UpdateRoadSystemWeight()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            if (IsDynamic)
            {
                roadSystem.ConstructGraph();
                inputManager.PathFindingRecalculate();
                timer = 0f;
            }
        }
    }
}
