using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class ConclusionCamera : MonoBehaviour, IScrollHandler
{
    Vector3 DefaultPosition = new Vector3(0, 1, -10);

    [Header("Transform")]
    public Transform cameraTransform;

    [Header("Constant Speed")]
    public float movementSpeed = 5;
    public float movementTime = 5;
    public float rotationAmount = 3;
    public Vector3 zoomAmount = new Vector3(0, -20, 20);

    [Header("Value Changed")]
    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    [Header("Screen Size")]
    float length = 1500f;
    float width = 711f;
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }
    void Update()
    {
        HandleMovementInput();
        UpdatePositionRotation();
    }

    void HandleMovementInput()
    {
        newPosition += transform.right * Input.GetAxis("Horizontal") * movementSpeed;
        newPosition += transform.up * Input.GetAxis("Vertical") * movementSpeed;
        newZoom += -Input.GetAxis("RightVertical") * zoomAmount;

        if (Input.GetButtonDown("RS_B"))
        {
            newZoom = DefaultPosition;
        }
    }

    Vector3 ClampZoomVector(Vector3 value)
    {
        value.x = 0;
        value.y = 0;
        value.z = Mathf.Clamp(value.z, -10f, 325f);
        return value;
    }

    Vector3 ClampPositionVector(Vector3 value)
    {
        value.x = Mathf.Clamp(value.x, 0f, length);
        value.y = Mathf.Clamp(value.y, 5f, 5f);
        value.z = Mathf.Clamp(value.z, 0f, width);
        return value;
    }

    void UpdatePositionRotation()
    {
        //newPosition = ClampPositionVector(newPosition);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        newZoom = ClampZoomVector(newZoom);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }

    public void OnScroll(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
