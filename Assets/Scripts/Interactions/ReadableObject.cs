using UnityEngine;

public class ReadableObject : MonoBehaviour
{
    [Header("Text Readable")]
    [SerializeField] private GameObject textPanelToOpen;
    [TextArea(3, 10)]
    [SerializeField] private string content = "placeholder text";
    [TextArea(3, 10)]
    [SerializeField] private string[] contentPages;

    [Header("Image Readable")]
    [SerializeField] private GameObject imagePanelToOpen;
    [SerializeField] private Sprite readableImage;
    [SerializeField] private string imageTitle;
    [TextArea(2, 5)]
    [SerializeField] private string imageDescription;

    [Header("Personnel File")]
    [SerializeField] private PersonnelFilePanel personnelFilePanel;
    [SerializeField] private PersonnelFileRecord[] personnelFileRecords;
    [SerializeField] private PersonnelFileRecord personnelFileRecord;

    [Header("Story Progress")]
    [SerializeField] private int minimumStoryBeatToInteract;
    [SerializeField] private int storyBeatAfterReading = -1;
    [SerializeField] private int minimumStoryBeatToAdvance;
    [SerializeField] private bool onlyAdvanceStory = true;
    [SerializeField] private bool advanceStoryOnlyOnce = true;

    private bool hasAdvancedStory;

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

        if (personnelFilePanel != null && personnelFileRecords != null && personnelFileRecords.Length > 0)
        {
            personnelFilePanel.Show(personnelFileRecords);
        }
        else if (personnelFilePanel != null)
        {
            personnelFilePanel.Show(personnelFileRecord);
        }
        else if (imagePanelToOpen != null && readableImage != null)
        {
            ReadableImagePanel imagePanel = GetReadableImagePanel();
            if (imagePanel != null)
            {
                imagePanel.Show(readableImage, imageTitle, imageDescription);
            }
            else
            {
                Debug.LogWarning("readable object image panel has no readable image panel component.");
            }
        }
        else if (textPanelToOpen != null)
        {
            TextPanel panel = GetTextPanel();
            if (panel != null)
            {
                if (contentPages != null && contentPages.Length > 0)
                {
                    panel.Show("", contentPages);
                }
                else
                {
                    panel.Show(content);
                }
            }
            else
            {
                Debug.LogWarning("readable object text panel has no text panel component.");
            }
        }
        else
        {
            Debug.LogWarning("readable object has no panel assigned.");
        }

        AdvanceStoryIfNeeded();
    }

    private TextPanel GetTextPanel()
    {
        TextPanel panel = textPanelToOpen.GetComponent<TextPanel>();
        if (panel != null)
        {
            return panel;
        }

        panel = textPanelToOpen.GetComponentInChildren<TextPanel>(true);
        if (panel != null)
        {
            return panel;
        }

        return textPanelToOpen.GetComponentInParent<TextPanel>(true);
    }

    private ReadableImagePanel GetReadableImagePanel()
    {
        ReadableImagePanel panel = imagePanelToOpen.GetComponent<ReadableImagePanel>();
        if (panel != null)
        {
            return panel;
        }

        panel = imagePanelToOpen.GetComponentInChildren<ReadableImagePanel>(true);
        if (panel != null)
        {
            return panel;
        }

        return imagePanelToOpen.GetComponentInParent<ReadableImagePanel>(true);
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
