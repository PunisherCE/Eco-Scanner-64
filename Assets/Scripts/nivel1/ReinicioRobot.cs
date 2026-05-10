using UnityEngine;

public class ReinicioRobot : MonoBehaviour
{

    [SerializeField] private GameObject reaparecerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < 130) 
        {
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false; // Desactivar temporalmente

            transform.position = reaparecerTransform.transform.position;

            if (cc != null) cc.enabled = true; // Reactivar
            Debug.Log("Robot reiniciado.");
        }
            
        
    }
}
