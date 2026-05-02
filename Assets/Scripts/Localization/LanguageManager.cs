using System.Collections.Generic;
using UnityEngine;

public enum Lang { IT, EN }

/// <summary>
/// Singleton di localizzazione. Persiste la lingua scelta in PlayerPrefs.
/// Ricava il testo corretto da DialogueLine in base a Current.
/// Tabella speaker (es. "IA"/"AI") hardcoded qui — poche voci.
/// </summary>
public class LanguageManager : MonoBehaviour
{
    private static LanguageManager _instance;

    /// <summary>
    /// Lazy singleton: si auto-crea al primo accesso. Non richiede di trascinare
    /// un GameObject in scena.
    /// </summary>
    public static LanguageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("LanguageManager");
                _instance = go.AddComponent<LanguageManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private const string PrefsKey = "language";
    private const Lang DefaultLang = Lang.IT;

    public Lang Current { get; private set; } = DefaultLang;

    public System.Action<Lang> OnLanguageChanged;

    private static readonly Dictionary<string, (string it, string en)> SpeakerTable = new()
    {
        { "IA",          ("IA", "AI") },
        { "PROTAGONIST", ("PROTAGONISTA", "PROTAGONIST") },
        { "ALPHA",       ("ALPHA", "ALPHA") },
    };

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        if (transform.parent == null) DontDestroyOnLoad(gameObject);

        Load();
    }

    public void SetLanguage(Lang lang)
    {
        if (Current == lang) return;
        Current = lang;
        PlayerPrefs.SetInt(PrefsKey, (int)lang);
        PlayerPrefs.Save();
        OnLanguageChanged?.Invoke(lang);
    }

    public string Get(DialogueLine line)
    {
        if (line == null) return string.Empty;

        switch (Current)
        {
            case Lang.EN:
                return string.IsNullOrEmpty(line.english) ? line.italian : line.english;
            case Lang.IT:
            default:
                return string.IsNullOrEmpty(line.italian) ? line.english : line.italian;
        }
    }

    public string GetSpeaker(string speakerKey)
    {
        if (string.IsNullOrEmpty(speakerKey)) return string.Empty;
        if (SpeakerTable.TryGetValue(speakerKey, out var pair))
            return Current == Lang.EN ? pair.en : pair.it;
        return speakerKey;
    }

    private void Load()
    {
        if (PlayerPrefs.HasKey(PrefsKey))
            Current = (Lang)PlayerPrefs.GetInt(PrefsKey);
        else
            Current = DefaultLang;
    }
}
