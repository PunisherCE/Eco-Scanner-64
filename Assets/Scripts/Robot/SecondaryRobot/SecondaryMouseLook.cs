using UnityEngine;

public class SecondaryMouseLook : MonoBehaviour
{
    [Header("Settings")]
    public float distance = 6f;          // How far behind the player the camera stays along Z
    public float height = 2f;            // Height offset
    public float smoothTime = 0.1f;      // Smooth damp time
    public Vector3 lookAtOffset = new Vector3(0, 1.5f, 0); // Offset to align fixed look angle

    [Header("References")]
    public Transform playerBody;         // The robot
    public Transform cameraTransform;    // The main camera

    private Vector3 _cameraVelocity = Vector3.zero;
    private Quaternion _fixedRotation;
    private float _fixedX;               // The locked X axis position
    private float _fixedY;               // The locked Y axis position

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;

        if (playerBody != null)
        {
            SecondaryRobotController robotController = playerBody.GetComponent<SecondaryRobotController>();
            if (robotController != null)
            {
                int invert = robotController.invertCamera ? -1 : 1;
                distance *= invert;
            }

            // Lock the X axis coordinate to the player's starting X position
            _fixedX = playerBody.position.x;
            _fixedY = playerBody.position.y + height;

            // Calculate initial rail position and lock in camera rotation once at start
            Vector3 targetLookAt = playerBody.position + lookAtOffset;
            Vector3 initialPos = new Vector3(_fixedX, playerBody.position.y + height, playerBody.position.z - distance);
            
            _fixedRotation = Quaternion.LookRotation(targetLookAt - initialPos);
            cameraTransform.rotation = _fixedRotation;
        }
    }

    void LateUpdate()
    {
        if (playerBody == null) return;

        // Smooth only along Z; X and Y stay fixed after initialization.
        float desiredZ = playerBody.position.z - distance;
        float smoothedZ = Mathf.SmoothDamp(
            cameraTransform.position.z,
            desiredZ,
            ref _cameraVelocity.z,
            smoothTime
        );
        cameraTransform.position = new Vector3(_fixedX, _fixedY, smoothedZ);

        // 3. Keep camera orientation completely fixed in world space
        cameraTransform.rotation = _fixedRotation;
    }
}