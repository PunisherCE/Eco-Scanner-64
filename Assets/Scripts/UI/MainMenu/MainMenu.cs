using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement StartGame = root.Q<VisualElement>("StartGame");
        StartGame.RegisterCallback<ClickEvent>(ev => {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Tetris");
        });
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
