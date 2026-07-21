using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    [Serializable]
    private class SpeakerPortrait
    {
        [SerializeField] private string speakerName;
        [SerializeField] private Sprite portrait;

        public string SpeakerName => speakerName;
        public Sprite Portrait => portrait;
    }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private float charactersPerSecond = 40f;
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioClip typingClip;
    [SerializeField] private bool loopTypingClip = true;

    [Header("Portraits")]
    [SerializeField] private GameObject portraitRoot;
    [SerializeField] private Image portraitImage;
    [SerializeField] private SpeakerPortrait[] speakerPortraits;

    public static bool IsAnyDialogueOpen { get; private set; }
    public static DialoguePanel Primary { get; private set; }

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
        Primary = null;
    }

    public static void HideOpenPanel()
    {
        DialoguePanel panel = FindAnyObjectByType<DialoguePanel>(FindObjectsInactive.Include);
        if (panel != null && panel.IsOpen)
        {
            panel.Close();
            return;
        }

        IsAnyDialogueOpen = false;
    }

    private void Awake()
    {
        if (Primary == null && panelRoot != null && speakerText != null && contentText != null)
        {
            Primary = this;
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        ClearPortrait();
    }

    private void OnDestroy()
    {
        if (Primary == this)
        {
            Primary = null;
        }
    }

    public void StartDialogue(DialogueLine[] lines, Action onComplete = null)
    {
        if (lines == null || lines.Length == 0) return;

        if (panelRoot == null)
        {
            Debug.LogWarning("dialogue panel has no panel root assigned.");
            onComplete?.Invoke();
            return;
        }

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
        ClearPortrait();
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

        SetPortrait(line.speaker);

        if (contentText != null)
        {
            StartTypingLine(line.text);
        }
    }

    private void SetPortrait(string speakerName)
    {
        Sprite portrait = GetPortraitForSpeaker(speakerName);

        if (portraitImage != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.enabled = portrait != null;
        }

        if (portraitRoot != null)
        {
            if (portraitRoot != panelRoot)
            {
                portraitRoot.SetActive(portrait != null);
            }
        }
    }

    private Sprite GetPortraitForSpeaker(string speakerName)
    {
        if (speakerPortraits == null)
        {
            return null;
        }

        foreach (SpeakerPortrait speakerPortrait in speakerPortraits)
        {
            if (speakerPortrait != null && speakerPortrait.SpeakerName == speakerName)
            {
                return speakerPortrait.Portrait;
            }
        }

        return null;
    }

    private void ClearPortrait()
    {
        if (portraitImage != null)
        {
            portraitImage.sprite = null;
            portraitImage.enabled = false;
        }

        if (portraitRoot != null)
        {
            if (portraitRoot != panelRoot)
            {
                portraitRoot.SetActive(false);
            }
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
