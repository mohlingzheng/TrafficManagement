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
    private Vector2 leftMovementVector = Vector2.zero;
    private Vector2 rightMovementVector = Vector2.zero;
    public Vector2 CameraMovementVector { get => leftMovementVector; }
    [SerializeField]
    Camera mainCamera;
    public CameraMovement cameraMovement;
    public Button pauseButton;
    public Button resumeButton;
    public Image pointer;
    bool Pressed = false;


    void Start()
    {
        
    }

    void Update()
    {
        //CheckArrowInput();
        //cameraMovement.MoveCamera(new Vector3(leftMovementVector.x, 0, leftMovementVector.y));
        //cameraMovement.RotateCamera(rightMovementVector);
        Pressed = Input.GetButtonDown("Y");
        if (Pressed)
        {
            interactButton();
        }
        ShootRaycast();
    }

    private void CheckClickHoldEvent()
    {
        if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {

            OnMouseClick?.Invoke(mainCamera.ScreenPointToRay(Input.mousePosition));
        }
    }

    private void CheckClickUpEvent()
    {
        if (Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            OnMouseUp?.Invoke();
        }
    }

    private void CheckClickDownEvent()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            OnMouseClick?.Invoke(mainCamera.ScreenPointToRay(Input.mousePosition));
        }
    }

    private void CheckEscClick()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscape.Invoke();
        }
    }

    private void CheckArrowInput()
    {
        leftMovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rightMovementVector = new Vector2(Input.GetAxis("RightHorizontal"), Input.GetAxis("RightVertical"));
    }

    public void ClearEvents()
    {
        OnMouseClick = null;
        OnMouseHold = null;
        OnEscape = null;
        OnMouseUp = null;
    }

    public void interactButton()
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

    public GameObject ShootRaycast()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(pointer.gameObject.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.blue);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
            Debug.Log("hit " + hit.collider.gameObject.name);
            return hit.collider.gameObject;
        }
        else
            return null;
    }
}
