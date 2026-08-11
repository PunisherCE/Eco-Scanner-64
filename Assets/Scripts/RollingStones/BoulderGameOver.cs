using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BoulderGameOver : MonoBehaviour
{
    public VisualElement mainContainer;
    SecondaryRobotController robot;

    void OnEnable()
    {
        // Get the VisualElement from the UIDocument
        var ui = GetComponent<UIDocument>();
        mainContainer = ui.rootVisualElement.Q<VisualElement>("MainContainer");

        // Start with alpha = 0
        Color startColor = mainContainer.style.backgroundColor.value;
        startColor.a = 0f;
        mainContainer.style.backgroundColor = startColor;

        // Start fade
        robot = FindFirstObjectByType<SecondaryRobotController>();
        StartCoroutine(FadeInBackground(2f));
    }

    void Start()
    {
    }

    IEnumerator FadeInBackground(float duration)
    {
        robot.isDead = true; // Set the robot's isDead flag to true
        robot.PlayDeathAnimation(); // Play the death animation
        Color start = mainContainer.style.backgroundColor.value;
        Color end = start;
        end.a = 1f; // full alpha

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            Color current = Color.Lerp(start, end, lerp);
            mainContainer.style.backgroundColor = current;

            yield return null;
        }

        // Ensure final value is exact
        mainContainer.style.backgroundColor = end;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
