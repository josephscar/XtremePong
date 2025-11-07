using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    [Header("References")]
    [SerializeField] private GameObject menuCanvas;       // Drag MenuCanvas here
    [SerializeField] private Button resumeButton;         // Drag ResumeButton here
    [SerializeField] private InputActionReference pauseAction; // Drag Pause action ref (Escape)
    [SerializeField] private Button saveButton;           // Drag Save button here
    [SerializeField] private Slider volumeSlider;         // Drag Volume slider here
    [SerializeField] private Slider difficultySlider;     // Drag Difficulty slider here (0..1)
    [SerializeField] private PaddleAI ai;                 // Drag AI paddle here (or will auto-find)

    private bool isPaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (menuCanvas)
            menuCanvas.SetActive(false); // Start hidden

        // Auto-find AI if not assigned
        if (!ai)
            ai = FindObjectOfType<PaddleAI>();

        // Load persisted settings (if any)
        if (PlayerPrefs.HasKey("master_volume"))
        {
            float v = Mathf.Clamp01(PlayerPrefs.GetFloat("master_volume", 1f));
            AudioListener.volume = v;
            if (SFX.I) SFX.I.masterVolume = v;
        }
        if (PlayerPrefs.HasKey("ai_difficulty"))
        {
            float t = Mathf.Clamp01(PlayerPrefs.GetFloat("ai_difficulty", 0.5f));
            if (ai) ai.SetDifficulty(t);
        }

        // Initialize Volume slider from current state
        if (volumeSlider)
        {
            float current = 1f;
            if (SFX.I) current = SFX.I.masterVolume;
            else current = AudioListener.volume;

            volumeSlider.SetValueWithoutNotify(Mathf.Clamp01(current));
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Initialize Difficulty slider from AI if present
        if (difficultySlider)
        {
            float t = 0.5f;
            if (ai)
            {
                // reverse-map responsiveness (~0.4..1.2) back to 0..1
                t = Mathf.InverseLerp(0.4f, 1.2f, ai.responsiveness);
            }
            difficultySlider.SetValueWithoutNotify(Mathf.Clamp01(t));
            difficultySlider.onValueChanged.AddListener(OnDifficultyChanged);
        }
    }

    void OnEnable()
    {
        if (pauseAction && pauseAction.action != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPause;
        }

        if (saveButton)
            saveButton.onClick.AddListener(OnClickSave);
    }

    void OnDisable()
    {
        if (pauseAction && pauseAction.action != null)
        {
            pauseAction.action.performed -= OnPause;
            pauseAction.action.Disable();
        }

        if (saveButton)
            saveButton.onClick.RemoveListener(OnClickSave);

        if (volumeSlider)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);

        if (difficultySlider)
            difficultySlider.onValueChanged.RemoveListener(OnDifficultyChanged);
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        TogglePause();
    }

    public void TogglePause()
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

        // Propagate pause state to all player inputs
        var players = FindObjectsOfType<PaddlePlayerInput>(true);
        foreach (var p in players)
            p.SetPaused(isPaused);
    }

    // UI Button Hooks
    public void OnClickResume()
    {
        if (!isPaused) return;
        TogglePause();
    }

    public void OnClickSave()
    {
        var gm = GameManager.Instance;
        var data = new GameSave
        {
            leftScore = gm ? gm.LeftScore : 0,
            rightScore = gm ? gm.RightScore : 0,
            masterVolume = SFX.I ? SFX.I.masterVolume : AudioListener.volume,
            aiResponsiveness = ai ? ai.responsiveness : 0.9f,
        };
        SaveSystem.Save(data);
    }

    public void OnVolumeChanged(float value)
    {
        value = Mathf.Clamp01(value);
        AudioListener.volume = value;
        if (SFX.I) SFX.I.masterVolume = value;
        PlayerPrefs.SetFloat("master_volume", value);
    }

    public void OnDifficultyChanged(float value)
    {
        if (!ai) ai = FindObjectOfType<PaddleAI>();
        if (ai)
        {
            ai.SetDifficulty(Mathf.Clamp01(value));
            PlayerPrefs.SetFloat("ai_difficulty", Mathf.Clamp01(value));
        }
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
