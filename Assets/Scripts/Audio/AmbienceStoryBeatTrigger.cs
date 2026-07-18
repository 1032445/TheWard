using UnityEngine;

public class AmbienceStoryBeatTrigger : MonoBehaviour
{
    [SerializeField] private int storyBeatToPlay = 5;
    [SerializeField] private AmbienceState state = AmbienceState.Confrontation;
    [SerializeField] private bool playOnce = true;

    private bool hasPlayed;

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
        if (storyBeat != storyBeatToPlay || (playOnce && hasPlayed))
        {
            return;
        }

        if (AmbienceManager.Instance == null)
        {
            Debug.LogWarning("no ambience manager found in scene.");
            return;
        }

        hasPlayed = true;
        AmbienceManager.Instance.SetState(state);
    }
}
