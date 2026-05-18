using UnityEngine;

public class LuzParpadeo : MonoBehaviour
{
    public float intervalo = 0.5f;
    public float tiempoParaDesactivar = 9f;
    private Light luz;

    void Start()
    {
        luz = GetComponent<Light>();

        if (luz == null)
        {
            Debug.LogError("No se encontró una luz en este objeto");
        }

        InvokeRepeating("AlternarLuz", 0f, intervalo);
        Invoke("ApagarDefinitivamente", tiempoParaDesactivar);
    }

    void AlternarLuz()
    {
        if (luz != null)
            luz.enabled = !luz.enabled;
    }
    void ApagarDefinitivamente()
    {
        // 1. Detenemos el parpadeo (el InvokeRepeating)
        CancelInvoke("AlternarLuz");

        // 2. Nos aseguramos de que la luz quede apagada
        if (luz != null)
        {
            luz.enabled = false;
        }

        Debug.Log("La luz se ha desactivado tras " + tiempoParaDesactivar + " segundos.");
    }


}
