using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D crosshairTexture;
    public Vector2 hotspot = new Vector2(32, 32); // Half of your texture size

    void Start()
    {
        // Change the cursor to the crosshair
        Cursor.SetCursor(crosshairTexture, hotspot, CursorMode.Auto);
    }

    void OnDisable()
    {
        // Reset to default system cursor when the script is disabled
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}