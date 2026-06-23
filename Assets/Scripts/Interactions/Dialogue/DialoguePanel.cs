using UnityEngine;
using TMPro;
using System;

public class DialoguePanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI contentText;

    public static bool IsAnyDialogueOpen { get; private set; }

    private DialogueLine[] currentLines;
    private int currentIndex;
    private Action onDialogueComplete;

    private void Awake()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void StartDialogue(DialogueLine[] lines, Action onComplete = null)
    {
        if (lines == null || lines.Length == 0) return;

        currentLines = lines;
        currentIndex = 0;
        onDialogueComplete = onComplete;
        panelRoot.SetActive(true);
        IsAnyDialogueOpen = true;
        ShowCurrentLine();
    }

    public bool Advance()
    {
        currentIndex++;

        if (currentIndex >= currentLines.Length)
        {
            Close();
            return false;
        }

        ShowCurrentLine();
        return true;
    }

    public void Close()
    {
        panelRoot.SetActive(false);
        IsAnyDialogueOpen = false;
        currentLines = null;
        Action completed = onDialogueComplete;
        onDialogueComplete = null;
        completed?.Invoke();
    }

    private void ShowCurrentLine()
    {
        DialogueLine line = currentLines[currentIndex];

        if (speakerText != null)
        {
            speakerText.text = line.speaker;
            speakerText.gameObject.SetActive(!string.IsNullOrEmpty(line.speaker));
        }

        if (contentText != null)
        {
            contentText.text = line.text;
        }
    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
