using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Cursori")]
    public Texture2D cursorDefault;   // cursore nei menu
    public Texture2D cursorShooting;  // cursore durante il gioco (o null = nascosto)
    public Vector2 hotspot = Vector2.zero; // punto di click della texture (es. 0,0 = angolo top-left)

    [Header("Comportamento")]
    public bool hideWhenShooting = true; // se true, nasconde il cursore durante il gioco

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Chiama questo quando entri in un menu
    public void SetMenuCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (cursorDefault != null)
            Cursor.SetCursor(cursorDefault, hotspot, CursorMode.Auto);
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // cursore di sistema
    }

    // Chiama questo quando inizia il gameplay
    public void SetGameplayCursor()
    {
        if (hideWhenShooting)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else if (cursorShooting != null)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.SetCursor(cursorShooting, hotspot, CursorMode.Auto);
        }
    }
}