using UnityEngine;

public enum Side { None, Left, Right }

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    public Side LastHitter { get; private set; } = Side.None;
    public float startSpeed = 10f;
    public float speedIncreasePerHit = 0.5f;
    public float maxSpeed = 20f;
    public float randomBounceY = 0.35f;

    Rigidbody2D rb;
    Vector2 spawnPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        spawnPos = transform.position;
    }

    void OnEnable()
    {
        ResetBall(serveToRight: Random.value > 0.5f);
    }

    public void ResetBall(bool serveToRight)
    {
        transform.position = spawnPos;
        rb.linearVelocity = Vector2.zero;

        float vy = Random.Range(-0.6f, 0.6f);
        Vector2 dir = new Vector2(serveToRight ? 1f : -1f, vy).normalized;
        rb.linearVelocity = dir * startSpeed;

        SFX.I?.PlayServe();
    }

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

    float HitFactor(Vector2 ballPos, Vector2 paddlePos, float paddleHeight)
    {
        return Mathf.Clamp((ballPos.y - paddlePos.y) / (paddleHeight * 0.5f), -1f, 1f);
    }
}
