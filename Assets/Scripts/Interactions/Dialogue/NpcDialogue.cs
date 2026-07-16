using UnityEngine;

public class NpcDialogue : MonoBehaviour
{
    [SerializeField] private NpcStoryState[] storyStates;

    private static DialoguePanel dialoguePanel;
    private NpcStoryState currentState;
    private SpriteRenderer[] spriteRenderers;
    private Sprite[] defaultSprites;
    private Collider2D[] colliders;
    private int appliedStoryBeat = int.MinValue;

    public bool CanInteract
    {
        get
        {
            if (appliedStoryBeat == int.MinValue)
            {
                ApplyStoryState(force: true);
            }

            return currentState != null && currentState.IsInteractable && currentState.HasDialogue;
        }
    }

    private void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        defaultSprites = new Sprite[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            defaultSprites[i] = spriteRenderers[i].sprite;
        }

        colliders = GetComponentsInChildren<Collider2D>(true);
    }

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
        if (dialoguePanel == null)
        {
            dialoguePanel = FindAnyObjectByType<DialoguePanel>();
        }

        ApplyStoryState(force: true);
    }

    public void Interact()
    {
        ApplyStoryState();

        if (!CanInteract)
        {
            return;
        }

        DialogueLine[] activeLines = currentState != null ? currentState.Lines : null;
        if (activeLines == null || activeLines.Length == 0)
        {
            return;
        }

        if (dialoguePanel != null)
        {
            NpcStoryState dialogueState = currentState;
            dialoguePanel.StartDialogue(activeLines, () => HandleDialogueComplete(dialogueState));
        }
        else
        {
            Debug.LogWarning("no panel found in scene.");
        }
    }

    private void HandleStoryBeatChanged(int storyBeat)
    {
        ApplyStoryState(storyBeat, force: true);
    }

    private void ApplyStoryState(bool force = false)
    {
        int storyBeat = StoryProgress.Instance != null ? StoryProgress.Instance.CurrentStoryBeat : 0;
        ApplyStoryState(storyBeat, force);
    }

    private void ApplyStoryState(int storyBeat, bool force = false)
    {
        if (!force && storyBeat == appliedStoryBeat)
        {
            return;
        }

        appliedStoryBeat = storyBeat;
        currentState = FindStateForStoryBeat(storyBeat);

        if (currentState == null)
        {
            ApplySpriteOverride(null);
            SetVisible(true);
            SetCollidersEnabled(true);
            return;
        }

        currentState.ApplyLocation(transform);
        ApplySpriteOverride(currentState.SpriteOverride);
        SetVisible(currentState.IsVisible);
        SetCollidersEnabled(currentState.IsInteractable);
    }

    private NpcStoryState FindStateForStoryBeat(int storyBeat)
    {
        if (storyStates == null || storyStates.Length == 0)
        {
            return null;
        }

        NpcStoryState bestState = null;
        int bestBeat = int.MinValue;

        foreach (NpcStoryState state in storyStates)
        {
            if (state == null || state.MinimumStoryBeat > storyBeat || state.MinimumStoryBeat < bestBeat)
            {
                continue;
            }

            bestState = state;
            bestBeat = state.MinimumStoryBeat;
        }

        return bestState;
    }

    private void HandleDialogueComplete(NpcStoryState completedState)
    {
        if (completedState == null || !completedState.AdvancesStoryAfterDialogue)
        {
            return;
        }

        if (StoryProgress.Instance != null)
        {
            StoryProgress.Instance.AdvanceToStoryBeat(completedState.StoryBeatAfterDialogue);
        }
        else
        {
            Debug.LogWarning("no progress found in scene.");
        }
    }

    private void SetVisible(bool visible)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.enabled = visible;
        }
    }

    private void ApplySpriteOverride(Sprite spriteOverride)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = spriteOverride != null ? spriteOverride : defaultSprites[i];
        }
    }

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider2D npcCollider in colliders)
        {
            npcCollider.enabled = enabled;
        }
    }
}
