using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        SetDefaultFacingDirection();

        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void Update()
    {
        if (TextPanel.IsAnyPanelOpen
            || DialoguePanel.IsAnyDialogueOpen
            || PersonnelFilePanel.IsAnyPersonnelFileOpen
            || LogbookPanel.IsAnyLogbookOpen
            || EndingChoicePanel.IsAnyEndingChoiceOpen
            || EndingSequence.IsEndingSequencePlaying)
        {
            moveInput = Vector2.zero;
            UpdateAnimator();
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        Vector2 animationDirection = Vector2.zero;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(moveInput.y) > 0.01f)
            {
                animationDirection = new Vector2(0f, Mathf.Sign(moveInput.y));
            }
            else
            {
                animationDirection = new Vector2(Mathf.Sign(moveInput.x), 0f);
            }

            animator.SetFloat("LastMoveX", animationDirection.x);
            animator.SetFloat("LastMoveY", animationDirection.y);
        }

        animator.SetFloat("MoveX", animationDirection.x);
        animator.SetFloat("MoveY", animationDirection.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    private void SetDefaultFacingDirection()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat("LastMoveX", 0f);
        animator.SetFloat("LastMoveY", -1f);
    }
}
