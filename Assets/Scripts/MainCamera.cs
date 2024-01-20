using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    // Zoom
    private readonly float zoomZSensitivity = .75f;
    public float zoomXYSensitivity = .8f;
    private readonly float minZoom = -500;
    private readonly float maxZoom = -3;

    private float scrollInput;

    // Pan
    private readonly float panSensitivity = .0019f;

    private Vector3 initialCameraPosition;
    private Vector3 initialMousePosition;
    private bool isPanning;

    private void Update()
    {
        scrollInput = Input.GetAxis("Mouse ScrollWheel");
    }

    private void LateUpdate()
    {
        Zoom();

        Pan();
    }

    private void Zoom()
    {
        if (scrollInput == 0)
            return;

        float cameraDistance = transform.position.z;

        // Increase zoom speed based on current distance
        cameraDistance -= cameraDistance * scrollInput * zoomZSensitivity;

        cameraDistance = Mathf.Clamp(cameraDistance, minZoom, maxZoom);

        Vector3 newCameraPositionXY;
        if (scrollInput < 0) // If zooming out
            newCameraPositionXY = transform.position;
        else
        {
            // Get mouse position
            Vector3 inputMousePos = Input.mousePosition;
            inputMousePos.z = -transform.position.z;
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(inputMousePos);

            Vector2 cameraPositionXY = (Vector2)transform.position;

            Vector2 cameraOffset = scrollInput * zoomXYSensitivity * (mousePosition - cameraPositionXY);

            newCameraPositionXY = transform.position + (Vector3)cameraOffset;
        }


        transform.position = new Vector3(newCameraPositionXY.x, newCameraPositionXY.y, cameraDistance);
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

            // Increase pan speed when camera is farther from grid
            Vector3 cameraDelta = -transform.position.z * panSensitivity * mouseDelta;

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