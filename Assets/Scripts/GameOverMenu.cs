using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the Game Over UI and actions.
/// - Shows winner label (optional)
/// - Pauses the game and enables cursor/UI selection
/// - Provides Replay and Quit actions
/// </summary>
public class GameOverMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menuCanvas;   // Root of Game Over UI
    [SerializeField] private Button replayButton;     // Replay button
    [SerializeField] private Button quitButton;       // Quit button
    [SerializeField] private Text resultText;         // Optional: assign to show winner text

    public bool IsShown { get; private set; }

    /// <summary>Ensure menu starts hidden.</summary>
    void Awake()
    {
        if (menuCanvas) menuCanvas.SetActive(false);
        IsShown = false;
    }

    /// <summary>Register UI button listeners.</summary>
    void OnEnable()
    {
        if (replayButton) replayButton.onClick.AddListener(OnClickReplay);
        if (quitButton) quitButton.onClick.AddListener(OnClickQuit);
    }

    /// <summary>Unregister UI button listeners.</summary>
    void OnDisable()
    {
        if (replayButton) replayButton.onClick.RemoveListener(OnClickReplay);
        if (quitButton) quitButton.onClick.RemoveListener(OnClickQuit);
    }

    /// <summary>
    /// Show the menu (and pause the game). Optionally provide a winner label.
    /// </summary>
    public void Show(string winnerLabel = null)
    {
        IsShown = true;
        if (menuCanvas) menuCanvas.SetActive(true);

        if (!string.IsNullOrEmpty(winnerLabel) && resultText)
            resultText.text = winnerLabel;

        // Pause game and audio
        Time.timeScale = 0f;
        AudioListener.pause = true;

        // Make cursor usable
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Select default button for keyboard/gamepad
        if (replayButton && EventSystem.current)
            EventSystem.current.SetSelectedGameObject(replayButton.gameObject);
    }

    /// <summary>Hide the menu (does not change time scale).</summary>
    public void Hide()
    {
        IsShown = false;
        if (menuCanvas) menuCanvas.SetActive(false);
    }

    /// <summary>Replay button: reloads the current active scene.</summary>
    public void OnClickReplay()
    {
        // Unpause before reloading
        Time.timeScale = 1f;
        AudioListener.pause = false;
        var idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(idx);
    }

    /// <summary>Quit button: exits the application (or stops play in Editor).</summary>
    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
