using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicRoadSystemWeight : MonoBehaviour
{
    public RoadSystem roadSystem;
    public InputManager inputManager;
    public bool IsDynamic = false;
    
    void Start()
    {
        StartCoroutine(UpdateRoadSystemWeight());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleDynamic()
    {
        IsDynamic = !IsDynamic;
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
            }
        }
    }
}
