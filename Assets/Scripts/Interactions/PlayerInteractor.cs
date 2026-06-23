using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private GameObject interactPromptUI;
    [SerializeField] private Key interactKey = Key.E;

    private ReadableObject currentReadable;
    private NpcDialogue currentNpc;

    private TextPanel textPanel;
    private DialoguePanel dialoguePanel;

    private void Start()
    {
        textPanel = FindAnyObjectByType<TextPanel>();
        dialoguePanel = FindAnyObjectByType<DialoguePanel>();
    }

    private void Update()
    {
        if (!Keyboard.current[interactKey].wasPressedThisFrame) return;

        if (dialoguePanel != null && dialoguePanel.IsOpen)
        {
            dialoguePanel.Advance();
            return;
        }

        if (textPanel != null && textPanel.IsOpen)
        {
            textPanel.Hide();
            return;
        }

        if (currentNpc != null)
        {
            if (currentNpc.CanInteract)
            {
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
            currentReadable.Interact();
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
            currentReadable = readable;
            SetPromptVisible(true);
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
        }
    }

    private void SetPromptVisible(bool visible)
    {
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(visible);
        }
    }
}
