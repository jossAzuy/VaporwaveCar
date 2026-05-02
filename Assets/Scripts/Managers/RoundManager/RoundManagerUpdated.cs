using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class RoundManagerUpdated : MonoBehaviour
{
    public static RoundManagerUpdated Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private PlayerPickupProgressUpdated playerProgress;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Configuración de Rondas")]
    [SerializeField] private RoundConfig[] rounds;
    [SerializeField] private float delayBetweenRounds = 2f;

    [Header("Eventos")]
    public UnityEvent<int> onRoundStart = new UnityEvent<int>();
    public UnityEvent<int> onRoundComplete = new UnityEvent<int>();

    private bool isRoundActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Auto-referencias de seguridad
        if (playerProgress == null)
            playerProgress = FindFirstObjectByType<PlayerPickupProgressUpdated>();

        if (enemySpawner == null)
            enemySpawner = FindFirstObjectByType<EnemySpawner>();

        // Escuchamos cuando el progreso se completa
        if (playerProgress != null)
            playerProgress.onProgressCompleted.AddListener(OnRoundCompleted);

        // Inicia la primera ronda
        StartNewRound();
    }

    // Inicia la ronda actual según RoundConfig
    private void StartNewRound()
    {
        if (isRoundActive) return;

        int roundIndex = playerProgress.currentRound - 1;

        if (roundIndex >= rounds.Length)
        {
            Debug.Log("No hay más rondas configuradas");
            return;
        }

        isRoundActive = true;
        RoundConfig config = rounds[roundIndex];

        Debug.Log($"=== RONDA {playerProgress.currentRound} ===");
        Debug.Log($"Enemigos: {config.enemyCount}");
        Debug.Log($"Créditos requeridos: {config.creditsRequired}");

        // CAMBIO IMPORTANTE:
        // SOLO RoundManager define los créditos requeridos
        playerProgress.ResetProgress(config.creditsRequired);

        /* // Spawn de enemigos
        if (enemySpawner != null)
            enemySpawner.SpawnEnemiesForRound(config.enemyCount);

        // Boss si aplica
        if (config.hasBoss)
            Debug.Log("Spawneando Boss para esta ronda"); */

        // Spawn de enemigos
        if (enemySpawner != null)
        {
            if (config.hasBoss)
            {
                // Spawnea enemigos normales (enemyCount - 1)
                int normalEnemies = Mathf.Max(0, config.enemyCount - 1);
                enemySpawner.SpawnEnemiesForRound(normalEnemies);

                // Spawnea el boss
                enemySpawner.SpawnBoss(config.bossPrefab);

                Debug.Log("Boss spawneado en esta ronda");
            }
            else
            {
                // Ronda sin boss
                enemySpawner.SpawnEnemiesForRound(config.enemyCount);
            }
        }

        onRoundStart?.Invoke(playerProgress.currentRound);
    }

    // Se llama cuando PlayerPickupProgress notifica progreso completo
    private void OnRoundCompleted()
    {
        if (!isRoundActive) return;

        isRoundActive = false;

        Debug.Log("POWER-UP SELECTION ABIERTA");

        // NO avanzamos ronda aquí
        // Esperamos a que el jugador elija un power-up
    }

    // Llamado desde botones de power-ups
    public void OnPowerUpSelected()
    {
        Debug.Log("Power-up seleccionado, avanzando ronda");

        // Reset visual de la barra inmediato (UX)
        playerProgress.ResetProgressVisualOnly();

        playerProgress.AdvanceRound();

        StartCoroutine(AdvanceToNextRound());
    }

    private IEnumerator AdvanceToNextRound()
    {
        yield return new WaitForSeconds(delayBetweenRounds);
        StartNewRound();
    }
}