using System.Collections;
using UnityEngine;

public class CardEffectSystem : MonoBehaviour
{
    public static CardEffectSystem Instance { get; private set; }

    [Header("Scene Refs")]
    public Ball ball;                      // assign in Inspector (or auto-find)
    public Transform leftPaddle;           // assign your left paddle transform
    public Transform rightPaddle;          // assign your right paddle transform
    public GameObject shieldWallPrefab;    // thin blocker with BoxCollider2D

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (!ball) ball = FindObjectOfType<Ball>();
    }

    public void Apply(Side owner, CardData card)
    {
        switch (card.kind)
        {
            case CardKind.SpeedBoost: ApplySpeedBoost(card.value); break;
            case CardKind.ShieldWall: StartCoroutine(SpawnShield(owner, card.duration)); break;
            case CardKind.TimeSlow:   StartCoroutine(TimeSlow(card.value <= 0 ? 0.6f : card.value, card.duration)); break;
        }
    }

    void ApplySpeedBoost(float mult)
    {
        Debug.Log("Used Speed Boost");
        if (!ball) return;
        var rb = ball.GetComponent<Rigidbody2D>();
        var v  = rb.linearVelocity;
        if (v.sqrMagnitude < 0.01f) return;
        rb.linearVelocity = v * Mathf.Max(1f, mult);
    }

    IEnumerator SpawnShield(Side owner, float duration)
    {
        if (!shieldWallPrefab) yield break;
        var anchor = owner == Side.Left ? leftPaddle : rightPaddle;
        if (!anchor) yield break;

        var offset = owner == Side.Left ? new Vector3(-0.6f, 0f, 0f) : new Vector3(0.6f, 0f, 0f);
        var go = Instantiate(shieldWallPrefab, anchor.position + offset, Quaternion.identity);
        yield return new WaitForSeconds(duration);
        if (go) Destroy(go);
    }

    IEnumerator TimeSlow(float scale, float duration)
    {
        var prev = Time.timeScale;
        Time.timeScale = Mathf.Clamp(scale, 0.1f, 1f);
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = prev;
    }
}
