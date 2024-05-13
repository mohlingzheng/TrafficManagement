using Barmetler.RoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;
    public Image pointer;
    public RoadBuildingManager roadBuildingManager;
    public RoadSystem roadSystem;
    public RoadSystem previewRoadSystem;
    public IntersectionManager intersectionManager;
    public DisplayManager displayManager;

    [Header("Input Mode")]
    public InputMode inputMode = InputMode.Default;

    [Header("Canvas")]
    public GameObject roadBuildingPanel;
    public GameObject buttonPanel;
    public Button SelectButton;

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
        if (EventSystem.current == null)
        {
            Debug.LogError("No EventSystem found in the scene.");
            return;
        }
    }

    void Update()
    {
        if (!KomtarSceneManager.IsPaused)
            ButtonInteraction();
        //if (!selection)
        //{
        //    selection = GameObject.FindGameObjectWithTag(Tag.Vehicle).transform;
        //}
    }

    //public void YButtonInteractButton()
    //{
    //    if (pauseButton.IsActive())
    //        ExecuteEvents.Execute(pauseButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    //    else
    //        ExecuteEvents.Execute(resumeButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    //}

    void ButtonInteraction()
    {
        HandleInputModeChange();
        GetRaycastPositionHit();

        GetRaycastObjectHit();

        if (inputMode == InputMode.Default)
        {
            SelecteOutlineObject();
        }
        else if (inputMode == InputMode.Build)
        {
            GetInputForPreviewBuilding();
        }
        else if (inputMode == InputMode.Remove)
        {
            GetInputForRemoveRoad();
        }

        DeveloperInteraction();
    }

    private void GetInputForRemoveRoad()
    {
        if (Input.GetButtonDown("A") && confirm == false)
        {
            if (firstPoint == Vector3.zero && pointedGameObject.transform.parent == roadSystem.gameObject.transform)
            {
                if (Tag.CompareTags(pointedGameObject.transform, Tag.Road_Small, Tag.Road_Large))
                {
                    Debug.Log("Remove Road, Correct Selection of Road");
                    firstPoint = pointedPosition;
                    firstSelectedGameObject = pointedGameObject;
                    roadBuildingManager.CreatePreviewRemovedRoad(previewRoadSystem.gameObject, firstSelectedGameObject, firstPoint, BuildMode.Preview);
                    confirm = true;
                }
                else
                {
                    Debug.Log("Do nothing");
                }
            }
        }
        else if (Input.GetButtonDown("A") && confirm == true)
        {
            Debug.Log("Implement Road Removing");
            roadBuildingManager.CreatePreviewRemovedRoad(roadSystem.gameObject, firstSelectedGameObject, firstPoint, BuildMode.Actual);
            ResetUponRoadModification();
        }
        else if (Input.GetButtonDown("B"))
        {
            DestroyAllPreviewObject();
            ResetRoadBuilding();
        }
    }

    private void DeveloperInteraction()
    {
        if ((int)Input.GetAxis("LeftVertical") != 0)
        {
            vehicleGeneration.carLimit = vehicleGeneration.carLimit + (int)Input.GetAxis("LeftVertical");
            vehicleGeneration.carLimit = Mathf.Clamp(vehicleGeneration.carLimit, 0, 1000);
            developer.GetComponentInChildren<TextMeshProUGUI>().text = vehicleGeneration.carLimit.ToString();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            PathFindingRecalculate();
        }

    }

    public void GetRaycastObjectHit()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(pointer.gameObject.transform.position);
        //Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue);
        if (inputMode == InputMode.Default)
        {
            if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject())
            {
                float radius = 10f;
                Collider[] colliders = Physics.OverlapSphere(hit.point, radius);
                GameObject gameObject;
                string[] vehiclesTag = new string[] { Tag.Vehicle };
                gameObject = GetPointedGameObjectFromListColliders(colliders, hit, vehiclesTag);
                if (gameObject != null)
                {
                    pointedGameObject = gameObject;
                    OutlineGameObject(pointedGameObject.transform);
                    return;
                }
                string[] goalsTag = new string[] { Tag.Goal };
                gameObject = GetPointedGameObjectFromListColliders(colliders, hit, goalsTag);
                if (gameObject != null)
                {
                    pointedGameObject = gameObject;
                    OutlineGameObject(pointedGameObject.transform);
                    return;
                }
                string[] roadsTag = new string[] { Tag.Road_Small, Tag.Road_Large };
                gameObject = GetPointedGameObjectFromListColliders(colliders, hit, roadsTag);
                if (gameObject != null)
                {
                    pointedGameObject = gameObject;
                    OutlineGameObject(pointedGameObject.transform);
                    return;
                }

                if (pointedGameObject == null)
                {
                    pointedGameObject = colliders[0].gameObject;
                    OutlineGameObject(pointedGameObject.transform);
                    return;
                }
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && !EventSystem.current.IsPointerOverGameObject())
            {
                OutlineGameObject(hit.transform);
                pointedGameObject = hit.collider.gameObject;
            }
            else
                pointedGameObject = null;
        }
    }

    private GameObject GetPointedGameObjectFromListColliders(Collider[] colliders, RaycastHit hit, string[] tags)
    {
        List<Collider> selectedColliders = new List<Collider>();
        foreach (Collider collider in colliders)
        {
            if (Tag.CompareTags(collider.transform, tags))
            {
                selectedColliders.Add(collider);
            }
        }

        if (selectedColliders.Count > 0)
        {
            float closestDistance = Mathf.Infinity;
            Collider closestCollider = null;
            for (int i = 0; i < selectedColliders.Count; i++)
            {
                float distance = Vector3.Distance(hit.point, selectedColliders[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = selectedColliders[i];
                }
            }
            return closestCollider.gameObject;
        }
        return null;
    }

    public void OutlineGameObject(Transform transform)
    {
        //if (transform.parent.name == "PreviewRoadSystem")
        //    return;
        // if previously highlight some gameobject, disable it and set to null
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            if (highlight.CompareTag(Tag.Vehicle))
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
                //highlight.gameObject.isStatic = false;
                Outline outline = highlight.gameObject.AddComponent<Outline>();
                outline.OutlineColor = new Color(91, 250, 98);
                outline.OutlineWidth = 2;
                outline.enabled = true;
                //highlight.gameObject.isStatic = true;
            }
            if (highlight.CompareTag(Tag.Vehicle))
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

                if (selection.CompareTag(Tag.Vehicle))
                {
                    VehicleMovement vehicleMovement = selection.GetComponent<VehicleMovement>();
                    vehicleMovement.isSelected = true;
                }

                Debug.Log("Object Selected");
            }
        }

        if (Input.GetButtonDown("B"))
        {
            if (selection != null)
            {
                if (selection.CompareTag(Tag.Vehicle))
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
            if (firstPoint == Vector3.zero && pointedGameObject.transform.parent == roadSystem.gameObject.transform)
            {
                if (Tag.CompareTags(pointedGameObject.transform, Tag.Road_Small, Tag.Road_Large))
                {
                    Debug.Log("first road");
                    firstPoint = pointedPosition;
                    firstSelectedGameObject = pointedGameObject;
                    firstAnchor = roadBuildingManager.CreatePreviewRoad(previewRoadSystem.gameObject, pointedGameObject, firstPoint, BuildMode.Preview);
                    SetUIOnRoadPreview(1);
                    roadBuildingManager.IncreaseCount();
                }
                else if (Tag.CompareTags(pointedGameObject.transform.parent, Tag.Intersection_3_Small, Tag.Intersection_3_Large))
                {
                    Debug.Log("first intersection");
                    firstPoint = pointedPosition;
                    firstSelectedGameObject = pointedGameObject;
                    firstAnchor = roadBuildingManager.CreatePreviewIntersection(previewRoadSystem.gameObject, pointedGameObject.transform.parent.gameObject, firstPoint, BuildMode.Preview);
                    SetUIOnRoadPreview(1);
                    roadBuildingManager.IncreaseCount();
                }
                else
                {
                    Debug.Log("Do nothing");
                }
                roadBuildingManager.RebuildAffectedRoad();
            }
            else if (secondPoint == Vector3.zero && pointedGameObject.transform.parent == roadSystem.gameObject.transform)
            {
                if (pointedGameObject == firstSelectedGameObject)
                {
                    Debug.Log("Cannot select same road or same intersection");
                }
                else if (Tag.CompareTags(pointedGameObject.transform, Tag.Road_Small, Tag.Road_Large))
                {
                    Debug.Log("second road");
                    secondPoint = pointedPosition;
                    secondSelectedGameObject = pointedGameObject;
                    secondAnchor = roadBuildingManager.CreatePreviewRoad(previewRoadSystem.gameObject, pointedGameObject, secondPoint, BuildMode.Preview);
                    SetUIOnRoadPreview(2);
                }
                else if (Tag.CompareTags(pointedGameObject.transform.parent, Tag.Intersection_3_Small, Tag.Intersection_3_Large))
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
                roadBuildingManager.RebuildAffectedRoad();
            }
            // do connection
            if (firstPoint != Vector3.zero && secondPoint != Vector3.zero)
            {
                Debug.Log("Preview Connect");
                roadBuildingManager.ConnectTwoIntersections(previewRoadSystem.gameObject, firstAnchor, secondAnchor, BuildMode.Preview);
                confirm = true;
                roadBuildingManager.IncreaseCount();
                roadBuildingManager.RebuildAffectedRoad();
            }

        }
        else if (Input.GetButtonDown("A") && confirm)
        {
            Debug.Log("implement build");
            StartCoroutine(BuildRoad());
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

    IEnumerator BuildRoad()
    {
        displayManager.ShowLoadingPanel();
        yield return new WaitForSecondsRealtime(0.01f);
        if (Tag.CompareTags(firstSelectedGameObject.transform, Tag.Road_Small, Tag.Road_Large))
            firstAnchor = roadBuildingManager.CreatePreviewRoad(roadSystem.gameObject, firstSelectedGameObject, firstPoint, BuildMode.Actual);
        else
            firstAnchor = roadBuildingManager.CreatePreviewIntersection(roadSystem.gameObject, firstSelectedGameObject.transform.parent.gameObject, firstPoint, BuildMode.Actual);

        if (Tag.CompareTags(secondSelectedGameObject.transform, Tag.Road_Small, Tag.Road_Large))
            secondAnchor = roadBuildingManager.CreatePreviewRoad(roadSystem.gameObject, secondSelectedGameObject, secondPoint, BuildMode.Actual);
        else
            secondAnchor = roadBuildingManager.CreatePreviewIntersection(roadSystem.gameObject, secondSelectedGameObject.transform.parent.gameObject, secondPoint, BuildMode.Actual);

        roadBuildingManager.ConnectTwoIntersections(roadSystem.gameObject, firstAnchor, secondAnchor, BuildMode.Actual);
        ResetUponRoadModification();
        displayManager.HideLoadingPanel();
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
            ChangeToSelectedColor(buttonPanel.transform.Find("Select").gameObject);
            ChangeToNormalColor(buttonPanel.transform.Find("Road").gameObject);
            ChangeToNormalColor(buttonPanel.transform.Find("Remove").gameObject);
        }
        else if (inputMode == InputMode.Build)
        {
            ChangeToNormalColor(buttonPanel.transform.Find("Select").gameObject);
            ChangeToSelectedColor(buttonPanel.transform.Find("Road").gameObject);
            ChangeToNormalColor(buttonPanel.transform.Find("Remove").gameObject);
        }
        else if (inputMode == InputMode.Remove)
        {
            ChangeToNormalColor(buttonPanel.transform.Find("Select").gameObject);
            ChangeToNormalColor(buttonPanel.transform.Find("Road").gameObject);
            ChangeToSelectedColor(  buttonPanel.transform.Find("Remove").gameObject);
        }
        else
        {
            Debug.Log("Wrong InputMode");
        }

        if (inputMode != InputMode.Build || inputMode != InputMode.Remove)
        {
            DestroyAllPreviewObject();
            ResetRoadBuilding();
        }
    }

    public void RemoveInputModeChoice()
    {
        ChangeToNormalColor(buttonPanel.transform.Find("Select").gameObject);
        ChangeToNormalColor(buttonPanel.transform.Find("Road").gameObject);
        ChangeToNormalColor(buttonPanel.transform.Find("Remove").gameObject);
    }

    private void HandleInputModeChange()
    {
        int size = Enum.GetValues(typeof(InputMode)).Length;
        int currentMode = (int)inputMode;
        if (Input.GetButtonDown("LB"))
        {
            currentMode--;
            currentMode = Mathf.Clamp(currentMode, 0, size-1);
            inputMode = (InputMode)currentMode;
            ChangeUIWithInputMode();
        }

        if (Input.GetButtonDown("RB"))
        {
            currentMode++;
            currentMode = Mathf.Clamp(currentMode, 0, size-1);
            inputMode = (InputMode)currentMode;
            ChangeUIWithInputMode();
        }

    }

    private void PathFindingRecalculate()
    {
        float time = 0;
        while (time < 3)
        {
            time += Time.deltaTime;
        }
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag(Tag.Vehicle);
        Debug.Log("Vehicle recalculating path");
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
        roadBuildingManager.ResetCount();
    }

    private void ChangeToNormalColor(GameObject button)
    {
        button.GetComponent<Image>().color = button.GetComponent<Button>().colors.normalColor;
    }

    private void ChangeToSelectedColor(GameObject button)
    {
        button.GetComponent<Image>().color = button.GetComponent<Button>().colors.selectedColor;
    }

    private void ResetUponRoadModification()
    {
        roadSystem.ConstructGraph();
        roadBuildingManager.RebuildAffectedRoad();
        ResetRoadBuilding();
        DestroyAllPreviewObject();
        intersectionManager.GetLatestIntersection();
        PathFindingRecalculate();
        SetUIOnRoadPreview(5);
    }
}
