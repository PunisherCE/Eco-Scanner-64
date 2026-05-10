using UnityEngine;

public class LuzFiesta : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float velocidad = 2f;
    public float anguloMax = 80f;

    void Update()
    {
        float angulo = Mathf.PingPong(Time.time * velocidad, anguloMax);
        transform.rotation = Quaternion.Euler(0, angulo, 0);
    }



}
