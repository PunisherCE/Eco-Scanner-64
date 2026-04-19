using UnityEngine;

public class ConstantScrollCamera : MonoBehaviour
{
    public float scrollSpeed = 5f;
    public bool isDead = false;

    public static ConstantScrollCamera Instance { get; private set; }

    void Awake()
    {
        // 2. Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Maintain "Single" status
            return;
        }

        // 3. Set the instance to this object
        Instance = this;
        
        // Optional: Keep this alive across different scenes
        // DontDestroyOnLoad(gameObject); 
    }

    void Update()
    {
        if (isDead) return;

        // Moves the camera constantly in the negative X direction
        transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);
    }
}
