using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
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
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Paddle"))
        {
            float y = HitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);

            Vector2 v = rb.linearVelocity;
            v.y = y + Random.Range(-randomBounceY, randomBounceY);
            v.x = Mathf.Sign(v.x) * Mathf.Max(Mathf.Abs(v.x), 0.6f);

            v = v.normalized * Mathf.Min(rb.linearVelocity.magnitude + speedIncreasePerHit, maxSpeed);
            rb.linearVelocity = v;
        }
    }

    float HitFactor(Vector2 ballPos, Vector2 paddlePos, float paddleHeight)
    {
        return Mathf.Clamp((ballPos.y - paddlePos.y) / (paddleHeight * 0.5f), -1f, 1f);
    }
}
