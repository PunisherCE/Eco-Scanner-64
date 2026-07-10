using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    [Tooltip("The GameObjects representing the portals or triggers for each level. The order must match levelStatNames.")]
    [SerializeField]
    private GameObject[] levelPortals = new GameObject[4];
    [SerializeField] private GameObject creditsUI;

    private int levelsCompleted = 0;

    // These are the names of the stats that represent level completion.
    // The order must correspond to the levelPortals array.
    // 0: EntrarMundo1, 1: SpaceShip_01, 2: Tetris, 3: Temple_of_Dead
    private readonly string[] levelStatNames = {
        "EntrarMundo1",
        "SpaceShip_01",
        "Tetris",
        "Temple_of_Dead"
    };

    void Start()
    {
        // Load the last used profile to check their progress.
        int currentProfile = StatsManager.LoadLastPlayer();

        // Load all stats for that profile.
        StatCollection stats = StatsManager.LoadStats(currentProfile);

        // Iterate through the defined level stats.
        for (int i = 0; i < levelStatNames.Length; i++)
        {
            // Check if a stat entry with the current level name exists.
            bool levelCompleted = stats.stats.Any(stat => stat.name == levelStatNames[i]);

            // If the level is completed, disable its corresponding portal GameObject.
            if (levelCompleted && i < levelPortals.Length && levelPortals[i] != null)
            {
                levelsCompleted++;
                levelPortals[i].SetActive(false);
            }

            if (levelsCompleted == 4)
            {
                // If all levels are completed, activate the credits UI.
                if (creditsUI != null)
                {
                    creditsUI.SetActive(true);
                    foreach (GameObject portal in levelPortals)
                    {
                        if (portal != null)
                        {
                            portal.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                // If not all levels are completed, ensure the credits UI is inactive.
                if (creditsUI != null)
                {
                    creditsUI.SetActive(false);
                }
            }
        }
    }
}
