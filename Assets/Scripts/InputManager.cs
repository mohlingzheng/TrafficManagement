using Barmetler.RoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public PreviewManager previewSystem;

    [Header("Input Mode")]
    public InputMode inputMode = InputMode.Build;

    [Header("Canvas")]
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
            //GetBuildingRoadPositions();
            GetBuildingRoadPositionsForPreview();
        }
        DeveloperInteraction();
    }

    private void DeveloperInteraction()
    {
        if ((int)Input.GetAxis("LeftVertical") != 0)
        {
            vehicleGeneration.carLimit = vehicleGeneration.carLimit + (int)Input.GetAxis("LeftVertical");
            vehicleGeneration.carLimit = Mathf.Clamp(vehicleGeneration.carLimit, 0, 100);
            developer.GetComponentInChildren<TextMeshProUGUI>().text = vehicleGeneration.carLimit.ToString();
        }
    }

    public void GetRaycastObjectHit()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(pointer.gameObject.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue);
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
            }
        }

        if (Input.GetButtonDown("B"))
        {
            if (selection != null)
            {
                selection.gameObject.GetComponent<Outline>().enabled = false;
                selection = null;
            }
        }
    }

    private void GetBuildingRoadPositionsForPreview()
    {
        if (Input.GetButtonDown("A") && pointedGameObject.CompareTag("Road"))
        {
            if (firstPoint == Vector3.zero)
            {
                firstPoint = pointedPosition;
                previewSystem.CreatePreviewRoad(pointedGameObject, firstPoint);
            }
            else
            {
                secondPoint = pointedPosition;
                //secondAnchor = roadBuildingManager.AddIntersectionToSingleRoad2(pointedGameObject, secondPoint, Quaternion.Euler(0, -90f, 0));
            }
            if (firstPoint != Vector3.zero && secondPoint != Vector3.zero)
            {
                //roadBuildingManager.ConnectTwoIntersections(firstPoint, firstAnchor, secondAnchor);
                firstPoint = Vector3.zero;
                secondPoint = Vector3.zero;
            }
        }

        if (Input.GetButtonDown("B"))
        {
            if (secondPoint != Vector3.zero)
            {
                secondPoint = Vector3.zero;
            }
            else
            {
                firstPoint = Vector3.zero;
            }
        }

    }

    private void GetBuildingRoadPositions()
    {
        if (Input.GetButtonDown("A") && pointedGameObject.CompareTag("Road"))
        {
            if (firstPoint == Vector3.zero)
            {
                firstPoint = pointedPosition;
                firstAnchor = roadBuildingManager.AddIntersectionToSingleRoad(pointedGameObject, firstPoint);
            }
            else
            {
                secondPoint = pointedPosition;
                secondAnchor = roadBuildingManager.AddIntersectionToSingleRoad2(pointedGameObject, secondPoint, Quaternion.Euler(0, -90f, 0));
            }
            if (firstPoint != Vector3.zero && secondPoint != Vector3.zero)
            {
                roadBuildingManager.ConnectTwoIntersections(firstPoint, firstAnchor, secondAnchor);
                firstPoint = Vector3.zero;
                secondPoint = Vector3.zero;
            }
        }

        if (Input.GetButtonDown("B"))
        {
            if (secondPoint != Vector3.zero)
            {
                secondPoint = Vector3.zero;
            }
            else
            {
                firstPoint = Vector3.zero;
            }
        }
        
    }

    public void GetRaycastPositionHit()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(pointer.gameObject.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.white);
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
}
