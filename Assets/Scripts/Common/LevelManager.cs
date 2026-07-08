using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    [Tooltip("The GameObjects representing the portals or triggers for each level. The order must match levelStatNames.")]
    [SerializeField]
    private GameObject[] levelPortals = new GameObject[4];

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
                levelPortals[i].SetActive(false);
            }
        }
    }
}
