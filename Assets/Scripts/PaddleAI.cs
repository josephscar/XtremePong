using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PaddleAI : MonoBehaviour
{
    [Header("Tracking")]
    public Transform ball;
    public float moveSpeed = 11f;
    public float clampY = 4.2f;
    [Range(0.1f, 1.2f)] public float responsiveness = 0.9f;
    public bool trackOnlyWhenApproaching = true;

    Rigidbody2D rb;
    Rigidbody2D ballRb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    void Start()
    {
        if (ball) ballRb = ball.GetComponent<Rigidbody2D>();
    }

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
}
