using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class SceneTriggerLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "SpaceShip_01";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Load the specified scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}