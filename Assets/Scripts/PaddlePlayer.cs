using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddlePlayerInput : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference moveAction;   // assign in Inspector
    public InputActionReference pauseAction;  // assign in Inspector (Esc/Start)

    [Header("Movement")]
    public float moveSpeed = 12f;
    public float clampY = 4.2f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isPaused;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
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

        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPause;
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

        if (pauseAction != null && pauseAction.action != null)
        {
            pauseAction.action.performed -= OnPause;
            pauseAction.action.Disable();
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (isPaused) return; // ignore input while paused
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        TogglePause();
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // freeze physics & movement
            moveInput = Vector2.zero;
            Debug.Log("Game Paused");
        }
        else
        {
            Time.timeScale = 1f; // resume
            Debug.Log("Game Resumed");
        }
    }

    void FixedUpdate()
    {
        if (isPaused) return;

        var desired = new Vector2(0f, moveInput.y * moveSpeed);
        rb.linearVelocity = desired;

        var next = rb.position + desired * Time.fixedDeltaTime;
        next.y = Mathf.Clamp(next.y, -clampY, clampY);
        rb.MovePosition(next);
    }
}
