using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Public variables for inspector configuration
    public float rotationSpeed = 5f;
    public Transform playerTransform;
    public bool ControllerInput;
    public string XAxis;
    public string YAxis;
    // Private variables for internal use
    private Ray ray;
    private float maxRaycastDistance = 5f;
    private UIManagement uiManager;
    
    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    // Update is called once per frame
    private void Update()
    {
        //Do not update when game is paused
        if (Time.timeScale == 0)
            return;
        if (playerTransform != null)
        {
            // Set input axis names based on controller or mouse usage
            if (ControllerInput)
            {
                XAxis = "RightStickHorizontal";
                YAxis = "RightStickVertical";
            }
            else
            {
                XAxis = "Mouse X";
                YAxis = "Mouse Y";
            }
            if (uiManager == null)
                uiManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIManagement>();
            
            CheckRayCollisions();
            // Update camera position to follow the player with a slight offset
            transform.position = playerTransform.position + new Vector3(0, 1, 0);
            RotateCharacter();
            RotateCamera();
        }
    }

    //Updates character rotation based on mouse movement
    //Only rotates on the Y axis
    private void RotateCharacter()
    {
        float mouseX = Input.GetAxis(XAxis);
        transform.Rotate(Vector3.up, mouseX * rotationSpeed);
    }

    //Rotates camera based on mouse movement
    private void RotateCamera()
    {
        float mouseY = Input.GetAxis(YAxis);
        transform.Rotate(Vector3.left, mouseY * rotationSpeed);
        transform.Rotate(Vector3.up, Input.GetAxis(XAxis) * rotationSpeed);
        float currentXRotation = Mathf.Clamp(transform.rotation.eulerAngles.x, -360f, 360f);
        transform.rotation = Quaternion.Euler(currentXRotation, transform.rotation.eulerAngles.y, 0f);
    }

    //This script has a ray cast that goes forward from the camera for a short distance
    //This ray allows the player to interct with nearby objects in the world
    //This method checks if the ray has collided with an interactable object 
    //uiManager is told to display correct pickup message based on the object hit
    private void CheckRayCollisions()
    {
        ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Interactable");
        if (Physics.Raycast(ray, out hit, maxRaycastDistance, layerMask))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            uiManager.DisplayPickupMessage(hit.collider.gameObject);
        }
        else
        {
            uiManager.DisplayPickupMessage(gameObject);
        }
    }
}
