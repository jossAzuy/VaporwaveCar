using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tipos de PowerUps disponibles en el juego
/// </summary>
public enum PowerUpType
{
    None,
    IncreasedFireRate,    // Aumento en la tasa de disparo (pasivo)
    ExtraLife,            // Añade un corazón extra (pasivo)
    RammingDamage,        // Destruye enemigos al colisionar (pasivo)
    TemporalShield,       // Escudo temporal (requiere doble click)
    Mines,                // Sistema de minas (activo - colocar minas)
    HealthRecovery        // Recupera vida (activo - uso inmediato)
}

/// <summary>
/// Gestiona los PowerUps seleccionados y activos del jugador
/// </summary>
public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [Header("PowerUp Activo")]
    [SerializeField] private PowerUpType selectedPowerUp = PowerUpType.None;
    [SerializeField] private PowerUpType activePowerUp = PowerUpType.None;

    [Header("Configuración - Fire Rate")]
    [SerializeField] private float fireRateMultiplier = 0.7f; // Reduce el tiempo entre disparos

    [Header("Configuración - Extra Life")]
    [SerializeField] private int extraLives = 1;

    [Header("Configuración - Ramming Damage")]
    [SerializeField] private int rammingDamage = 999; // Daño instantáneo
    [SerializeField] private bool isRammingActive = false;

    [Header("Configuración - Temporal Shield")]
    [SerializeField] private float shieldDuration = 5f;
    [SerializeField] private bool isShieldActive = false;
    [SerializeField] private float doubleTapTimeWindow = 0.3f;
    private float lastTapTime = 0f;

    [Header("Configuración - Mines")]
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private int maxMines = 3;
    private int currentMines = 0;

    [Header("Referencias")]
    [SerializeField] private AutoFireSystem autoFireSystem;
    //[SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerHealth_ playerHealth;
    [SerializeField] private GameObject shieldVisual; // Visual del escudo

    [Header("Eventos")]
    public UnityEvent<PowerUpType> onPowerUpSelected;
    public UnityEvent<PowerUpType> onPowerUpActivated;
    public UnityEvent onShieldActivated;
    public UnityEvent onShieldDeactivated;

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
        // Buscar referencias si no están asignadas
        if (autoFireSystem == null)
            autoFireSystem = FindFirstObjectByType<AutoFireSystem>();

        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth_>();

        // Asegurar que el escudo visual esté desactivado al inicio
        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    private void Update()
    {
        // Detectar doble click para activar escudo temporal
        if (selectedPowerUp == PowerUpType.TemporalShield && !isShieldActive)
        {
            DetectDoubleTap();
        }

        // Detectar click para colocar minas
        if (selectedPowerUp == PowerUpType.Mines && Input.GetMouseButtonDown(0))
        {
            TryPlaceMine();
        }
    }

    /// <summary>
    /// Selecciona un PowerUp para ser usado
    /// </summary>
    public void SelectPowerUp(PowerUpType powerUpType)
    {
        selectedPowerUp = powerUpType;
        onPowerUpSelected?.Invoke(powerUpType);

        Debug.Log($"PowerUp seleccionado: {powerUpType}");
    }

    /// <summary>
    /// Activa el PowerUp seleccionado al inicio de la ronda
    /// </summary>
    public void ActivatePowerUpForRound()
    {
        if (selectedPowerUp == PowerUpType.None)
        {
            Debug.Log("No hay PowerUp seleccionado");
            return;
        }

        activePowerUp = selectedPowerUp;

        switch (selectedPowerUp)
        {
            case PowerUpType.IncreasedFireRate:
                ActivateIncreasedFireRate();
                break;

            case PowerUpType.ExtraLife:
                ActivateExtraLife();
                break;

            case PowerUpType.RammingDamage:
                ActivateRammingDamage();
                break;

            case PowerUpType.HealthRecovery:
                ActivateHealthRecovery();
                break;

            case PowerUpType.TemporalShield:
                // Se activa con doble click, solo preparar
                Debug.Log("Escudo temporal listo. Haz doble click para activar.");
                break;

            case PowerUpType.Mines:
                // Se activan con click, solo preparar
                currentMines = maxMines;
                Debug.Log($"Sistema de minas listo. {currentMines} minas disponibles.");
                break;
        }

        onPowerUpActivated?.Invoke(selectedPowerUp);
    }

    #region Powerup Implementations

    /// <summary>
    /// Aumenta la tasa de disparo reduciendo el fireRate
    /// </summary>
    private void ActivateIncreasedFireRate()
    {
        if (autoFireSystem != null)
        {
            // autoFireSystem.SetFireRateMultiplier(fireRateMultiplier);
            Debug.Log($"Tasa de disparo aumentada (x{1 / fireRateMultiplier:F1})");
        }
    }

    /// <summary>
    /// Añade vidas extra al jugador
    /// </summary>
    private void ActivateExtraLife()
    {
        if (playerHealth != null)
        {
            playerHealth.AddExtraLives(extraLives);
            Debug.Log($"Vidas extra añadidas: +{extraLives}");
        }
    }

    /// <summary>
    /// Activa la habilidad de destruir enemigos al colisionar
    /// </summary>
    private void ActivateRammingDamage()
    {
        isRammingActive = true;
        Debug.Log("Ramming Damage activado. ¡Destruye enemigos al colisionar!");
    }

    /// <summary>
    /// Recupera vida del jugador
    /// </summary>
    private void ActivateHealthRecovery()
    {
        if (playerHealth != null)
        {
            playerHealth.RecoverHealth(1);
            Debug.Log("Vida recuperada");
        }
    }

    /// <summary>
    /// Detecta doble click para activar el escudo
    /// </summary>
    private void DetectDoubleTap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastTap = Time.time - lastTapTime;

            if (timeSinceLastTap <= doubleTapTimeWindow)
            {
                // Doble click detectado
                ActivateShield();
            }

            lastTapTime = Time.time;
        }
    }

    /// <summary>
    /// Activa el escudo temporal
    /// </summary>
    private void ActivateShield()
    {
        if (isShieldActive) return;

        isShieldActive = true;

        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        onShieldActivated?.Invoke();
        Debug.Log($"¡Escudo activado por {shieldDuration} segundos!");

        Invoke(nameof(DeactivateShield), shieldDuration);
    }

    /// <summary>
    /// Desactiva el escudo temporal
    /// </summary>
    private void DeactivateShield()
    {
        isShieldActive = false;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        onShieldDeactivated?.Invoke();
        Debug.Log("Escudo desactivado");
    }

    /// <summary>
    /// Intenta colocar una mina
    /// </summary>
    private void TryPlaceMine()
    {
        if (currentMines <= 0)
        {
            Debug.Log("No quedan minas disponibles");
            return;
        }

        if (minePrefab != null)
        {
            // Colocar mina en la posición del jugador
            Vector3 minePosition = transform.position;
            Instantiate(minePrefab, minePosition, Quaternion.identity);

            currentMines--;
            Debug.Log($"Mina colocada. Minas restantes: {currentMines}");
        }
    }

    #endregion

    #region Public Getters

    public bool IsRammingActive()
    {
        return isRammingActive;
    }

    public bool IsShieldActive()
    {
        return isShieldActive;
    }

    public int GetRammingDamage()
    {
        return rammingDamage;
    }

    public PowerUpType GetSelectedPowerUp()
    {
        return selectedPowerUp;
    }

    public PowerUpType GetActivePowerUp()
    {
        return activePowerUp;
    }

    public int GetRemainingMines()
    {
        return currentMines;
    }

    #endregion

    #region Reset

    /// <summary>
    /// Resetea los powerups al estado inicial
    /// </summary>
    public void ResetPowerUps()
    {
        isRammingActive = false;
        isShieldActive = false;
        currentMines = 0;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        // Resetear fire rate si estaba modificado
        if (autoFireSystem != null && activePowerUp == PowerUpType.IncreasedFireRate)
        {
            //  autoFireSystem.SetFireRateMultiplier(1f);
        }

        selectedPowerUp = PowerUpType.None;
        activePowerUp = PowerUpType.None;
    }

    #endregion
}
