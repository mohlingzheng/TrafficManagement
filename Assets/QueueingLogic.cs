using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class QueueingLogic : MonoBehaviour
{
    public GameObject VehicleList;
    public Queue<GameObject> queue;
    public int Count;
    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<GameObject>();
        VehicleList = GameObject.Find("VehicleList");
        StartCoroutine(GenerateVehicle());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GenerateVehicle()
    {
        while (true)
        {
            if (queue.Count > 0)
            {
                Count = queue.Count;
                if (!CheckAnyVehicle())
                {
                    GameObject vehicle = queue.Dequeue();
                    Instantiate(vehicle, VehicleList.transform);
                    vehicle.transform.position = transform.position;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }

    }

    private bool CheckAnyVehicle()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit, 100f))
        {
            if (Tag.CompareTags(hit.collider.transform, Tag.Vehicle))
                return true;
            return false;
        }
        else
        {
            return false;
        }
    }
}
