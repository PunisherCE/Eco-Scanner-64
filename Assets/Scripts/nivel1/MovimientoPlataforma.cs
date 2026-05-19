using UnityEngine;

public class MovimientoPlataforma : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidad = 7f;
    [SerializeField] private float limiteZ = 12.5f;
    [SerializeField] private GameObject objetoPadre;

    [Tooltip("Si es true, inicia hacia la derecha (+Z). Si es false, hacia la izquierda (-Z).")]
    public bool moviendoHaciaDerecha = true;

    void Update()
    {
        MoverPlataforma();
        VerificarLimites();
    }

    private void MoverPlataforma()
    {
        // Determinamos la dirección basada en el booleano
        float direccion = moviendoHaciaDerecha ? 1f : -1f;

        // Aplicamos el movimiento en el eje Z
        transform.Translate(Vector3.forward * direccion * velocidad * Time.deltaTime);
    }

    private void VerificarLimites()
    {
        // Si va hacia la derecha y supera el límite positivo
        if (moviendoHaciaDerecha && transform.position.z >= objetoPadre.transform.position.z + limiteZ)
        {
            moviendoHaciaDerecha = false; // Cambia dirección a izquierda
            CorregirPosicion(objetoPadre.transform.position.z + limiteZ);
        }
        // Si va hacia la izquierda y supera el límite negativo
        else if (!moviendoHaciaDerecha && transform.position.z <= objetoPadre.transform.position.z - limiteZ)
        {
            moviendoHaciaDerecha = true; // Cambia dirección a derecha
            CorregirPosicion(objetoPadre.transform.position.z - limiteZ);
        }
    }

    private void CorregirPosicion(float limite)
    {
        // Ajustamos la posición exacta para evitar que se pase por los frames
        Vector3 pos = transform.position;
        pos.z = limite;
        transform.position = pos;
    }
}
