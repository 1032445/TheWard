using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class DialoguePanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private float charactersPerSecond = 40f;
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioClip typingClip;
    [SerializeField] private bool loopTypingClip = true;

    public static bool IsAnyDialogueOpen { get; private set; }

    private DialogueLine[] currentLines;
    private int currentIndex;
    private Action onDialogueComplete;
    private Coroutine typewriterRoutine;
    private bool isTyping;
    private AudioClip previousTypingAudioClip;
    private bool previousTypingLoopSetting;
    private bool isPlayingTypingAudio;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        ResetOpenState();
    }

    public static void ResetOpenState()
    {
        IsAnyDialogueOpen = false;
    }

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
        if (isTyping)
        {
            FinishTypingLine();
            return true;
        }

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
        StopTypingLine();
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
            StartTypingLine(line.text);
        }
    }

    private void StartTypingLine(string text)
    {
        StopTypingLine();

        contentText.text = text;
        contentText.maxVisibleCharacters = 0;
        typewriterRoutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        contentText.ForceMeshUpdate();

        int totalVisibleCharacters = contentText.textInfo.characterCount;
        float delay = charactersPerSecond > 0f ? 1f / charactersPerSecond : 0f;
        PlayTypingAudio();

        for (int visibleCharacters = 1; visibleCharacters <= totalVisibleCharacters; visibleCharacters++)
        {
            contentText.maxVisibleCharacters = visibleCharacters;

            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
        }

        contentText.maxVisibleCharacters = int.MaxValue;
        typewriterRoutine = null;
        isTyping = false;
        StopTypingAudio();
    }

    private void FinishTypingLine()
    {
        StopTypingLine();

        if (contentText != null)
        {
            contentText.maxVisibleCharacters = int.MaxValue;
        }
    }

    private void StopTypingLine()
    {
        if (typewriterRoutine != null)
        {
            StopCoroutine(typewriterRoutine);
            typewriterRoutine = null;
        }

        isTyping = false;
        StopTypingAudio();
    }

    private void PlayTypingAudio()
    {
        if (typingAudioSource == null || typingClip == null)
        {
            return;
        }

        previousTypingAudioClip = typingAudioSource.clip;
        previousTypingLoopSetting = typingAudioSource.loop;
        typingAudioSource.clip = typingClip;
        typingAudioSource.loop = loopTypingClip;
        typingAudioSource.Play();
        isPlayingTypingAudio = true;
    }

    private void StopTypingAudio()
    {
        if (!isPlayingTypingAudio || typingAudioSource == null)
        {
            return;
        }

        typingAudioSource.Stop();
        typingAudioSource.clip = previousTypingAudioClip;
        typingAudioSource.loop = previousTypingLoopSetting;
        isPlayingTypingAudio = false;
    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
