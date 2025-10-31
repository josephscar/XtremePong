using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddlePlayerInput : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Drag the Move action from your Input Actions asset (Value/Vector2).")]
    public InputActionReference moveAction; // assign in Inspector

    [Header("Movement")]
    public float moveSpeed = 12f;
    public float clampY = 4.2f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // we control motion
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    void OnEnable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.Enable();
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
        }
        else
        {
            Debug.LogError("PaddlePlayerInput: Move ActionReference not assigned.");
        }
    }

    void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
            moveAction.action.Disable();
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>(); // we only use Y
    }

    void FixedUpdate()
    {
        // Drive kinematic Rigidbody2D via MovePosition using desired velocity.
        var desired = new Vector2(0f, moveInput.y * moveSpeed);
        rb.linearVelocity = desired; // Unity 6 API

        var next = rb.position + desired * Time.fixedDeltaTime;
        next.y = Mathf.Clamp(next.y, -clampY, clampY);
        rb.MovePosition(next);
    }
}
