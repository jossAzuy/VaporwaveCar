using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Sistema centralizado de pausa
/// Controla Time.timeScale, audio y UI
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Estado")]
    private bool isPaused = false;
    public bool IsPaused => isPaused;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string volumeParameterName = "GameplayVolume";
    [SerializeField] private float pausedVolume = -80f;
    [SerializeField] private float normalVolume = 0f;

    [Header("UI")]
    [SerializeField] private UIScreenManager uiScreenManager;

    [Header("Controles")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private bool canPause = true;

    [Header("Eventos")]
    public UnityEvent onPause = new UnityEvent();
    public UnityEvent onResume = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Asegurar que el objeto persista entre escenas si es necesario 
           DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey) && canPause)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    /// <summary>
    /// Pausa el juego
    /// </summary>
    public void Pause()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        // Ajustar audio
        if (audioMixer != null)
        {
            audioMixer.SetFloat(volumeParameterName, pausedVolume);
        }

        // Mostrar UI de pausa
        if (uiScreenManager != null)
        {
            uiScreenManager.ShowPause();
        }

        Debug.Log("Juego pausado");
        onPause?.Invoke();
    }

    /// <summary>
    /// Reanuda el juego
    /// </summary>
    public void Resume()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        // Restaurar audio
        if (audioMixer != null)
        {
            audioMixer.SetFloat(volumeParameterName, normalVolume);
        }

        // Mostrar UI de juego
        if (uiScreenManager != null)
        {
            uiScreenManager.ShowGameplay();
        }

        Debug.Log("Juego reanudado");
        onResume?.Invoke();
    }

    /// <summary>
    /// Alterna entre pausa y reanudación
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    /// <summary>
    /// Habilita o deshabilita la capacidad de pausar
    /// </summary>
    public void SetPauseEnabled(bool enabled)
    {
        canPause = enabled;
    }

    public void GoToMainMenu(string sceneName)
    {
        // Resetear estado global
        isPaused = false;
        Time.timeScale = 1f;

        if (audioMixer != null)
        {
            audioMixer.SetFloat(volumeParameterName, normalVolume);
        }

        SceneManager.LoadScene(sceneName);
    }
}
