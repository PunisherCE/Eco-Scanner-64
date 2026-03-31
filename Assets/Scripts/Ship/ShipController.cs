using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShipController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 20f;
    public ConstantScrollCamera cameraScript; // Reference the script to get its speed
    public Transform cameraTransform;
    public float xBound = 8f;
    public float yBound = 4.5f;

    [Header("Combat Settings")]
    public GameObject bulletPrefab;
    public GameObject missilePrefab;
    public Transform firePoint;
    public int missileAmmo = 10;
    public GameObject[] explosionPrefab = new GameObject[3];

    [Header("Stats")]
    public int maxHealth = 6;
    private int health;
    private int Kills = 0;


    [Header("UI")]
    public UIDocument document;
    private VisualElement healthBar;
    private TextElement missileCount;
    private TextElement killCount;


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

    public void Start()
    {
        health = maxHealth;

        VisualElement root = document.rootVisualElement;
        healthBar = root.Q<VisualElement>("HealthBar");
        missileCount = root.Q<TextElement>("MissileCount");
        killCount = root.Q<TextElement>("KillCount");
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
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Camera cam = cameraTransform.GetComponent<Camera>();

            Ray ray = cam.ScreenPointToRay(mousePosition);

            // Since ship is at Z=90, the plane must be at Z=90.
            // We use Vector3.back if the camera is at Z < 90.
            Vector3 planeNormal = (cam.transform.position.z < transform.position.z) ? Vector3.back : Vector3.forward;
            Plane targetPlane = new Plane(planeNormal, new Vector3(0, 0, 90f));

            if (targetPlane.Raycast(ray, out float distance))
            {
                Vector3 worldTarget = ray.GetPoint(distance);

                // Condition: Target X must be less than Ship X
                if (worldTarget.x < transform.position.x)
                {
                    // Instantiate using the ship's current rotation (which is -90 Y)
                    // or Quaternion.identity; the Missile script will fix it anyway.
                    GameObject missileGo = Instantiate(missilePrefab, firePoint.position, transform.rotation);
                    Missile missileScript = missileGo.GetComponent<Missile>();

                    if (missileScript != null)
                    {
                        missileScript.SetTarget(worldTarget);
                    }

                    missileAmmo--;
                    missileCount.text = "Ammo: " + missileAmmo.ToString();
                }
            }
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

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object we hit has the "Enemy" tag
        if (other.CompareTag("Enemy"))
        {
            // 1. (Optional) Instantiate an explosion effect here
            // Instantiate(explosionPrefab, transform.position, transform.rotation);
            int explosionIndex = Random.Range(0, explosionPrefab.Length);
            Instantiate(explosionPrefab[explosionIndex], transform.position, transform.rotation);
            explosionIndex = Random.Range(0, explosionPrefab.Length);
            Instantiate(explosionPrefab[explosionIndex], other.transform.position, transform.rotation);
            EnemySpawner.PlayerDies();
            // 2. Destroy this ship
            Debug.Log("Ship crashed into " + other.name);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Health: " + health);
        float healthPercentage = (float)this.health / (float)maxHealth;
        healthPercentage *= 100;
        healthBar.style.width = new Length(healthPercentage, LengthUnit.Percent);

        if (health <= 0) Die();
    }

    private void Die()
    {
        healthBar.style.width = new Length(0, LengthUnit.Percent);
        int explosionIndex = Random.Range(0, explosionPrefab.Length);
        Instantiate(explosionPrefab[explosionIndex], transform.position, transform.rotation);
        EnemySpawner.PlayerDies();
        Destroy(gameObject);
    }

    public void KillIncrement()
    {
        Kills++;
        killCount.text = "Kills: " + Kills.ToString();
    }
}