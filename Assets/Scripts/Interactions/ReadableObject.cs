using UnityEngine;

public class ReadableObject : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string content = "placeholder text";

    [Header("Personnel File")]
    [SerializeField] private PersonnelFilePanel personnelFilePanel;
    [SerializeField] private PersonnelFileRecord personnelFileRecord;

    [Header("Story Progress")]
    [SerializeField] private int minimumStoryBeatToInteract;
    [SerializeField] private int storyBeatAfterReading = -1;
    [SerializeField] private int minimumStoryBeatToAdvance;
    [SerializeField] private bool onlyAdvanceStory = true;
    [SerializeField] private bool advanceStoryOnlyOnce = true;

    private static TextPanel textPanel;
    private bool hasAdvancedStory;

    private void Start()
    {
        if (textPanel == null)
        {
            textPanel = FindAnyObjectByType<TextPanel>();
        }
    }

    public bool CanInteract
    {
        get
        {
            if (minimumStoryBeatToInteract <= 0)
            {
                return true;
            }

            return StoryProgress.Instance != null
                && StoryProgress.Instance.CurrentStoryBeat >= minimumStoryBeatToInteract;
        }
    }

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        if (personnelFilePanel != null)
        {
            personnelFilePanel.Show(personnelFileRecord);
        }
        else if (textPanel != null)
        {
            textPanel.Show(content);
        }
        else
        {
            Debug.LogWarning("no panel found in scene");
        }

        AdvanceStoryIfNeeded();
    }

    private void AdvanceStoryIfNeeded()
    {
        if (storyBeatAfterReading < 0)
        {
            return;
        }

        if (advanceStoryOnlyOnce && hasAdvancedStory)
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
            StoryProgress.Instance.AdvanceToStoryBeat(storyBeatAfterReading);
        }
        else
        {
            StoryProgress.Instance.SetStoryBeat(storyBeatAfterReading);
        }

        hasAdvancedStory = true;
    }
}
