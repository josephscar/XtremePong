using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Central match coordinator.
/// - Tracks scores for left and right players
/// - Serves the ball with a delay after a goal
/// - Updates UI score text
/// - Detects match end and shows the Game Over menu (if assigned)
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Refs")]
    /// <summary>Reference to the active ball.</summary>
    public Ball ball;
    /// <summary>Left/player paddle transform (optional, not required by logic).</summary>
    public Transform playerPaddle;
    /// <summary>Right/AI paddle transform (optional, not required by logic).</summary>
    public Transform aiPaddle;
    /// <summary>Score UI for the left side.</summary>
    public Text leftScoreText;
    /// <summary>Score UI for the right side.</summary>
    public Text rightScoreText;
    /// <summary>Optional Game Over menu to show when someone reaches pointsToWin.</summary>
    public GameOverMenu gameOverMenu;

    [Header("Rules")]
    /// <summary>Score required to win the match.</summary>
    public int pointsToWin = 11;
    /// <summary>Delay between a goal and the next serve.</summary>
    public float serveDelay = 1.0f;

    int leftScore, rightScore;
    bool pendingServeToRight;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>Initialize UI at start.</summary>
    void Start() => UpdateUI();

    /// <summary>
    /// Called by Goal when the ball enters a goal trigger.
    /// Updates scores, checks for match end, and schedules the next serve.
    /// </summary>
    /// <param name="leftDelta">Points to add to left player.</param>
    /// <param name="rightDelta">Points to add to right player.</param>
    /// <param name="_">Reserved (ball GameObject, not used).</param>
    public void OnScore(int leftDelta, int rightDelta, GameObject _)
    {

        SFX.I?.PlayScore();

        leftScore += leftDelta;
        rightScore += rightDelta;
        UpdateUI();

        // Check match end first; stop further serves when over.
        bool leftWon = leftScore >= pointsToWin;
        bool rightWon = rightScore >= pointsToWin;

        if (leftWon || rightWon)
        {
            // Game Over flow
            string winner = leftWon ? "Left Wins!" : "Right Wins!";
            SFX.I?.PlayGameEnd();

            if (gameOverMenu)
            {
                gameOverMenu.Show(winner);
            }
            else
            {
                // Fallback: pause game if menu not wired yet
                Time.timeScale = 0f;
            }

            // Stop any pending serves
            CancelInvoke(nameof(ServeInternal));
            return;
        }

        // If not game over, schedule next serve towards the side that conceded.
        bool leftServingNext = rightDelta > 0;
        pendingServeToRight = leftServingNext;

        CancelInvoke(nameof(ServeInternal));
        Invoke(nameof(ServeInternal), serveDelay);
    }

    /// <summary>
    /// Performs the actual ball reset/serve after a delay.
    /// </summary>
    void ServeInternal() => ball.ResetBall(serveToRight: pendingServeToRight);


    /// <summary>
    /// Updates both score text fields.
    /// </summary>
    void UpdateUI()
    {
        if (leftScoreText) leftScoreText.text = leftScore.ToString();
        if (rightScoreText) rightScoreText.text = rightScore.ToString();
    }

    // Expose scores for save/load
    public int LeftScore => leftScore;
    public int RightScore => rightScore;

    /// <summary>
    /// Sets scores directly and refreshes the UI. Use when loading saves.
    /// </summary>
    public void SetScores(int left, int right)
    {
        leftScore = Mathf.Max(0, left);
        rightScore = Mathf.Max(0, right);
        UpdateUI();
    }
}
