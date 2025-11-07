using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Refs")]
    public Ball ball;
    public Transform playerPaddle;
    public Transform aiPaddle;
    public Text leftScoreText;
    public Text rightScoreText;
    public GameOverMenu gameOverMenu;

    [Header("Rules")]
    public int pointsToWin = 11;
    public float serveDelay = 1.0f;

    int leftScore, rightScore;
    bool pendingServeToRight;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => UpdateUI();

    public void OnScore(int leftDelta, int rightDelta, GameObject _)
    {

        SFX.I?.PlayScore();

        leftScore += leftDelta;
        rightScore += rightDelta;
        UpdateUI();

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

        bool leftServingNext = rightDelta > 0;
        pendingServeToRight = leftServingNext;

        CancelInvoke(nameof(ServeInternal));
        Invoke(nameof(ServeInternal), serveDelay);
    }

    void ServeInternal() => ball.ResetBall(serveToRight: pendingServeToRight);


    void UpdateUI()
    {
        if (leftScoreText) leftScoreText.text = leftScore.ToString();
        if (rightScoreText) rightScoreText.text = rightScore.ToString();
    }

    // Expose scores for save/load
    public int LeftScore => leftScore;
    public int RightScore => rightScore;

    public void SetScores(int left, int right)
    {
        leftScore = Mathf.Max(0, left);
        rightScore = Mathf.Max(0, right);
        UpdateUI();
    }
}
