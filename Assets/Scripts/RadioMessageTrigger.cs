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

    private DialoguePanel dialoguePanel;
    private bool hasPlayed;
    private Coroutine playRoutine;

    private void OnEnable()
    {
        StoryProgress.StoryBeatChanged += HandleStoryBeatChanged;
    }

    private void OnDisable()
    {
        StoryProgress.StoryBeatChanged -= HandleStoryBeatChanged;
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

        while (TextPanel.IsAnyPanelOpen || DialoguePanel.IsAnyDialogueOpen)
        {
            yield return null;
        }

        PlayRadioMessage();
        playRoutine = null;
    }

    private void PlayRadioMessage()
    {
        hasPlayed = true;

        if (audioSource != null && radioClip != null)
        {
            audioSource.PlayOneShot(radioClip);
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.StartDialogue(messageLines);
        }
        else
        {
            Debug.LogWarning("no dialogue panel found in scene.");
        }
    }
}
