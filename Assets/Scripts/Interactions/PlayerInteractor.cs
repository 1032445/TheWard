using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private GameObject interactPromptUI;
    [SerializeField] private Key interactKey = Key.E;
    [SerializeField] private Key taskBoardKey = Key.T;

    private ReadableObject currentReadable;
    private NpcDialogue currentNpc;
    private PatrolTaskObject currentPatrolTaskObject;
    private PatrolBoard currentPatrolBoard;
    private KitchenDoorInteraction currentKitchenDoor;
    private LogbookObject currentLogbook;
    private EndingChoiceObject currentEndingChoice;

    private TextPanel textPanel;
    private DialoguePanel dialoguePanel;
    private PatrolBoardPanel patrolBoardPanel;
    private PlayerController playerController;

    private void Start()
    {
        textPanel = FindAnyObjectByType<TextPanel>();
        dialoguePanel = FindAnyObjectByType<DialoguePanel>();
        patrolBoardPanel = FindAnyObjectByType<PatrolBoardPanel>(FindObjectsInactive.Include);
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (EndingChoicePanel.IsAnyEndingChoiceOpen)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current[taskBoardKey].wasPressedThisFrame)
        {
            ToggleTaskBoard();
            return;
        }

        if (!Keyboard.current[interactKey].wasPressedThisFrame) return;

        if (DialoguePanel.IsAnyDialogueOpen && (dialoguePanel == null || !dialoguePanel.IsOpen))
        {
            DialoguePanel.HideOpenPanel();
            return;
        }

        if (dialoguePanel != null && dialoguePanel.IsOpen)
        {
            dialoguePanel.Advance();
            return;
        }

        if (PersonnelFilePanel.IsAnyPersonnelFileOpen)
        {
            PersonnelFilePanel.HideOpenPanel();
            return;
        }

        if (LogbookPanel.IsAnyLogbookOpen)
        {
            LogbookPanel.HideOpenPanel();
            return;
        }

        if (PatrolBoardPanel.IsAnyPatrolBoardOpen)
        {
            PatrolBoardPanel.HideOpenPanel();
            return;
        }

        if (ReadableImagePanel.IsAnyReadableImageOpen)
        {
            ReadableImagePanel.HideOpenPanel();
            return;
        }

        if (TextPanel.IsAnyPanelOpen)
        {
            TextPanel.AdvanceOpenPanel();
            return;
        }

        if (ReadableObject.IsAnyGenericPanelOpen)
        {
            ReadableObject.HideOpenPanel();
            return;
        }

        if (currentNpc != null)
        {
            if (currentNpc.CanInteract)
            {
                if (playerController != null)
                {
                    playerController.FaceToward(currentNpc.transform.position);
                }

                currentNpc.FaceToward(transform.position);
                currentNpc.Interact();
            }
            else
            {
                currentNpc = null;
                SetPromptVisible(false);
            }
        }
        else if (currentReadable != null)
        {
            if (currentReadable.CanInteract)
            {
                currentReadable.Interact();
            }
            else
            {
                currentReadable = null;
                SetPromptVisible(false);
            }
        }
        else if (currentPatrolTaskObject != null)
        {
            currentPatrolTaskObject.Interact();
        }
        else if (currentPatrolBoard != null)
        {
            currentPatrolBoard.Interact();
        }
        else if (currentKitchenDoor != null)
        {
            currentKitchenDoor.Interact();
        }
        else if (currentLogbook != null)
        {
            if (currentLogbook.CanInteract)
            {
                currentLogbook.Interact();
            }
            else
            {
                currentLogbook = null;
                SetPromptVisible(false);
            }
        }
        else if (currentEndingChoice != null)
        {
            if (currentEndingChoice.CanInteract)
            {
                currentEndingChoice.Interact();
            }
            else
            {
                currentEndingChoice = null;
                SetPromptVisible(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        NpcDialogue npc = other.GetComponent<NpcDialogue>();
        if (npc != null)
        {
            if (npc.CanInteract)
            {
                currentNpc = npc;
                SetPromptVisible(true);
            }
            return;
        }

        ReadableObject readable = other.GetComponent<ReadableObject>();
        if (readable != null)
        {
            if (readable.CanInteract)
            {
                currentReadable = readable;
                SetPromptVisible(true);
            }
            return;
        }

        PatrolTaskObject patrolTaskObject = other.GetComponent<PatrolTaskObject>();
        if (patrolTaskObject != null)
        {
            currentPatrolTaskObject = patrolTaskObject;
            SetPromptVisible(true);
            return;
        }

        PatrolBoard patrolBoard = other.GetComponent<PatrolBoard>();
        if (patrolBoard != null)
        {
            currentPatrolBoard = patrolBoard;
            SetPromptVisible(true);
            return;
        }

        KitchenDoorInteraction kitchenDoor = other.GetComponent<KitchenDoorInteraction>();
        if (kitchenDoor != null)
        {
            currentKitchenDoor = kitchenDoor;
            SetPromptVisible(true);
            return;
        }

        LogbookObject logbook = other.GetComponent<LogbookObject>();
        if (logbook != null)
        {
            if (logbook.CanInteract)
            {
                currentLogbook = logbook;
                SetPromptVisible(true);
            }
            return;
        }

        EndingChoiceObject endingChoice = other.GetComponent<EndingChoiceObject>();
        if (endingChoice != null)
        {
            if (endingChoice.CanInteract)
            {
                currentEndingChoice = endingChoice;
                SetPromptVisible(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (currentReadable == null)
        {
            ReadableObject readable = other.GetComponent<ReadableObject>();
            if (readable != null && readable.CanInteract)
            {
                currentReadable = readable;
                SetPromptVisible(true);
                return;
            }
        }

        if (currentLogbook == null)
        {
            LogbookObject logbook = other.GetComponent<LogbookObject>();
            if (logbook != null && logbook.CanInteract)
            {
                currentLogbook = logbook;
                SetPromptVisible(true);
                return;
            }
        }

        if (currentEndingChoice == null)
        {
            EndingChoiceObject endingChoice = other.GetComponent<EndingChoiceObject>();
            if (endingChoice != null && endingChoice.CanInteract)
            {
                currentEndingChoice = endingChoice;
                SetPromptVisible(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        NpcDialogue npc = other.GetComponent<NpcDialogue>();
        if (npc != null && npc == currentNpc)
        {
            currentNpc = null;
            SetPromptVisible(false);
            return;
        }

        ReadableObject readable = other.GetComponent<ReadableObject>();
        if (readable != null && readable == currentReadable)
        {
            currentReadable = null;
            SetPromptVisible(false);
            return;
        }

        PatrolTaskObject patrolTaskObject = other.GetComponent<PatrolTaskObject>();
        if (patrolTaskObject != null && patrolTaskObject == currentPatrolTaskObject)
        {
            currentPatrolTaskObject = null;
            SetPromptVisible(false);
            return;
        }

        PatrolBoard patrolBoard = other.GetComponent<PatrolBoard>();
        if (patrolBoard != null && patrolBoard == currentPatrolBoard)
        {
            currentPatrolBoard = null;
            SetPromptVisible(false);
            return;
        }

        KitchenDoorInteraction kitchenDoor = other.GetComponent<KitchenDoorInteraction>();
        if (kitchenDoor != null && kitchenDoor == currentKitchenDoor)
        {
            currentKitchenDoor = null;
            SetPromptVisible(false);
            return;
        }

        LogbookObject logbook = other.GetComponent<LogbookObject>();
        if (logbook != null && logbook == currentLogbook)
        {
            currentLogbook = null;
            SetPromptVisible(false);
            return;
        }

        EndingChoiceObject endingChoice = other.GetComponent<EndingChoiceObject>();
        if (endingChoice != null && endingChoice == currentEndingChoice)
        {
            currentEndingChoice = null;
            SetPromptVisible(false);
        }
    }

    private void SetPromptVisible(bool visible)
    {
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(visible);
        }
    }

    private void ToggleTaskBoard()
    {
        if (PatrolBoardPanel.IsAnyPatrolBoardOpen)
        {
            PatrolBoardPanel.HideOpenPanel();
            return;
        }

        if (IsReadingOrTalking())
        {
            return;
        }

        if (patrolBoardPanel == null)
        {
            patrolBoardPanel = FindAnyObjectByType<PatrolBoardPanel>(FindObjectsInactive.Include);
        }

        if (patrolBoardPanel != null)
        {
            patrolBoardPanel.Show();
        }
        else
        {
            Debug.LogWarning("no patrol board panel found in scene.");
        }
    }

    private bool IsReadingOrTalking()
    {
        return TextPanel.IsAnyPanelOpen
            || DialoguePanel.IsAnyDialogueOpen
            || PersonnelFilePanel.IsAnyPersonnelFileOpen
            || LogbookPanel.IsAnyLogbookOpen
            || ReadableImagePanel.IsAnyReadableImageOpen
            || EndingChoicePanel.IsAnyEndingChoiceOpen
            || EndingSequence.IsEndingSequencePlaying;
    }
}
