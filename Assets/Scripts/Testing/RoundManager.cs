using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Sistema de rondas que gestiona el spawn de enemigos
/// Cuando se completa el progreso, avanza a la siguiente ronda
/// </summary>
public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private PlayerPickupProgressUpdated playerProgress;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Configuración de Rondas")]
    [SerializeField] private int enemiesPerRound = 3;
    [SerializeField] private float enemySpawnMultiplier = 1.5f; // Multiplica enemigos cada ronda
    [SerializeField] private float delayBetweenRounds = 2f;

    [Header("Eventos")]
    public UnityEvent<int> onRoundStart = new UnityEvent<int>();
    public UnityEvent<int> onRoundComplete = new UnityEvent<int>();

    // Configuracion basada en rondas (implementada solo parcialmente)
    [SerializeField] private RoundConfig[] rounds;


    private int currentEnemyCount = 0;
    private int currentRound = 1;
    private bool isRoundActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (playerProgress == null)
        {
            playerProgress = FindFirstObjectByType<PlayerPickupProgressUpdated>();
        }

        if (enemySpawner == null)
        {
            enemySpawner = FindFirstObjectByType<EnemySpawner>();
        }

        if (playerProgress != null)
        {
            playerProgress.onProgressCompleted.AddListener(OnRoundCompleted);
        }

        StartNewRound();
    }

    /* private void StartNewRound()
    {
        if (isRoundActive)
            return;

        isRoundActive = true;
        currentRound = playerProgress.currentRound;
        
        // Calcular cantidad de enemigos para esta ronda
        currentEnemyCount = Mathf.RoundToInt(enemiesPerRound * Mathf.Pow(enemySpawnMultiplier, currentRound - 1));

        Debug.Log($"=== RONDA {currentRound} ===");
        Debug.Log($"Enemigos a spawnear: {currentEnemyCount}");

        onRoundStart?.Invoke(currentRound);

        // Spawnear enemigos
        if (enemySpawner != null)
        {
            enemySpawner.SpawnEnemiesForRound(currentEnemyCount);
        }
    } */

    private void StartNewRound()
    {
        if (isRoundActive)
            return;

        isRoundActive = true;

        int roundIndex = playerProgress.currentRound - 1;

        if (roundIndex >= rounds.Length)
        {
            Debug.Log("No hay más rondas configuradas");
            return;
        }

        RoundConfig config = rounds[roundIndex];

        currentRound = playerProgress.currentRound;
        currentEnemyCount = config.enemyCount;

        Debug.Log($"=== RONDA {currentRound} ===");
        Debug.Log($"Enemigos: {config.enemyCount}");

        // Configurar progreso
        playerProgress.ResetProgress(config.creditsRequired);

        // Spawn enemigos normales
        enemySpawner.SpawnEnemiesForRound(config.enemyCount);

        // Spawn boss si aplica
        if (config.hasBoss)
        {
            Debug.Log("Spawneando Boss para esta ronda");
            // enemySpawner.SpawnBoss(); // luego lo vemos *******************
        }

        onRoundStart?.Invoke(currentRound);
    }


    /*   private void OnRoundCompleted()
      {
          if (!isRoundActive)
              return;

          isRoundActive = false;

          Debug.Log($"Ronda {currentRound} completada!");
          onRoundComplete?.Invoke(currentRound);

          StartCoroutine(AdvanceToNextRound());
      } */


    // Esta funcion fue pensada para abrir la UI de power-ups, pero esto ya hacce de forma manual en el Inspector.
    private void OnRoundCompleted()
    {
        if (!isRoundActive)
            return;

        isRoundActive = false;

        Debug.Log("POWER-UP SELECTION ABIERTA");

        // Acá después abrirás tu UI
        // Cuando el jugador elija:


        // StartCoroutine(AdvanceToNextRound());
    }

    private IEnumerator AdvanceToNextRound()
    {
        yield return new WaitForSeconds(delayBetweenRounds);
        StartNewRound();
    }

    public int GetCurrentRound()
    {
        return currentRound;
    }

    public int GetCurrentEnemyCount()
    {
        return currentEnemyCount;
    }

    public void SetEnemySpawnMultiplier(float multiplier)
    {
        enemySpawnMultiplier = multiplier;
    }

    public void SetEnemiesPerRound(int count)
    {
        enemiesPerRound = count;
    }

    public void OnPowerUpSelected(int newCreditsRequired)
    {
        Debug.Log("Notificación recibida: power-up seleccionado");

        // Ocultar UI de power-ups. 
        // UIManager.Instance.HidePowerUpUI(); // si tienes un UIManager - Ya se hace externamente.

        // Reiniciar el progreso con el requisito de la ronda siguiente
        if (playerProgress != null)
        {
            playerProgress.ResetProgress(newCreditsRequired);
        }

        // Reanudar la siguiente ronda
        StartCoroutine(AdvanceToNextRound());
    }

}
