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
        if (isPaused) return; // ignore input while paused
        moveInput = ctx.ReadValue<Vector2>();
    }

    // Pause input is handled centrally by PauseManager.

    private void TogglePause()
    {
        // Keep for fallback only; PauseManager now controls timeScale
        isPaused = !isPaused;
        if (isPaused)
        {
            moveInput = Vector2.zero;
            Debug.Log("Game Paused (local)");
        }
        else
        {
            Debug.Log("Game Resumed (local)");
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

    // Allow external controllers (PauseManager) to set pause state
    public void SetPaused(bool paused)
    {
        isPaused = paused;
        if (paused)
        {
            moveInput = Vector2.zero;
        }
    }
}
