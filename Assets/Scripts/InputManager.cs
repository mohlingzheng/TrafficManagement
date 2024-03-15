using Barmetler.RoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
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
    bool YPressed = false, XPressed = false, APressed = false, BPressed = false;
    public RoadSystem roadSystem;

    [Header("Input Mode")]
    public InputMode inputMode;

    [Header("Outline")]
    public GameObject pointedGameObject = null;
    public Transform highlight;
    public Transform selection;

    [Header("Canvas")]
    public GameObject buttonPanel;


    void Start()
    {
        
    }

    void FixedUpdate()
    {
        ButtonInteraction();
        pointedGameObject = ShootRaycast();
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
        YPressed = Input.GetButtonDown("Y");
        if (YPressed)
        {
            Debug.Log("Y Pressed");
            //YButtonInteractButton();
            roadSystem.RebuildAllRoads();
        }
        XPressed = Input.GetButtonDown("X");
        if (XPressed || Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("X Pressed");
            Debug.Log("Keyboard Y clicked");
            roadBuildingManager.BuildRoad();
        }
        APressed = Input.GetButtonDown("A");
        if (APressed)
        {
            Debug.Log("A Pressed");
        }
        BPressed = Input.GetButtonDown("B");
        if (BPressed)
        {
            Debug.Log("B Pressed");
        }

        if (Input.GetButtonDown("LB"))
        {

        }

        HandleInputModeChange();

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


    public GameObject ShootRaycast()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(pointer.gameObject.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity) && !EventSystem.current.IsPointerOverGameObject()){
            OutlineGameObject(hit.transform, hit);
            return hit.collider.gameObject;
        }
        else
            return null;
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

        if (APressed)
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

        if (BPressed)
        {
            if (selection != null)
            {
                selection.gameObject.GetComponent<Outline>().enabled = false;
                selection = null;
            }
        }
    }
}
