using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardPickup : MonoBehaviour
{
    public CardData card;

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("Collision");
        if (!other.CompareTag("Ball")) return;

        var rb = other.attachedRigidbody;
        var ball = rb ? rb.GetComponent<Ball>() : null;
        if (!ball) return;

        Debug.Log($"Pickup triggered by ball. LastHitter = {ball.LastHitter}");

        var targetHand = FindHandFor(ball.LastHitter);
        if (targetHand != null && targetHand.TryAdd(card))
        {
            Debug.Log("Collected.");
            Destroy(gameObject); // collected!
        }
        // else: optional “hand full” feedback
    }

    PlayerHand FindHandFor(Side side)
    {
        foreach (var hand in FindObjectsOfType<PlayerHand>())
            if (hand.owner == side) return hand;
        return null;
    }
}
