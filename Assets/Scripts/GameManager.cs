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
        leftScore += leftDelta;
        rightScore += rightDelta;
        UpdateUI();

        bool leftServingNext = rightDelta > 0;
        pendingServeToRight = leftServingNext;

        CancelInvoke(nameof(ServeInternal));
        Invoke(nameof(ServeInternal), serveDelay);

        if (leftScore >= pointsToWin || rightScore >= pointsToWin)
        {
            leftScore = 0; rightScore = 0; UpdateUI();
        }
    }

    void ServeInternal() => ball.ResetBall(serveToRight: pendingServeToRight);

    void UpdateUI()
    {
        if (leftScoreText) leftScoreText.text = leftScore.ToString();
        if (rightScoreText) rightScoreText.text = rightScore.ToString();
    }
}
