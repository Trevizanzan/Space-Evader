using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Overlay di dialogo "VN-style" ancorato in basso.
/// Riproduce una DialogueSequence in typewriter, con tap per avanzare riga
/// e hold del tasto Pausa (Esc/Start) per skippare.
///
/// Tutto il timing è su Time.unscaledDeltaTime: durante il dialogo
/// Time.timeScale = 0 (gameplay congelato).
///
/// Singleton scena: una sola istanza in GameScene, riferita da
/// DifficultyManager / BossBase via Instance.
/// </summary>
public class DialogueOverlay : MonoBehaviour
{
    public static DialogueOverlay Instance { get; private set; }

    [Header("Refs")]
    [Tooltip("GameObject contenitore di tutti i visual del pannello (toggle on/off).")]
    [SerializeField] private GameObject panelRoot;
    [Tooltip("Vignetta full-screen mostrata solo quando DialogueSequence.isOpening = true.")]
    [SerializeField] private GameObject openingVignette;
    [SerializeField] private TMP_Text speakerLabel;
    [SerializeField] private TMP_Text bodyText;

    [Header("Debug")]
    [Tooltip("Disabilita tutti i dialoghi. Solo per debug — non usare in produzione.")]
    [SerializeField] public bool skipAllDialogue = false;

    [Header("Skip Hold")]
    [Tooltip("Secondi di hold del tasto Pausa (Esc/Start) per skippare l'intera sequenza.")]
    [SerializeField] private float skipHoldThreshold = 0.6f;

    [Header("Typewriter Defaults")]
    [Tooltip("Caratteri/sec usati se il sequence asset ha defaultCharsPerSecond <= 0.")]
    [SerializeField] private float fallbackCharsPerSecond = 35f;

    public bool IsActive { get; private set; }

    private SpaceEvaderInputActions input;
    private float skipHoldTime;
    private bool skipRequested;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        input = new SpaceEvaderInputActions();

        if (panelRoot != null) panelRoot.SetActive(false);
        if (openingVignette != null) openingVignette.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        input?.Disable();
        input?.Dispose();
    }

    /// <summary>
    /// Riproduce la sequenza. Bloccante: chiamare con yield return.
    /// </summary>
    public IEnumerator Play(DialogueSequence seq)
    {
        if (skipAllDialogue) yield break;
        if (seq == null || seq.lines == null || seq.lines.Length == 0)
            yield break;

        IsActive = true;
        skipHoldTime = 0f;
        skipRequested = false;

        // Aspetta un frame prima di abilitare l'input: evita che un click/tap
        // già in corso (es. focus della finestra nell'Editor) skippi la prima riga.
        yield return null;

        input.Player.Enable();

        if (panelRoot != null) panelRoot.SetActive(true);
        if (openingVignette != null) openingVignette.SetActive(seq.isOpening);

        if (speakerLabel != null)
        {
            speakerLabel.text = LanguageManager.Instance != null
                ? LanguageManager.Instance.GetSpeaker(seq.speakerKey)
                : seq.speakerKey;
        }

        float prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        for (int i = 0; i < seq.lines.Length; i++)
        {
            if (skipRequested) break;

            DialogueLine line = seq.lines[i];

            string fullText = LanguageManager.Instance != null
                ? LanguageManager.Instance.Get(line)
                : line.italian;
            if (fullText == null) fullText = string.Empty;

            float cps = line.charsPerSecond > 0f
                ? line.charsPerSecond
                : (seq.defaultCharsPerSecond > 0f ? seq.defaultCharsPerSecond : fallbackCharsPerSecond);

            yield return RunLineRoutine(fullText, cps);
            if (skipRequested) break;

            if (line.pauseAfter > 0f)
            {
                float t = 0f;
                while (t < line.pauseAfter)
                {
                    if (TickSkipHold()) { skipRequested = true; break; }
                    if (input.Player.Fire.WasPressedThisFrame()) break;
                    t += Time.unscaledDeltaTime;
                    yield return null;
                }
                if (skipRequested) break;
            }
        }

        if (panelRoot != null) panelRoot.SetActive(false);
        if (openingVignette != null) openingVignette.SetActive(false);

        Time.timeScale = prevTimeScale == 0f ? 1f : prevTimeScale;
        input.Player.Disable();
        IsActive = false;
    }

    private IEnumerator RunLineRoutine(string fullText, float cps)
    {
        if (bodyText == null) yield break;

        bodyText.text = fullText;
        bodyText.ForceMeshUpdate();
        int totalChars = bodyText.textInfo.characterCount;
        bodyText.maxVisibleCharacters = 0;

        // Phase 1: typewriter
        int visible = 0;
        float charAcc = 0f;
        bool typewriterDone = totalChars == 0;

        while (!typewriterDone)
        {
            if (TickSkipHold()) { skipRequested = true; yield break; }

            if (input.Player.Fire.WasPressedThisFrame())
            {
                visible = totalChars;
                bodyText.maxVisibleCharacters = visible;
                typewriterDone = true;
                yield return null; // consuma il frame, evita advance immediato
                break;
            }

            charAcc += Time.unscaledDeltaTime * cps;
            while (charAcc >= 1f && visible < totalChars)
            {
                visible++;
                charAcc -= 1f;
            }
            bodyText.maxVisibleCharacters = visible;
            if (visible >= totalChars) typewriterDone = true;

            yield return null;
        }

        bodyText.maxVisibleCharacters = totalChars;

        // Phase 2: attendi tap per avanzare
        while (true)
        {
            if (TickSkipHold()) { skipRequested = true; yield break; }
            if (input.Player.Fire.WasPressedThisFrame())
            {
                yield return null; // consuma il frame, evita che la riga successiva inizi già con Fire=true
                break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Avanza il timer di hold del tasto Pausa.
    /// Ritorna true quando la soglia è raggiunta (skip richiesto).
    /// </summary>
    private bool TickSkipHold()
    {
        if (input.Player.Pause.IsPressed())
        {
            skipHoldTime += Time.unscaledDeltaTime;
            if (skipHoldTime >= skipHoldThreshold) return true;
        }
        else
        {
            skipHoldTime = 0f;
        }
        return false;
    }
}
