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

        //115 is the middle zoom amount
        //current zoom amount ranges from -.5 to 1.5
        currentZoomAmount = (fieldOfView / 115) - 1;
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
            //increase pan speed exponentially based on zoom amount, with a growth rate of 50
            Vector3 cameraDelta = panSensitivity * Mathf.Pow(50, currentZoomAmount) * mouseDelta;

            Vector3 targetCameraPosition = initialCameraPosition + cameraDelta;

            targetCameraPosition.x = Mathf.Clamp(targetCameraPosition.x, -1000, 1000);
            targetCameraPosition.y = Mathf.Clamp(targetCameraPosition.y, -1000, 1000);
            targetCameraPosition.z = transform.position.z;

            transform.position = targetCameraPosition;
        }

        if (Input.GetMouseButtonUp(1))
            isPanning = false;
    }
}