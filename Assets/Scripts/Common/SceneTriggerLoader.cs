using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class SceneTriggerLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "SpaceShip_01";
    [SerializeField] private string thisSceneName = "SceneMain"; // Name of the current scene

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            if (sceneToLoad == "SceneMain")
            {
                // Get the current player profile index
                int currentProfile = StatsManager.LoadLastPlayer();

                // Save a stat to mark that this scene has been entered.
                // The stat name is the scene name, and the value is 100 as requested.
                StatsManager.SaveStat(currentProfile, thisSceneName, 100);
            }

            // Load the specified scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}