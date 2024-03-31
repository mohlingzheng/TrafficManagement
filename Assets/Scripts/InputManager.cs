using Barmetler.RoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;
    public Button pauseButton;
    public Button resumeButton;
    public Image pointer;
    public RoadBuildingManager roadBuildingManager;
    public RoadSystem roadSystem;
    public RoadSystem previewRoadSystem;
    public IntersectionManager intersectionManager;

    [Header("Input Mode")]
    public InputMode inputMode = InputMode.Build;

    [Header("Canvas")]
    public GameObject roadBuildingPanel;
    public GameObject buttonPanel;

    [Header("Raycast")]
    public Vector3 pointedPosition = Vector3.zero;
    public GameObject pointedGameObject = null;


    [Header("Outline")]
    public Transform highlight;
    public Transform selection;

    [Header("Road Building")]
    public Vector3 firstPoint = Vector3.zero;
    public Vector3 secondPoint = Vector3.zero;
    public RoadAnchor firstAnchor = null;
    public RoadAnchor secondAnchor = null;
    public GameObject firstSelectedGameObject = null;
    public GameObject secondSelectedGameObject = null;
    public bool confirm = false;

    [Header("Developer")]
    public VehicleGeneration vehicleGeneration;
    public GameObject developer;


    void Start()
    {
        ChangeUIWithInputMode();
    }

    void FixedUpdate()
    {
        ButtonInteraction();
    }

    public void YButtonInteractButton()
    {
        if (pauseButton.IsActive())
            ExecuteEvents.Execute(pauseButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        else
            ExecuteEvents.Execute(resumeButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(true);
        Debug.Log("pause");
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pauseButton.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(false);
        Debug.Log("resume");
    }

    void ButtonInteraction()
    {
        // Input.GetAxis or Input.GetButtonDown
        // For Keyboard: Input.GetKeyDown(KeyCode.)
        HandleInputModeChange();
        GetRaycastPositionHit();
        GetRaycastObjectHit();
        if (inputMode == InputMode.Default)
        {
            SelecteOutlineObject();
        }
        else if (inputMode == InputMode.Build)
        {
            //GetBuildingRoadPositionsForPreview();
            GetInputForPreviewBuilding();
        }
        DeveloperInteraction();
    }

    private void DeveloperInteraction()
    {
        if ((int)Input.GetAxis("LeftVertical") != 0)
        {
            vehicleGeneration.carLimit = vehicleGeneration.carLimit + (int)Input.GetAxis("LeftVertical");
            vehicleGeneration.carLimit = Mathf.Clamp(vehicleGeneration.carLimit, 0, 1000);
            developer.GetComponentInChildren<TextMeshProUGUI>().text = vehicleGeneration.carLimit.ToString();
        }

    }

    public void GetRaycastObjectHit()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(pointer.gameObject.transform.position);
        //Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && !EventSystem.current.IsPointerOverGameObject())
        {
            OutlineGameObject(hit.transform, hit);
            pointedGameObject = hit.collider.gameObject;
        }
        else
            pointedGameObject = null;
    }

    public void OutlineGameObject(Transform transform, RaycastHit raycastHit)
    {
        // if previously highlight some gameobject, disable it and set to null
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            if (highlight.CompareTag("Vehicle"))
            {
                highlight.transform.GetComponent<VehicleMovement>().isSelected = false;
            }
            highlight = null;
        }

        highlight = transform;

        if (highlight != selection)
        {
            if (highlight.gameObject.GetComponent<Outline>() != null)
            {
                highlight.gameObject.GetComponent<Outline>().enabled = true;
            }
            else
            {
                Outline outline = highlight.gameObject.AddComponent<Outline>();
                outline.OutlineColor = new Color(91, 250, 98);
                outline.OutlineWidth = 2;
                outline.enabled = true;
                //highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.magenta;
                //highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
            }
            //Debug.Log(highlight.name);
            if (highlight.CompareTag("Vehicle"))
            {
                highlight.transform.GetComponent<VehicleMovement>().isSelected = true;
            }
        }
        else
        {
            highlight = null;
        }

        
    }

    private void SelecteOutlineObject()
    {
        if (Input.GetButtonDown("A"))
        {
            if (highlight)
            {
                if (selection != null)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                }
                selection = highlight;
                Outline outline = selection.gameObject.GetComponent<Outline>();
                outline.enabled = true;
                outline.OutlineColor = new Color(91, 250, 98);
                outline.OutlineWidth = 2;
                highlight = null;

                if (selection.CompareTag("Vehicle"))
                {
                    VehicleMovement vehicleMovement = selection.GetComponent<VehicleMovement>();
                    vehicleMovement.isSelected = true;
                }
            }
        }

        if (Input.GetButtonDown("B"))
        {
            if (selection != null)
            {
                if (selection.CompareTag("Vehicle"))
                {
                    VehicleMovement vehicleMovement = selection.GetComponent<VehicleMovement>();
                    vehicleMovement.isSelected = false;
                }

                selection.gameObject.GetComponent<Outline>().enabled = false;
                selection = null;
            }
        }
    }

    private void GetInputForPreviewBuilding()
    {
        if (Input.GetButtonDown("A") && confirm == false)
        {
            if (firstPoint == Vector3.zero)
            {
                if (pointedGameObject.CompareTag("Road"))
                {
                    Debug.Log("first road");
                    firstPoint = pointedPosition;
                    firstSelectedGameObject = pointedGameObject;
                    firstAnchor = roadBuildingManager.CreatePreviewRoad(previewRoadSystem.gameObject, pointedGameObject, firstPoint, BuildMode.Preview);
                    SetUIOnRoadPreview(1);
                }
                else if (pointedGameObject.transform.parent.CompareTag("Intersection3"))
                {
                    Debug.Log("first intersection");
                    firstPoint = pointedPosition;
                    firstSelectedGameObject = pointedGameObject;
                    firstAnchor = roadBuildingManager.CreatePreviewIntersection(previewRoadSystem.gameObject, pointedGameObject.transform.parent.gameObject, firstPoint, BuildMode.Preview);
                    SetUIOnRoadPreview(1);
                }
                else
                {
                    Debug.Log("Do nothing");
                }
            }
            else if (secondPoint == Vector3.zero)
            {
                if (pointedGameObject == firstSelectedGameObject)
                {
                    Debug.Log("Cannot select same road or same intersection");
                }
                else if (pointedGameObject.CompareTag("Road"))
                {
                    Debug.Log("second road");
                    secondPoint = pointedPosition;
                    secondSelectedGameObject = pointedGameObject;
                    secondAnchor = roadBuildingManager.CreatePreviewRoad(previewRoadSystem.gameObject, pointedGameObject, secondPoint, BuildMode.Preview);
                    SetUIOnRoadPreview(2);
                }
                else if (pointedGameObject.transform.parent.CompareTag("Intersection3"))
                {
                    Debug.Log("second intersection");
                    secondPoint = pointedPosition;
                    secondSelectedGameObject = pointedGameObject;
                    secondAnchor = roadBuildingManager.CreatePreviewIntersection(previewRoadSystem.gameObject, pointedGameObject.transform.parent.gameObject, secondPoint, BuildMode.Preview);
                    SetUIOnRoadPreview(2);
                }
                else
                {
                    Debug.Log("Do nothing");
                }
            }
            // do connection
            if (firstPoint != Vector3.zero && secondPoint != Vector3.zero)
            {
                Debug.Log("Preview Connect");
                roadBuildingManager.ConnectTwoIntersections(previewRoadSystem.gameObject, firstAnchor, secondAnchor, BuildMode.Preview);
                confirm = true;
            }

        }
        else if (Input.GetButtonDown("A") && confirm)
        {
            Debug.Log("implement build");
            if (firstSelectedGameObject.CompareTag("Road"))
                firstAnchor = roadBuildingManager.CreatePreviewRoad(roadSystem.gameObject, firstSelectedGameObject, firstPoint, BuildMode.Actual);
            else 
                firstAnchor = roadBuildingManager.CreatePreviewIntersection(roadSystem.gameObject, firstSelectedGameObject.transform.parent.gameObject, firstPoint, BuildMode.Actual);
            
            if (secondSelectedGameObject.CompareTag("Road"))
                secondAnchor = roadBuildingManager.CreatePreviewRoad(roadSystem.gameObject, secondSelectedGameObject, secondPoint, BuildMode.Actual);
            else
                secondAnchor = roadBuildingManager.CreatePreviewIntersection(roadSystem.gameObject, secondSelectedGameObject.transform.parent.gameObject, secondPoint, BuildMode.Actual);

            roadBuildingManager.ConnectTwoIntersections(roadSystem.gameObject, firstAnchor, secondAnchor, BuildMode.Actual);
            ResetRoadBuilding();
            DestroyAllPreviewObject();
            intersectionManager.GetLatestIntersection();
            PathFindingRecalculate();
            SetUIOnRoadPreview(5);
        }
        else if (Input.GetButtonDown("B"))
        {
            if (secondPoint != Vector3.zero)
            {
                Debug.Log("second preview cancel");
                secondPoint = Vector3.zero;
                secondAnchor = null;
                secondSelectedGameObject = null;
                foreach (Transform child in previewRoadSystem.transform)
                {
                    if (child.name == "2")
                        Destroy(child.gameObject);
                }
                roadBuildingManager.ReduceCount();
                SetUIOnRoadPreview(4);
                confirm = false;
            }
            else if (firstPoint != Vector3.zero)
            {
                Debug.Log("first preview cancel");
                firstPoint = Vector3.zero;
                firstAnchor = null;
                firstSelectedGameObject = null;
                foreach (Transform child in previewRoadSystem.transform)
                {
                    Destroy(child.gameObject);
                }
                roadBuildingManager.ReduceCount();
                SetUIOnRoadPreview(3);
            }
        }
    }

    public void SetUIOnRoadPreview(int num)
    {
        TextMeshProUGUI[] texts = roadBuildingPanel.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (TextMeshProUGUI text in texts)
        {
            if (num == 1 && text.gameObject.name == "Text First")
            {
                text.text = "First Road built";
            }
            if (num == 2 && text.gameObject.name == "Text Second")
            {
                text.text = "Second Road built";
            }
            if (num == 3 && text.gameObject.name == "Text First")
            {
                text.text = "First Build";
            }
            if (num == 4 && text.gameObject.name == "Text Second")
            {
                text.text = "Second Built";
            }
            if (num == 5 && text.gameObject.name == "Text First")
            {
                text.text = "First Built";
            }
            if (num == 5 && text.gameObject.name == "Text Second")
            {
                text.text = "Second Built";
            }
        }
    }

    public void ResetRoadBuilding()
    {
        firstPoint = Vector3.zero;
        secondPoint = Vector3.zero;
        firstAnchor = null;
        secondAnchor = null;
        firstSelectedGameObject = null;
        secondSelectedGameObject = null;
        confirm = false;
    }

    public void GetRaycastPositionHit()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(pointer.gameObject.transform.position);
        //Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.white);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            pointedPosition = hit.point;
        }
    }

    private void ChangeUIWithInputMode()
    {
        if (inputMode == InputMode.Default)
        {
            buttonPanel.transform.GetChild(0).GetComponent<Image>().color = Color.green;
            buttonPanel.transform.GetChild(1).GetComponent<Image>().color = Color.white;
        }
        else if (inputMode == InputMode.Build)
        {
            buttonPanel.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            buttonPanel.transform.GetChild(1).GetComponent<Image>().color = Color.green;
        }
    }

    private void HandleInputModeChange()
    {
        int size = Enum.GetValues(typeof(InputMode)).Length;
        int currentMode = (int)inputMode;
        if (Input.GetButtonDown("LB"))
        {
            currentMode = (currentMode - 1);
            currentMode = Mathf.Clamp(currentMode, 0, size);
            inputMode = (InputMode)currentMode;
            Debug.Log("Change InputMode to " + inputMode);
            ChangeUIWithInputMode();
        }

        if (Input.GetButtonDown("RB"))
        {
            currentMode = (currentMode + 1);
            currentMode = Mathf.Clamp(currentMode, 0, size);
            inputMode = (InputMode)currentMode;
            Debug.Log("Change InputMode to " + inputMode);
            ChangeUIWithInputMode();
        }

    }

    private void PathFindingRecalculate()
    {
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("Vehicle");
        foreach (GameObject vehicle in vehicles)
        {
            VehicleMovement vehicleMovement = vehicle.GetComponent<VehicleMovement>();
            StartCoroutine(vehicleMovement.SetMovePointLoop());
        }
    }

    private void DestroyAllPreviewObject()
    {
        foreach (Transform child in previewRoadSystem.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
