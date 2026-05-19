using UnityEngine;

public class PlayerPlatformRider : MonoBehaviour
{
    private CharacterController characterController;
    private MovimientoPlataforma currentPlatform;
    
    [SerializeField] private float platformSlideSpeed = 2.5f;

    void Start()
    {
        // Cache the player's Character Controller component
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Only apply movement if the player is currently on a platform
        if (currentPlatform != null)
        {
            // Determine direction based on the platform's boolean
            Vector3 moveDirection = currentPlatform.moviendoHaciaDerecha ? Vector3.forward : Vector3.back;
            
            // Move the player smoothly independent of frame rate
            characterController.Move(moveDirection * platformSlideSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object we stepped on is a moving platform
        if (other.CompareTag("MovingPlatform"))
        {
            // Grab the script component from the platform
            currentPlatform = other.GetComponent<MovimientoPlataforma>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear the reference when we jump or walk off the platform
        if (other.CompareTag("MovingPlatform"))
        {
            currentPlatform = null;
        }
    }
}