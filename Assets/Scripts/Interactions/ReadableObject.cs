using UnityEngine;

public class ReadableObject : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string content = "placeholder text";

    [Header("Personnel File")]
    [SerializeField] private PersonnelFilePanel personnelFilePanel;
    [SerializeField] private PersonnelFileRecord personnelFileRecord;

    [Header("Story Progress")]
    [SerializeField] private int storyBeatAfterReading = -1;
    [SerializeField] private int minimumStoryBeatToAdvance;
    [SerializeField] private bool onlyAdvanceStory = true;

    private static TextPanel textPanel;

    private void Start()
    {
        if (textPanel == null)
        {
            textPanel = FindAnyObjectByType<TextPanel>();
        }
    }

    public void Interact()
    {
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
    }
}
