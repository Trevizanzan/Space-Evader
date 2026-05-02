using UnityEngine;

/// <summary>
/// Singola "card" del dialogo.
/// Il testo è duplicato in IT/EN; LanguageManager sceglie quale leggere a runtime.
/// </summary>
[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string italian;

    [TextArea(2, 5)]
    public string english;

    [Tooltip("Pausa extra DOPO la riga, in secondi. Per le righe '...' usa 1.5–2.0.")]
    public float pauseAfter = 0.4f;

    [Tooltip("Velocità typewriter (caratteri al secondo). 0 = usa il default della sequence.")]
    public float charsPerSecond = 0f;
}
