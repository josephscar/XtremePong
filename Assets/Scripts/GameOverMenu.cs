using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menuCanvas;   // Root of Game Over UI
    [SerializeField] private Button replayButton;     // Replay button
    [SerializeField] private Button quitButton;       // Quit button
    [SerializeField] private Text resultText;         // Optional: assign to show winner text

    public bool IsShown { get; private set; }

    void Awake()
    {
        if (menuCanvas) menuCanvas.SetActive(false);
        IsShown = false;
    }

    void OnEnable()
    {
        if (replayButton) replayButton.onClick.AddListener(OnClickReplay);
        if (quitButton) quitButton.onClick.AddListener(OnClickQuit);
    }

    void OnDisable()
    {
        if (replayButton) replayButton.onClick.RemoveListener(OnClickReplay);
        if (quitButton) quitButton.onClick.RemoveListener(OnClickQuit);
    }

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

    public void Hide()
    {
        IsShown = false;
        if (menuCanvas) menuCanvas.SetActive(false);
    }

    public void OnClickReplay()
    {
        // Unpause before reloading
        Time.timeScale = 1f;
        AudioListener.pause = false;
        var idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(idx);
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

