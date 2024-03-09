using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputSystem : MonoBehaviour
{
    public event Action<Ray> OnMouseClick, OnMouseHold;
    public event Action OnMouseUp, OnEscape;

    public Camera mainCamera;
    public Button pauseButton;
    public Button resumeButton;
    public Image pointer;
    public RoadBuildingManager roadBuildingManager;
    bool YPressed = false, XPressed = false;

    // Outline Related
    private Transform highlight;
    private Transform selection;


    void Start()
    {
        
    }

    void Update()
    {
        ButtonInteraction();
        ShootRaycast();
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
        YPressed = Input.GetButtonDown("Y");
        if (YPressed)
        {
            YButtonInteractButton();
        }
        XPressed = Input.GetButtonDown("X");
        if (XPressed || Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("X Pressed");
            roadBuildingManager.BuildRoad();
        }


    }

    public GameObject ShootRaycast()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(pointer.gameObject.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.blue);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity) && !EventSystem.current.IsPointerOverGameObject()){
            //Debug.Log("hit " + hit.collider.gameObject.name);
            OutlineGameObject(hit.transform, hit);
            return hit.collider.gameObject;
        }
        else
            return null;
    }

    public void OutlineGameObject(Transform transform, RaycastHit raycastHit)
    {
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

        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (highlight)
        //    {
        //        if (selection != null)
        //        {
        //            selection.gameObject.GetComponent<Outline>().enabled = false;
        //        }
        //        selection = raycastHit.transform;
        //        selection.gameObject.GetComponent<Outline>().enabled = true;
        //        highlight = null;
        //    }
        //    else
        //    {
        //        if (selection)
        //        {
        //            selection.gameObject.GetComponent<Outline>().enabled = false;
        //            selection = null;
        //        }
        //    }
        //}
    }
}
