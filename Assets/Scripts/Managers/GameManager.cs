using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Managers")]
    [SerializeField] private UIScreenManager uiScreenManager;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    private const string GAMEPLAY_VOLUME = "GameplayVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PlayerHealth_.OnPlayerDeath += HandleGameOver;
    }

    private void OnDisable()
    {
        PlayerHealth_.OnPlayerDeath -= HandleGameOver;
    }

    private void Start()
    {
        ResumeGame();
        uiScreenManager.ShowGameplay();
    }

    // =====================
    // GAME FLOW
    // =====================

    private void HandleGameOver()
    {
        // uiScreenManager.ShowGameOver(); // No quiero mostrar la pantalla de game over automaticamente aqui
        //PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        audioMixer.SetFloat(GAMEPLAY_VOLUME, -80f);
        //uiScreenManager.ShowPause();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        audioMixer.SetFloat(GAMEPLAY_VOLUME, 0f);
        uiScreenManager.ShowGameplay();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
