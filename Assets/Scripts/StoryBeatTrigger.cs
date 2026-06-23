using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StoryBeatTrigger : MonoBehaviour
{
    [SerializeField] private int storyBeat;
    [SerializeField] private bool onlyAdvance = true;
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered;

    private void Reset()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce)
        {
            return;
        }

        if (other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        if (StoryProgress.Instance == null)
        {
            Debug.LogWarning("no progress found in scene.");
            return;
        }

        hasTriggered = true;

        if (onlyAdvance)
        {
            StoryProgress.Instance.AdvanceToStoryBeat(storyBeat);
        }
        else
        {
            StoryProgress.Instance.SetStoryBeat(storyBeat);
        }
    }
}
