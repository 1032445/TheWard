using System.Collections;
using UnityEngine;

public class RadioMessageTrigger : MonoBehaviour
{
    [SerializeField] private int storyBeatToPlay = 1;
    [SerializeField] private float delaySeconds = 2f;
    [SerializeField] private bool playOnce = true;
    [SerializeField] private DialogueLine[] messageLines;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip radioClip;
    [SerializeField] private bool loopRadioClipDuringMessage = true;

    private DialoguePanel dialoguePanel;
    private bool hasPlayed;
    private Coroutine playRoutine;
    private bool isLoopingRadioClip;
    private AudioClip previousAudioClip;
    private bool previousLoopSetting;

    private void OnEnable()
    {
        StoryProgress.StoryBeatChanged += HandleStoryBeatChanged;
    }

    private void OnDisable()
    {
        StoryProgress.StoryBeatChanged -= HandleStoryBeatChanged;
        StopLoopingRadioClip();
    }

    private void Start()
    {
        dialoguePanel = FindAnyObjectByType<DialoguePanel>();

        if (StoryProgress.Instance != null)
        {
            TryPlayForStoryBeat(StoryProgress.Instance.CurrentStoryBeat);
        }
    }

    private void HandleStoryBeatChanged(int storyBeat)
    {
        TryPlayForStoryBeat(storyBeat);
    }

    private void TryPlayForStoryBeat(int storyBeat)
    {
        if (storyBeat != storyBeatToPlay)
        {
            return;
        }

        if (playOnce && hasPlayed)
        {
            return;
        }

        if (playRoutine == null)
        {
            playRoutine = StartCoroutine(PlayAfterDelay());
        }
    }

    private IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);

        while (TextPanel.IsAnyPanelOpen
            || DialoguePanel.IsAnyDialogueOpen
            || PersonnelFilePanel.IsAnyPersonnelFileOpen
            || LogbookPanel.IsAnyLogbookOpen
            || EndingChoicePanel.IsAnyEndingChoiceOpen)
        {
            yield return null;
        }

        PlayRadioMessage();
        playRoutine = null;
    }

    private void PlayRadioMessage()
    {
        hasPlayed = true;
        bool canShowDialogue = dialoguePanel != null && messageLines != null && messageLines.Length > 0;

        if (canShowDialogue)
        {
            PlayRadioAudio(true);
            dialoguePanel.StartDialogue(messageLines, StopLoopingRadioClip);
        }
        else
        {
            PlayRadioAudio(false);

            if (dialoguePanel == null)
            {
                Debug.LogWarning("no dialogue panel found in scene.");
            }
            else
            {
                Debug.LogWarning("radio message trigger has no dialogue lines.");
            }
        }
    }

    private void PlayRadioAudio(bool canLoopDuringMessage)
    {
        if (audioSource == null || radioClip == null)
        {
            return;
        }

        if (!canLoopDuringMessage || !loopRadioClipDuringMessage)
        {
            audioSource.PlayOneShot(radioClip);
            return;
        }

        previousAudioClip = audioSource.clip;
        previousLoopSetting = audioSource.loop;
        audioSource.loop = true;

        if (audioSource.clip != radioClip)
        {
            audioSource.clip = radioClip;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        isLoopingRadioClip = true;
    }

    private void StopLoopingRadioClip()
    {
        if (!isLoopingRadioClip || audioSource == null)
        {
            return;
        }

        audioSource.Stop();
        audioSource.clip = previousAudioClip;
        audioSource.loop = previousLoopSetting;
        isLoopingRadioClip = false;
    }
}
