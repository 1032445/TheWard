using UnityEngine;

public class LogbookObject : MonoBehaviour
{
    [SerializeField] private LogbookPanel logbookPanel;
    [SerializeField] private LogbookPage[] pages;

    [Header("Story Progress")]
    [SerializeField] private int storyBeatAfterFirstOpen = -1;
    [SerializeField] private int minimumStoryBeatToAdvance;
    [SerializeField] private bool onlyAdvanceStory = true;

    private bool hasAdvancedStory;

    public void Interact()
    {
        if (logbookPanel != null)
        {
            logbookPanel.Show(pages);
        }
        else
        {
            Debug.LogWarning("no logbook panel assigned.");
        }

        AdvanceStoryIfNeeded();
    }

    private void AdvanceStoryIfNeeded()
    {
        if (hasAdvancedStory || storyBeatAfterFirstOpen < 0)
        {
            return;
        }

        if (StoryProgress.Instance == null)
        {
            Debug.LogWarning("no progress found in scene.");
            return;
        }

        if (StoryProgress.Instance.CurrentStoryBeat < minimumStoryBeatToAdvance)
        {
            return;
        }

        if (onlyAdvanceStory)
        {
            StoryProgress.Instance.AdvanceToStoryBeat(storyBeatAfterFirstOpen);
        }
        else
        {
            StoryProgress.Instance.SetStoryBeat(storyBeatAfterFirstOpen);
        }

        hasAdvancedStory = true;
    }
}
