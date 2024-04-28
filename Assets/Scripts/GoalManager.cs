using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;

public class GoalManager : MonoBehaviour
{

    public GameObject test;
    public GameObject road;
    public Collider testCollider;
    public Collider roadCollider;
    public GameObject Prefabs;
    int count = 0;

    void Start()
    {
        RemoveBoxCollider();
        //testCollider = test.GetComponent<Collider>();
        //roadCollider = road.GetComponent<Collider>();
    }

    private void RemoveBoxCollider()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag(Tag.Goal);
        foreach (GameObject building in buildings)
        {
            if (building.GetComponent<Collider>() == null)
            {
                //building.GetComponent<Collider>().enabled = false;
                building.AddComponent<BoxCollider>();
            }
        }
    }

    void Update()
    {
        //count++;
        //if (count >= 120)
        //{
        //    Test();
        //    count &= 120;
        //}
    }

    void Test()
    {
        if (testCollider != null && roadCollider != null)
        {
            if (testCollider.bounds.Intersects(roadCollider.bounds))
            {
                Debug.Log("collide");
            }
        }
    }
}
