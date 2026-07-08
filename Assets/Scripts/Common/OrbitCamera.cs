using UnityEngine;

public class AutoOrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 targetOffset = Vector3.zero; // e.g., (0, height, 0)

    [Header("Orbit")]
    public float distance = 600f;            // Radius of the orbit
    public float orbitSpeed = 30f;         // Degrees per second (yaw)
    public float verticalAngle = 20f;      // Pitch angle (fixed tilt)
    public bool clockwise = true;          // Toggle direction

    [Header("Smoothing")]
    public float positionLerp = 10f;       // Higher = snappier; 0 = no smoothing

    private float yaw; // internal yaw accumulator

    void LateUpdate()
    {
        if (target == null) return;

        // Advance yaw over time
        float dir = clockwise ? 1f : -1f;
        yaw += orbitSpeed * dir * Time.deltaTime;

        // Compute desired orbit position around target
        Quaternion orbitRotation = Quaternion.Euler(verticalAngle, yaw, 0f);
        Vector3 desiredPos = target.position + targetOffset + orbitRotation * new Vector3(0f, 0f, -distance);

        // Smooth move (optional)
        if (positionLerp > 0f)
            transform.position = Vector3.Lerp(transform.position, desiredPos, positionLerp * Time.deltaTime);
        else
            transform.position = desiredPos;

        // Always look at the target
        transform.LookAt(target.position + targetOffset, Vector3.up);
    }
}