using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class CameraController : MonoBehaviour
{
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
        newPosition += transform.forward * Input.GetAxis("Vertical") * movementSpeed;
        newRotation *= Quaternion.Euler(new Vector3(0f, Input.GetAxis("RightHorizontal"), 0f) * rotationAmount);
        newZoom += -Input.GetAxis("RightVertical") * zoomAmount;

        if (Input.GetButtonDown("RS_B"))
        {
            newRotation = Quaternion.Euler(0, 0, 0);
            newZoom = new Vector3(0, 400, -400);
        }
    }

    Vector3 ClampZoomVector(Vector3 value)
    {
        value.x = Mathf.Clamp(value.x, 0f, value.x);
        value.y = Mathf.Clamp(value.y, 100f, 1300f);
        value.z = Mathf.Clamp(value.z, -1300f, -100f);
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
        newPosition = ClampPositionVector(newPosition);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        newZoom = ClampZoomVector(newZoom);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
