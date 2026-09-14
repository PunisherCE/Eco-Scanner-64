using UnityEngine;
using UnityEngine.UIElements;

public class Tutorial : MonoBehaviour
{
    Label info;
    Button start;

    public GameObject pause;

    public string tutorialText;

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        info = root.Q<Label>("Info");
        start = root.Q<Button>("Start");

        info.text = tutorialText;
        Time.timeScale = 0f;

        if (start == null)
        {
            Debug.Log("Start not found");

        } else Debug.Log("Start found");

        start.RegisterCallback<ClickEvent>(ev =>
        {
            Time.timeScale = 1f;
            pause.SetActive(true);
            this.gameObject.SetActive(false);
        });
        
    }
}
