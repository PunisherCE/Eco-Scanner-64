using UnityEngine;

public class LuzFiesta : MonoBehaviour
{
    public float velocidad = 2f;
    public float gradosMovimiento = 60f; // Cuántos grados girará a cada lado

    private float anguloInicialY;

    void Start()
    {
        // Guardamos la rotación Y que pusiste manualmente en el Inspector
        anguloInicialY = transform.localRotation.eulerAngles.y;
    }

    void Update()
    {
        // Calculamos la oscilación
        // Mathf.Sin crea un movimiento más fluido y natural que PingPong
        float oscilacion = Mathf.Sin(Time.time * velocidad) * gradosMovimiento;

        // Aplicamos el giro sobre el eje Y original, manteniendo X y Z como están
        transform.localRotation = Quaternion.Euler(
            transform.localRotation.eulerAngles.x,
            anguloInicialY + oscilacion,
            transform.localRotation.eulerAngles.z
        );
    }


}
