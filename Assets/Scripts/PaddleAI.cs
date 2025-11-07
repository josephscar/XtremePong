using UnityEngine;

/// <summary>
/// Basic AI controller for the right-side paddle.
/// - Tracks the ball with configurable responsiveness
/// - Optionally only moves when the ball is approaching
/// - Moves a kinematic Rigidbody2D within vertical bounds
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PaddleAI : MonoBehaviour
{
    [Header("Tracking")]
    /// <summary>Ball transform to track (assign via inspector).</summary>
    public Transform ball;
    /// <summary>Base vertical movement speed.</summary>
    public float moveSpeed = 11f;
    /// <summary>Vertical clamp extent (half-height of play area).</summary>
    public float clampY = 4.2f;
    /// <summary>Scales reaction aggressiveness (wired to difficulty slider).</summary>
    [Range(0.1f, 1.2f)] public float responsiveness = 0.9f;
    /// <summary>If true, AI pauses when the ball is moving away.</summary>
    public bool trackOnlyWhenApproaching = true;

    Rigidbody2D rb;
    Rigidbody2D ballRb;

    /// <summary>Configure Rigidbody2D for kinematic movement.</summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    /// <summary>Cache the ball's Rigidbody2D if available.</summary>
    void Start()
    {
        if (ball) ballRb = ball.GetComponent<Rigidbody2D>();
    }

    /// <summary>Simple tracking logic, executed each physics step.</summary>
    void FixedUpdate()
    {
        if (!ball)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 desired = Vector2.zero;

        // Only track when the ball is heading toward this paddle (assumes AI on right).
        bool ballInfoValid = ballRb != null;
        bool ballMovingLeft = ballInfoValid && ballRb.linearVelocity.x < 0f;
        bool shouldHold = trackOnlyWhenApproaching && ballMovingLeft; // stop if moving away

        if (!shouldHold)
        {
            float dir = Mathf.Sign(ball.position.y - transform.position.y);
            desired = new Vector2(0f, dir * moveSpeed * responsiveness);
        }

        rb.linearVelocity = desired; // Unity 6 API

        var next = rb.position + desired * Time.fixedDeltaTime;
        next.y = Mathf.Clamp(next.y, -clampY, clampY);
        rb.MovePosition(next);
    }

    /// <summary>
    /// Difficulty hook: expects 0..1 from UI slider and maps to responsiveness.
    /// </summary>
    public void SetDifficulty(float t)
    {
        // Map slider [0..1] to responsiveness range
        responsiveness = Mathf.Lerp(0.4f, 1.2f, Mathf.Clamp01(t));
    }
}
