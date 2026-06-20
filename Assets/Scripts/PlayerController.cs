using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

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
        if (TextPanel.IsAnyPanelOpen || DialoguePanel.IsAnyDialogueOpen)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}