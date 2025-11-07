using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menuCanvas;       // Drag MenuCanvas here
    [SerializeField] private Button resumeButton;         // Drag ResumeButton here
    [SerializeField] private InputActionReference pauseAction; // Drag Pause action ref (Escape)

    private bool isPaused = false;

    void Awake()
    {
        if (menuCanvas)
            menuCanvas.SetActive(false); // Start hidden
    }

    void OnEnable()
    {
        if (pauseAction && pauseAction.action != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPause;
        }
    }

    void OnDisable()
    {
        if (pauseAction && pauseAction.action != null)
        {
            pauseAction.action.performed -= OnPause;
            pauseAction.action.Disable();
        }
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        TogglePause();
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (menuCanvas)
            menuCanvas.SetActive(isPaused);

        if (isPaused)
        {
            AudioListener.pause = true;

            // Ensure the first button is selected (for keyboard/gamepad)
            if (resumeButton && EventSystem.current)
                EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            AudioListener.pause = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        Debug.Log($"Pause toggled: {(isPaused ? "Paused" : "Resumed")}");
    }

    // UI Button Hooks
    public void OnClickResume()
    {
        if (!isPaused) return;
        TogglePause();
    }

    public void OnClickQuit()
    {
        Debug.Log("Quit button pressed.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
