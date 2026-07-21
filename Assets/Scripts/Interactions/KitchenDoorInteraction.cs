using UnityEngine;

public class KitchenDoorInteraction : MonoBehaviour
{
    [Header("Patrol Check")]
    [SerializeField] private int patrolTaskNumber = 1;
    [SerializeField] private DialogueLine[] staffDialogue;
    [TextArea(2, 5)]
    [SerializeField] private string checkedMessage = "The kitchen access light is red. The door does not give.";
    [TextArea(2, 5)]
    [SerializeField] private string tooEarlyMessage = "This is scheduled later in the patrol.";
    [TextArea(2, 5)]
    [SerializeField] private string alreadyCheckedMessage = "The kitchen doors are already checked.";

    [Header("Door Opening")]
    [SerializeField] private int storyBeatRequiredToOpen = 3;
    [SerializeField] private GameObject doorObjectToDisable;
    [TextArea(2, 5)]
    [SerializeField] private string openedMessage = "The kitchen doors open.";
    [SerializeField] private bool showOpenedMessage = true;

    private TextPanel textPanel;
    private DialoguePanel dialoguePanel;
    private bool isOpen;

    private void Start()
    {
        if (doorObjectToDisable == null)
        {
            doorObjectToDisable = gameObject;
        }

        textPanel = FindAnyObjectByType<TextPanel>();
        dialoguePanel = FindAnyObjectByType<DialoguePanel>();
    }

    public void Interact()
    {
        if (PatrolManager.Instance != null)
        {
            PatrolTaskResult patrolResult = GetPatrolTaskState();
            if (patrolResult == PatrolTaskResult.TooEarly || patrolResult == PatrolTaskResult.Completed)
            {
                CheckDoorForPatrol();
                return;
            }
        }

        if (CanOpenDoor())
        {
            OpenDoor();
            return;
        }

        CheckDoorForPatrol();
    }

    private PatrolTaskResult GetPatrolTaskState()
    {
        if (patrolTaskNumber < PatrolManager.Instance.CurrentTaskNumber)
        {
            return PatrolTaskResult.AlreadyDone;
        }

        if (patrolTaskNumber > PatrolManager.Instance.CurrentTaskNumber)
        {
            return PatrolTaskResult.TooEarly;
        }

        return PatrolTaskResult.Completed;
    }

    private bool CanOpenDoor()
    {
        return !isOpen
            && StoryProgress.Instance != null
            && StoryProgress.Instance.CurrentStoryBeat >= storyBeatRequiredToOpen;
    }

    private void CheckDoorForPatrol()
    {
        if (PatrolManager.Instance == null)
        {
            ShowMessage("No patrol route found.");
            return;
        }

        if (patrolTaskNumber < PatrolManager.Instance.CurrentTaskNumber)
        {
            ShowMessage(alreadyCheckedMessage);
            return;
        }

        if (patrolTaskNumber > PatrolManager.Instance.CurrentTaskNumber)
        {
            ShowMessage(tooEarlyMessage);
            return;
        }

        if (dialoguePanel != null && staffDialogue != null && staffDialogue.Length > 0)
        {
            dialoguePanel.StartDialogue(staffDialogue, CompletePatrolTask);
        }
        else
        {
            CompletePatrolTask();
            ShowMessage(string.IsNullOrWhiteSpace(checkedMessage) ? "Kitchen access checked." : checkedMessage);
        }
    }

    private void CompletePatrolTask()
    {
        if (PatrolManager.Instance != null)
        {
            PatrolManager.Instance.TryCompleteTask(patrolTaskNumber);
        }
    }

    private void OpenDoor()
    {
        isOpen = true;

        if (showOpenedMessage)
        {
            ShowMessage(openedMessage);
        }

        if (doorObjectToDisable != null)
        {
            doorObjectToDisable.SetActive(false);
        }
    }

    private void ShowMessage(string message)
    {
        if (textPanel != null)
        {
            textPanel.Show(message);
        }
        else
        {
            Debug.LogWarning(message);
        }
    }
}
