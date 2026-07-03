using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EndingChoiceObject : MonoBehaviour
{
    [Header("Choice")]
    [SerializeField] private EndingChoiceType choiceType;
    [SerializeField] private int minimumStoryBeatToInteract = 5;
    [SerializeField] private int storyBeatAfterChoice = -1;
    [SerializeField] private bool onlyAdvanceStory = true;

    [Header("Confirmation")]
    [SerializeField] private EndingChoicePanel choicePanelOverride;
    [SerializeField] private string confirmationTitle = "Make this choice?";
    [TextArea(2, 5)]
    [SerializeField] private string confirmationBody = "There may be no way back from this.";
    [SerializeField] private string confirmLabel = "Confirm";
    [SerializeField] private string cancelLabel = "Step Away";

    [Header("After Confirmation")]
    [SerializeField] private DialogueLine[] confirmedDialogue;
    [SerializeField] private UnityEvent onChoiceConfirmed;

    private static EndingChoicePanel sharedChoicePanel;
    private static DialoguePanel dialoguePanel;
    private static bool anyChoiceConfirmed;
    private bool hasChosen;

    public EndingChoiceType ChoiceType => choiceType;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        ResetChoiceState();
    }

    public static void ResetChoiceState()
    {
        sharedChoicePanel = null;
        dialoguePanel = null;
        anyChoiceConfirmed = false;
    }

    private void Awake()
    {
        anyChoiceConfirmed = false;
    }

    public bool CanInteract
    {
        get
        {
            return !hasChosen
                && !anyChoiceConfirmed
                && StoryProgress.Instance != null
                && StoryProgress.Instance.CurrentStoryBeat >= minimumStoryBeatToInteract;
        }
    }

    private void Start()
    {
        if (sharedChoicePanel == null)
        {
            sharedChoicePanel = choicePanelOverride;
        }

        if (sharedChoicePanel == null)
        {
            sharedChoicePanel = FindAnyObjectByType<EndingChoicePanel>();
        }

        if (dialoguePanel == null)
        {
            dialoguePanel = FindAnyObjectByType<DialoguePanel>();
        }
    }

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        if (sharedChoicePanel == null)
        {
            Debug.LogWarning("no ending choice panel found in scene.");
            return;
        }

        sharedChoicePanel.Show(
            confirmationTitle,
            confirmationBody,
            confirmLabel,
            cancelLabel,
            ConfirmChoice);
    }

    private void ConfirmChoice()
    {
        if (hasChosen)
        {
            return;
        }

        hasChosen = true;
        anyChoiceConfirmed = true;
        AdvanceStoryIfNeeded();

        if (dialoguePanel != null && confirmedDialogue != null && confirmedDialogue.Length > 0)
        {
            dialoguePanel.StartDialogue(confirmedDialogue, InvokeConfirmedEvent);
        }
        else
        {
            InvokeConfirmedEvent();
        }
    }

    private void AdvanceStoryIfNeeded()
    {
        if (storyBeatAfterChoice < 0)
        {
            return;
        }

        if (StoryProgress.Instance == null)
        {
            Debug.LogWarning("no progress found in scene.");
            return;
        }

        if (onlyAdvanceStory)
        {
            StoryProgress.Instance.AdvanceToStoryBeat(storyBeatAfterChoice);
        }
        else
        {
            StoryProgress.Instance.SetStoryBeat(storyBeatAfterChoice);
        }
    }

    private void InvokeConfirmedEvent()
    {
        onChoiceConfirmed?.Invoke();
    }
}
