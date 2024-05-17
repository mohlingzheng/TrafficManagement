using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataCollectionManager : MonoBehaviour
{
    public GameObject SpecificRoad;
    public GameObject vehicle;
    public GameObject differentVehicle;
    public GameObject VehicleList;

    public GameObject created;

    public GameObject StartIntersection;
    public GameObject EndIntersection;
    public RoadSystem roadSystem;

    public bool reach = false;
    public bool start = false;
    public int generateNumber;

    public int numCar;
    public float timer;
    public float roadLength;
    public float trafficDensity;

    // Start is called before the first frame update
    void Start()
    {
        generateNumber = Random.Range(0, 25);
        Working();
        //SetRandomEndRoad();
        StartCoroutine(GenerateVehicle(generateNumber));
        StartCoroutine(StartMyVehicle());
    }

    // Update is called once per frame
    void Update()
    {
        CalculateTrafficDensity();
        AddTimer();
        CheckRoadTypeOn(created, true);
        ReachThenOutputData();
        Respawn();
        CheckInput();
    }

    private void SetRandomEndRoad()
    {
        int px = Random.Range(830, 1500);
        int pz = Random.Range(200, 1000);
        EndIntersection.transform.position = new Vector3(px, 0.15f, pz);
        roadSystem.RebuildAllRoads();
    }

    IEnumerator StartMyVehicle()
    {
        yield return new WaitForSeconds(generateNumber);
        GenerateSpecificVehicle("Specific Vehicle");
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            start = true;
            GenerateSpecificVehicle("Specific Vehicle");
        }
    }

    private void Respawn()
    {
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("Vehicle");
        if (vehicles.Length == 0)
            return;
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i].name == "Specific Vehicle")
                continue;
            if (CheckRoadTypeOn(vehicles[i]))
            {
                GenerateManyVehicle();
                Destroy(vehicles[i]);
            }
        }
    }

    private void CalculateTrafficDensity()
    {
        RoadTrafficDensity roadTrafficDensity = SpecificRoad.GetComponent<RoadTrafficDensity>();
        numCar = generateNumber;
        roadLength = roadTrafficDensity.RoadLength;
        trafficDensity = numCar / roadLength;
    }

    IEnumerator GenerateVehicle(int number)
    {
        for (int i = 0; i < number; i++)
        {
            GenerateManyVehicle();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void AddTimer()
    {
        timer += Time.deltaTime;
    }

    private void Working()
    {
        Time.timeScale = 20f;
        KomtarSceneManager.IsPaused = false;
    }

    private void GenerateSpecificVehicle(string name = "Vehicle")
    {
        created = Instantiate(differentVehicle, VehicleList.transform);
        created.transform.position = GameObject.Find("StartIntersection").transform.position;
        created.name = name;
    }
    private void GenerateManyVehicle()
    {
        GameObject veh = Instantiate(vehicle, VehicleList.transform);
        veh.transform.position = GameObject.Find("StartIntersection").transform.position;
        veh.name = "Vehicle";
    }

    private bool CheckAnyVehicle()
    {
        RaycastHit hit;
        if (Physics.Raycast(StartIntersection.transform.position, transform.up, out hit, 100f))
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

    private void ReachThenOutputData()
    {
        if (reach)
        {
            ExportResult();
            this.gameObject.SetActive(false);
        }
    }

    private void ExportResult()
    {
        string filePath = "Assets/Resources/DataCollection/data.txt";

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length >= 3000)
            {
                Debug.Log(lines.Length);
                #if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
                    Debug.Log("Play mode stopped.");
                #endif
            }
        }

        if (!File.Exists(filePath))
        {
            using (StreamWriter writer = new System.IO.StreamWriter(filePath))
            {
                writer.WriteLine(roadLength);
                writer.WriteLine(trafficDensity);
                writer.WriteLine(timer);
            }

            Debug.Log("Data has been written to the file.");
        }
        else
        {
            using (StreamWriter writer = new System.IO.StreamWriter(filePath, append: true))
            {
                writer.WriteLine(roadLength);
                writer.WriteLine(trafficDensity);
                writer.WriteLine(timer);
            }

            Debug.Log("Data has been appended to the existing file.");
        }

        SceneManager.LoadScene(4);

    }


    private bool CheckRoadTypeOn(GameObject theVehicle, bool specific = false)
    {
        if (theVehicle == null)
            return false;
        RaycastHit hit;
        Vector3 position = theVehicle.transform.position;
        position.y += 0.5f;
        if (Physics.Raycast(position, Vector3.down, out hit, 2f))
        {
            if (hit.collider.transform.parent != null)
            {
                if (hit.collider.transform.parent.name == "EndIntersection")
                {
                    if (specific)
                        reach = true;
                    return true;
                }
            }
        }
        return false;
    }
}
