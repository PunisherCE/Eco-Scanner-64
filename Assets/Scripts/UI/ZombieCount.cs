using UnityEngine;
using UnityEngine.UIElements;

public class ZombieCount : MonoBehaviour
{
    static Label Count;

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        Count = root.Q<Label>("Count");
    }

    public static void UpdateUI()
    {
        Count.text = "Killed: " + ZombieSpawner.TotalZombiesKilled.ToString();
    }
}
