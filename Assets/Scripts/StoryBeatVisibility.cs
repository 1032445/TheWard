using UnityEngine;

public class StoryBeatVisibility : MonoBehaviour
{
    [SerializeField] private int minimumStoryBeatToShow = 5;
    [SerializeField] private int maximumStoryBeatToShow = -1;
    [SerializeField] private bool includeInactiveChildren = true;

    private Renderer[] renderers;
    private Collider2D[] colliders;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(includeInactiveChildren);
        colliders = GetComponentsInChildren<Collider2D>(includeInactiveChildren);
    }

    private void OnEnable()
    {
        StoryProgress.StoryBeatChanged += HandleStoryBeatChanged;
        ApplyVisibility();
    }

    private void OnDisable()
    {
        StoryProgress.StoryBeatChanged -= HandleStoryBeatChanged;
    }

    private void Start()
    {
        ApplyVisibility();
    }

    private void HandleStoryBeatChanged(int storyBeat)
    {
        ApplyVisibility(storyBeat);
    }

    private void ApplyVisibility()
    {
        int storyBeat = StoryProgress.Instance != null ? StoryProgress.Instance.CurrentStoryBeat : 0;
        ApplyVisibility(storyBeat);
    }

    private void ApplyVisibility(int storyBeat)
    {
        bool shouldShow = storyBeat >= minimumStoryBeatToShow
            && (maximumStoryBeatToShow < 0 || storyBeat <= maximumStoryBeatToShow);

        foreach (Renderer targetRenderer in renderers)
        {
            if (targetRenderer != null)
            {
                targetRenderer.enabled = shouldShow;
            }
        }

        foreach (Collider2D targetCollider in colliders)
        {
            if (targetCollider != null)
            {
                targetCollider.enabled = shouldShow;
            }
        }
    }
}
