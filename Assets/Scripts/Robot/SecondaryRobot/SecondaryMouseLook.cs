using UnityEngine;

public class SecondaryMouseLook : MonoBehaviour
{
    [Header("Settings")]
    public float distance = 6f;          // How far behind the player the camera stays
    public float height = 2f;            // Height offset
    public float smoothSpeed = 8f;       // Smooth follow

    [Header("References")]
    public Transform playerBody;         // The robot
    public Transform cameraTransform;    // The main camera

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

        // Desired position: behind the player, using a fixed world direction
        Vector3 desiredPos =
            playerBody.position
            - Vector3.forward * distance // Use a fixed world direction instead of player's forward
            + Vector3.up * height;

        // Smooth follow
        cameraTransform.position =
            Vector3.Lerp(cameraTransform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // Always look at the player
        cameraTransform.LookAt(playerBody.position + Vector3.up * 1.5f);
    }
}
