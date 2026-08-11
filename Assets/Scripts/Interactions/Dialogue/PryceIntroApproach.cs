using System.Collections;
using UnityEngine;

public class PryceIntroApproach : MonoBehaviour
{
    [SerializeField] private NpcDialogue pryceDialogue;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float interactionRange = 1.2f;
    [SerializeField] private AnimationClip walkingAnimation;
    [SerializeField] private AnimationClip idleAnimation;

    private bool hasStarted;

    private void Start()
    {
        if (pryceDialogue == null)
        {
            pryceDialogue = GetComponent<NpcDialogue>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        PlayAnimation(idleAnimation);

        PlayerController player = FindAnyObjectByType<PlayerController>();

        if (player != null && pryceDialogue != null)
        {
            StartCoroutine(ApproachPlayer(player));
        }
        else
        {
            Debug.LogWarning("Pryce intro approach is missing the player or Pryce dialogue component.");
        }
    }

    private IEnumerator ApproachPlayer(PlayerController player)
    {
        if (hasStarted)
        {
            yield break;
        }

        hasStarted = true;
        player.enabled = false;

        PlayAnimation(walkingAnimation);

        yield return null;

        while (Vector2.Distance(transform.position, player.transform.position) > interactionRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        PlayAnimation(idleAnimation);

        player.FaceToward(transform.position);
        pryceDialogue.FaceToward(player.transform.position);

        player.enabled = true;
        pryceDialogue.Interact();
    }

    private void PlayAnimation(AnimationClip animationClip)
    {
        if (animator == null || animationClip == null)
        {
            return;
        }

        animator.Play(animationClip.name, 0, 0f);
    }
}
