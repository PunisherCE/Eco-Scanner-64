using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Settings")]
    public float sensitivity = 0.1f;
    public float minVerticalAngle = -85f; 
    public float maxVerticalAngle = 85f;
    public float distance = 4.0f;
    public Vector3 pivotOffset = new Vector3(0, 1f, 0);

    [Header("References")]
    public Transform playerBody;     // The Robot
    public Transform cameraTransform; // The Main Camera (Child)

    private float verticalRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        // If you didn't assign it, try to find it in children
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();

        // 1. Horizontal: Rotate the whole Robot
        playerBody.Rotate(Vector3.up * lookInput.x * sensitivity);

        // 2. Vertical: Rotate the Pivot (this object)
        verticalRotation -= lookInput.y * sensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
    }

    // LateUpdate is best for cameras to prevent jitter
    void LateUpdate()
    {
        // 3. Orbit the camera vertically around the player
        Quaternion rotation = Quaternion.Euler(verticalRotation, playerBody.eulerAngles.y, 0f);
        Vector3 targetPos = playerBody.position + pivotOffset;
        
        cameraTransform.position = targetPos - (rotation * Vector3.forward * distance);
        cameraTransform.LookAt(targetPos);
    }
}