using UnityEngine;

public class LuzParpadeo : MonoBehaviour
{
    public float intervalo = 1f;
    private Light luz;

    void Start()
    {
        luz = GetComponent<Light>();

        if (luz == null)
        {
            Debug.LogError("No se encontró una luz en este objeto");
        }

        InvokeRepeating("AlternarLuz", 0f, intervalo);
    }

    void AlternarLuz()
    {
        if (luz != null)
            luz.enabled = !luz.enabled;
    }
}
