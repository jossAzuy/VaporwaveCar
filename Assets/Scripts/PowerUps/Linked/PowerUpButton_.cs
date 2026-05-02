using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Botón para seleccionar un PowerUp específico
/// Se asigna desde el inspector
/// </summary>
public class PowerUpButton_ : MonoBehaviour
{
    [Header("PowerUp Configuration")]
    [SerializeField] private PowerUpType powerUpType = PowerUpType.None;
    [SerializeField] private int newCreditsRequired = 15;

    [Header("UI (Opcional)")]
    [SerializeField] private TextMeshProUGUI powerUpNameText;
    [SerializeField] private TextMeshProUGUI powerUpDescriptionText;
    [SerializeField] private Image powerUpIcon;

    [Header("Referencias")]
    private UIScreenManager uiManager;

    private void Start()
    {
        // Buscar UIScreenManager si no está asignado
        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIScreenManager>();
        }

        // Actualizar UI si hay textos asignados
        UpdateUI();
    }

    /// <summary>
    /// Método llamado cuando se hace click en el botón
    /// </summary>
    /* public void SelectPowerUp()
    {
        Debug.Log($"PowerUp seleccionado: {powerUpType}");

        // Seleccionar el powerup en el manager
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.SelectPowerUp(powerUpType);
        }

        // Resetear el progreso con nuevos requisitos
        PlayerPickupProgress_ progress = FindFirstObjectByType<PlayerPickupProgress_>();
        if (progress != null)
        {
            progress.ResetProgress(newCreditsRequired);
        }

        // Activar el powerup para esta ronda
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.ActivatePowerUpForRound();
        }

        // Ocultar UI de power-ups y volver a gameplay
        if (uiManager != null)
        {
            uiManager.ShowGameplay();
        }
    } */

    /// <summary>
    /// Actualiza los textos e iconos de la UI
    /// </summary>
    private void UpdateUI()
    {
        if (powerUpNameText != null)
        {
            powerUpNameText.text = GetPowerUpName(powerUpType);
        }

        if (powerUpDescriptionText != null)
        {
            powerUpDescriptionText.text = GetPowerUpDescription(powerUpType);
        }
    }

    /// <summary>
    /// Obtiene el nombre del powerup
    /// </summary>
    private string GetPowerUpName(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.IncreasedFireRate:
                return "Disparo Rápido";
            case PowerUpType.ExtraLife:
                return "Vida Extra";
            case PowerUpType.RammingDamage:
                return "Embestida";
            case PowerUpType.TemporalShield:
                return "Escudo";
            case PowerUpType.Mines:
                return "Minas";
            case PowerUpType.HealthRecovery:
                return "Recuperar Vida";
            default:
                return "Sin PowerUp";
        }
    }

    /// <summary>
    /// Obtiene la descripción del powerup
    /// </summary>
    private string GetPowerUpDescription(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.IncreasedFireRate:
                return "Aumenta la velocidad de disparo";
            case PowerUpType.ExtraLife:
                return "Añade un corazón extra";
            case PowerUpType.RammingDamage:
                return "Destruye enemigos al colisionar";
            case PowerUpType.TemporalShield:
                return "Escudo temporal (Doble click)";
            case PowerUpType.Mines:
                return "Coloca minas explosivas";
            case PowerUpType.HealthRecovery:
                return "Recupera un corazón";
            default:
                return "";
        }
    }

    /// <summary>
    /// Método público para configurar el tipo de powerup desde código
    /// </summary>
    public void SetPowerUpType(PowerUpType type)
    {
        powerUpType = type;
        UpdateUI();
    }

    public PowerUpType GetPowerUpType()
    {
        return powerUpType;
    }
}
