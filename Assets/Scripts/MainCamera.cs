using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    //zoom
    private readonly float zoomSensitivity = 20;
    private readonly float minZoom = 50;
    private readonly float maxZoom = 180;

    private float currentZoomAmount;

    //pan
    private readonly float panSensitivity = .02f;

    private Vector3 initialCameraPosition;
    private Vector3 initialMousePosition;
    private bool isPanning;

    private void LateUpdate()
    {
        Zoom();

        Pan();
    }

    private void Zoom()
    {
        float fieldOfView = mainCamera.fieldOfView;
        fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        fieldOfView = Mathf.Clamp(fieldOfView, minZoom, maxZoom);
        mainCamera.fieldOfView = fieldOfView;

        //115 is the middle zoom amount. It's a constant, and its value only matters relative to panSensitivity's value
        currentZoomAmount = fieldOfView / 115;
    }

    private void Pan()
    {
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            initialCameraPosition = transform.position;
            initialMousePosition = Input.mousePosition;
            isPanning = true;
        }
        
        if (isPanning)
        {
            Vector3 mouseDelta = initialMousePosition - Input.mousePosition;
            Vector3 cameraDelta = currentZoomAmount * panSensitivity * mouseDelta;
            Vector3 targetCameraPosition = initialCameraPosition + cameraDelta;

            targetCameraPosition.x = Mathf.Clamp(targetCameraPosition.x, -GameManager.GridSize.x, GameManager.GridSize.x);
            targetCameraPosition.y = Mathf.Clamp(targetCameraPosition.y, -GameManager.GridSize.y, GameManager.GridSize.y);
            targetCameraPosition.z = transform.position.z;

            transform.position = targetCameraPosition;
        }

        if (Input.GetMouseButtonUp(1))
            isPanning = false;
    }
}