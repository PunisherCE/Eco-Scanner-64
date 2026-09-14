using UnityEngine;
using UnityEngine.UIElements;

public class EndGameCredits : MonoBehaviour
{

    Button backButton;

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        backButton = root.Q<Button>("Back");

        backButton.RegisterCallback<ClickEvent>(ev =>
        {
            this.gameObject.SetActive(false);
        });
    }
}
