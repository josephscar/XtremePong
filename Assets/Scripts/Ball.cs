using UnityEngine;

/// <summary>Identifies which side last hit the ball.</summary>
public enum Side { None, Left, Right }

/// <summary>
/// Controls Pong ball physics and interactions.
/// - Spawns and serves with randomized vertical bias
/// - Bounces off paddles with basic aim based on contact point
/// - Increases speed with each paddle hit up to a cap
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    /// <summary>Which side last touched the ball.</summary>
    public Side LastHitter { get; private set; } = Side.None;
    /// <summary>Initial speed when served.</summary>
    public float startSpeed = 10f;
    /// <summary>Speed added after each paddle hit.</summary>
    public float speedIncreasePerHit = 0.5f;
    /// <summary>Maximum allowed speed.</summary>
    public float maxSpeed = 20f;
    /// <summary>Random Y variation added to paddle bounce to avoid stale rallies.</summary>
    public float randomBounceY = 0.35f;

    Rigidbody2D rb;
    Vector2 spawnPos;

    /// <summary>Cache Rigidbody2D and initial spawn position.</summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        spawnPos = transform.position;
    }

    /// <summary>Auto-serve when enabled (eg. scene load).</summary>
    void OnEnable()
    {
        ResetBall(serveToRight: Random.value > 0.5f);
    }

    /// <summary>
    /// Reset to spawn and launch in the specified horizontal direction.
    /// </summary>
    public void ResetBall(bool serveToRight)
    {
        transform.position = spawnPos;
        rb.linearVelocity = Vector2.zero;

        float vy = Random.Range(-0.6f, 0.6f);
        Vector2 dir = new Vector2(serveToRight ? 1f : -1f, vy).normalized;
        rb.linearVelocity = dir * startSpeed;

        SFX.I?.PlayServe();
    }

    /// <summary>
    /// Handle paddle collisions to produce aimed bounces and SFX.
    /// </summary>
    void OnCollisionEnter2D(Collision2D col)
    {
        // --- Identify hitter ---
        if (col.collider.CompareTag("PlayerPaddle"))    LastHitter = Side.Left;
        else if (col.collider.CompareTag("AIPaddle"))   LastHitter = Side.Right;

        // --- Paddle bounce physics ---
        if (col.collider.CompareTag("PlayerPaddle") || col.collider.CompareTag("AIPaddle"))
        {
            float y = HitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);

            Vector2 v = rb.linearVelocity;
            v.y = y + Random.Range(-randomBounceY, randomBounceY);
            v.x = Mathf.Sign(v.x) * Mathf.Max(Mathf.Abs(v.x), 0.6f);

            v = v.normalized * Mathf.Min(rb.linearVelocity.magnitude + speedIncreasePerHit, maxSpeed);
            rb.linearVelocity = v;

            // --- Audio ---
            float speed = rb.linearVelocity.magnitude;
            float intensity01 = Mathf.InverseLerp(2f, 14f, speed);
            SFX.I?.PlayHit(intensity01);

        }
    }

    /// <summary>
    /// Returns a normalized hit factor [-1..1] based on vertical offset from paddle center.
    /// </summary>
    float HitFactor(Vector2 ballPos, Vector2 paddlePos, float paddleHeight)
    {
        return Mathf.Clamp((ballPos.y - paddlePos.y) / (paddleHeight * 0.5f), -1f, 1f);
    }
}
