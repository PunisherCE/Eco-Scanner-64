using UnityEngine;

public class SecondaryMouseLook : MonoBehaviour
{
    [Header("Settings")]
    public float distance = 6f;          // How far behind the player the camera stays
    public float height = 2f;            // Height offset
    public float smoothTime = 0.1f;      // Time it takes for the camera to reach the target. Lower is faster.
    public Vector3 lookAtOffset = new Vector3(0, 1.5f, 0); // The point above the player's pivot to look at.

    [Header("References")]
    public Transform playerBody;         // The robot
    public Transform cameraTransform;    // The main camera

    // Private variable to store the camera's velocity for SmoothDamp
    private Vector3 _cameraVelocity = Vector3.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;

        if (playerBody != null)
        {
            int invert = playerBody.GetComponent<SecondaryRobotController>().invertForward;  // Get the invert setting from the robot controller
            distance = distance * invert;  // Apply inversion to the camera distance
        }
    }

    void LateUpdate()
    {
        if (playerBody == null) return;

        // 1. Calculate the desired camera position using a fixed world direction.
        // This keeps the camera from rotating with the player.
        Vector3 desiredPos =
            playerBody.position
            - Vector3.forward * distance // Use a fixed world direction instead of player's forward
            + Vector3.up * height;

        // 2. Smoothly move the camera towards the desired position.
        // Vector3.SmoothDamp is ideal for this as it provides a much smoother follow and avoids jitter.
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, desiredPos, ref _cameraVelocity, smoothTime);

        // 3. Always look at the player's look-at target.
        cameraTransform.LookAt(playerBody.position + lookAtOffset);
    }
}
