using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public ConstantScrollCamera cameraScript; // Reference the script to get its speed
    public Transform cameraTransform;
    public float xBound = 8f; 
    public float yBound = 4.5f;

    [Header("Combat Settings")]
    public GameObject bulletPrefab;
    public GameObject missilePrefab;
    public Transform firePoint;
    public int missileAmmo = 10;

    private Vector2 moveInput;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        
        // Safety: If you forgot to assign cameraTransform, use Main Camera
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        if (cameraScript == null) cameraScript = cameraTransform.GetComponent<ConstantScrollCamera>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public void OnMissile(InputAction.CallbackContext context)
    {
        if (context.started && missileAmmo > 0)
        {
            Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
            missileAmmo--;
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // 1. Start with current position
        Vector3 currentPos = transform.position;

        // 2. Add Camera's constant movement (Left is Vector3.left)
        // This ensures the ship "drifts" with the camera automatically
        float cameraStep = cameraScript.scrollSpeed * Time.deltaTime;
        currentPos += Vector3.left * cameraStep;

        // 3. Add Player Input movement
        Vector3 playerMove = new Vector3(-moveInput.x * 0.75f, moveInput.y * 0.5f, 0) * moveSpeed * Time.deltaTime;
        Vector3 newPos = currentPos + playerMove;

        // 4. Clamp relative to Camera Center
        float minX = cameraTransform.position.x - xBound;
        float maxX = cameraTransform.position.x + xBound;
        float minY = cameraTransform.position.y - yBound;
        float maxY = cameraTransform.position.y + yBound;

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        transform.position = newPos;
    }
}