using UnityEngine;

/// <summary>
/// Asset di dialogo (intro/outro di un livello, monologo di apertura, ecc.).
/// Composto da una lista ordinata di DialogueLine.
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Game/Dialogue Sequence")]
public class DialogueSequence : ScriptableObject
{
    [Tooltip("Chiave dello speaker (vedi LanguageManager.SpeakerTable). Es: \"IA\".")]
    public string speakerKey = "IA";

    [Tooltip("Velocità typewriter di default (caratteri/sec). Le DialogueLine possono override-arla.")]
    public float defaultCharsPerSecond = 35f;

    [Tooltip("Solo per il monologo di apertura: aggiunge una vignetta su tutto lo schermo.")]
    public bool isOpening = false;

    public DialogueLine[] lines;
}
