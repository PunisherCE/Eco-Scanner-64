using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public delegate void HealthChanged(float normalizedHealth);
    public HealthChanged onHealthChanged;
    VisualElement healthBar;

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        healthBar = root.Q<VisualElement>("HealthBar");
    }
    void Start()
    {
        onHealthChanged += UpdateHealthBar;
    }

    void Update()
    {
    }

    void UpdateHealthBar(float normalizedHealth)
    {
        Debug.Log("Health updated to: " + normalizedHealth);
        healthBar.style.width = new StyleLength(new Length(normalizedHealth * 100, LengthUnit.Percent));
    }
}
