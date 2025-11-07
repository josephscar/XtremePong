using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles human player's paddle input using the new Input System.
/// - Reads vertical movement input and moves a kinematic Rigidbody2D
/// - Can be paused externally via PauseManager.SetPaused
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PaddlePlayerInput : MonoBehaviour
{
    [Header("Input")]
    /// <summary>Assigned in Inspector: movement action (expects Vector2).</summary>
    public InputActionReference moveAction;   // assign in Inspector
    /// <summary>Deprecated here: Pause handled by PauseManager.</summary>
    public InputActionReference pauseAction;  // assign in Inspector (Esc/Start)

    [Header("Movement")]
    public float moveSpeed = 12f;
    public float clampY = 4.2f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isPaused;

    /// <summary>Cache and configure Rigidbody2D for kinematic movement.</summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    /// <summary>Register input callbacks.</summary>
    void OnEnable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.Enable();
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
        }
    }

    /// <summary>Unregister input callbacks.</summary>
    void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
            moveAction.action.Disable();
        }
    }

    /// <summary>
    /// Reads the input vector and stores the Y component for vertical motion.
    /// Ignored when paused.
    /// </summary>
    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (isPaused) return; // ignore input while paused
        moveInput = ctx.ReadValue<Vector2>();
    }

    // Pause input is handled centrally by PauseManager.

    /// <summary>
    /// Fallback pause toggle (used only if PauseManager is absent).
    /// </summary>
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

    /// <summary>
    /// Applies kinematic movement each physics step, clamped to bounds.
    /// </summary>
    void FixedUpdate()
    {
        if (isPaused) return;

        var desired = new Vector2(0f, moveInput.y * moveSpeed);
        rb.linearVelocity = desired;

        var next = rb.position + desired * Time.fixedDeltaTime;
        next.y = Mathf.Clamp(next.y, -clampY, clampY);
        rb.MovePosition(next);
    }

    /// <summary>Allow PauseManager (or other systems) to set pause state.</summary>
    public void SetPaused(bool paused)
    {
        isPaused = paused;
        if (paused)
        {
            moveInput = Vector2.zero;
        }
    }
}
