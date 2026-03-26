using UnityEngine;

public class ConstantScrollCamera : MonoBehaviour
{
    public float scrollSpeed = 5f;

    void Update()
    {
        // Moves the camera constantly in the negative X direction
        transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);
    }
}
