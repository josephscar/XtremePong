using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool isLeftGoal; // true = left wall behind Player (AI scores), false = right wall (Player scores)

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.attachedRigidbody) return;
        if (!other.CompareTag("Ball")) return;

        int leftDelta = isLeftGoal ? 0 : 1;    // ball into right goal -> left scores +1
        int rightDelta = isLeftGoal ? 1 : 0;   // ball into left goal  -> right scores +1

        GameManager.Instance.OnScore(leftDelta, rightDelta, other.attachedRigidbody.gameObject);
    }
}
