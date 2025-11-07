using UnityEngine;

/// <summary>
/// Goal trigger volume for one side of the arena.
/// When the ball enters, awards a point to the opposite side
/// and notifies the GameManager.
/// </summary>
public class Goal : MonoBehaviour
{
    /// <summary>
    /// True if this is the left goal (behind the player). If true, the right side scores.
    /// False means this is the right goal (behind AI), so the left side scores.
    /// </summary>
    public bool isLeftGoal; // true = left wall behind Player (AI scores), false = right wall (Player scores)

    /// <summary>
    /// Detect ball entry and compute score deltas for each side.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.attachedRigidbody) return;
        if (!other.CompareTag("Ball")) return;

        int leftDelta = isLeftGoal ? 0 : 1;    // ball into right goal -> left scores +1
        int rightDelta = isLeftGoal ? 1 : 0;   // ball into left goal  -> right scores +1

        GameManager.Instance.OnScore(leftDelta, rightDelta, other.attachedRigidbody.gameObject);
    }
}
