using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueSequence))]
public class DialogueSequenceEditor : Editor
{
    private const int WarnCharCount = 120;

    private string importItalian = "";
    private string importEnglish = "";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("Quick Import", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Incolla le righe italiane (una per riga) nella prima box, le inglesi nella seconda.\n" +
            "\"Parse → Lines\" sovrascrive l'array lines esistente.",
            MessageType.Info);

        EditorGUILayout.LabelField("Italiano");
        importItalian = EditorGUILayout.TextArea(importItalian, GUILayout.Height(90));

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("English (opzionale)");
        importEnglish = EditorGUILayout.TextArea(importEnglish, GUILayout.Height(90));

        EditorGUILayout.Space(6);
        if (GUILayout.Button("Clear All Lines"))
        {
            var seq = (DialogueSequence)target;
            Undo.RecordObject(seq, "Clear Dialogue Lines");
            seq.lines = new DialogueLine[0];
            EditorUtility.SetDirty(seq);
        }

        EditorGUILayout.Space(2);
        if (GUILayout.Button("Parse → Lines"))
        {
            var seq = (DialogueSequence)target;
            Undo.RecordObject(seq, "Parse Dialogue Lines");

            string[] itLines = SplitLines(importItalian);
            string[] enLines = SplitLines(importEnglish);

            var result = new List<DialogueLine>();
            int count = Mathf.Max(itLines.Length, enLines.Length);
            for (int i = 0; i < count; i++)
            {
                result.Add(new DialogueLine
                {
                    italian = i < itLines.Length ? itLines[i] : string.Empty,
                    english = i < enLines.Length ? enLines[i] : string.Empty,
                });
            }

            seq.lines = result.ToArray();
            EditorUtility.SetDirty(seq);
            importItalian = string.Empty;
            importEnglish = string.Empty;
        }

        // Warnings
        var targetSeq = (DialogueSequence)target;
        if (targetSeq.lines == null || targetSeq.lines.Length == 0) return;

        bool anyWarning = false;
        for (int i = 0; i < targetSeq.lines.Length; i++)
        {
            var line = targetSeq.lines[i];
            int itLen = line.italian?.Length ?? 0;
            int enLen = line.english?.Length ?? 0;
            int longest = Mathf.Max(itLen, enLen);
            if (longest > WarnCharCount)
            {
                if (!anyWarning)
                {
                    EditorGUILayout.Space(8);
                    EditorGUILayout.LabelField("Warnings", EditorStyles.boldLabel);
                    anyWarning = true;
                }
                EditorGUILayout.HelpBox(
                    $"Riga {i + 1} ({longest} caratteri): potrebbe eccedere il pannello. Spezza in due righe.",
                    MessageType.Warning);
            }
        }
    }

    private static string[] SplitLines(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return new string[0];
        var parts = raw.Split('\n');
        var result = new List<string>();
        foreach (var p in parts)
        {
            var t = p.Trim().TrimEnd('\r');
            if (!string.IsNullOrEmpty(t))
                result.Add(t);
        }
        return result.ToArray();
    }
}
