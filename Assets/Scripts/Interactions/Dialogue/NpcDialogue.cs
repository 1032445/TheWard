using UnityEngine;

public class NpcDialogue : MonoBehaviour
{
    [SerializeField] private DialogueLine[] lines;

    private static DialoguePanel dialoguePanel;

    private void Start()
    {
        if (dialoguePanel == null)
        {
            dialoguePanel = FindAnyObjectByType<DialoguePanel>();
        }
    }

    public void Interact()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.StartDialogue(lines);
        }
        else
        {
            Debug.LogWarning("No DialoguePanel found in scene.");
        }
    }
}