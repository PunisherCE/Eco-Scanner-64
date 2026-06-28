using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public InputAction pauseAction;
    public PlayerInput playerInput;

    private UIDocument uiDocument;
    private VisualElement root;

    private Button btnResume;
    private Button btnExit;
    private Button btnQuit;

    private bool isPaused = false;

    private VisualElement mainContainer;

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        mainContainer = root.Q<VisualElement>("MainContainer");

        btnResume = root.Q<Button>("Resume");
        btnExit = root.Q<Button>("Exit");
        btnQuit = root.Q<Button>("Quit");

        btnResume.RegisterCallback<ClickEvent>(ev => ResumeGame());
        btnExit.RegisterCallback<ClickEvent>(ev =>
        {
            ResumeGame(); // <- vuelve a Player y desbloquea cursor
            SceneManager.LoadScene("SceneMain");
        });

        btnQuit.RegisterCallback<ClickEvent>(ev => QuitGame());

        mainContainer.style.display = DisplayStyle.None;

        pauseAction.Enable();
        pauseAction.performed += ctx =>
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        };
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        mainContainer.style.display = DisplayStyle.Flex;

        playerInput.SwitchCurrentActionMap("UI");

        // Unlock and show the cursor for UI interaction
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        mainContainer.style.display = DisplayStyle.None;

        playerInput.SwitchCurrentActionMap("Player");

        // Lock and hide the cursor for gameplay
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }


    void OnDisable()
    {
        pauseAction.Disable();
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
