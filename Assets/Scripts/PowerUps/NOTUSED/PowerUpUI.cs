using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Muestra información sobre el PowerUp activo del jugador en la UI
/// </summary>
public class PowerUpUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private GameObject powerUpPanel;
    [SerializeField] private TextMeshProUGUI powerUpNameText;
    [SerializeField] private TextMeshProUGUI powerUpStatusText;
    [SerializeField] private Image powerUpIcon;
    
    [Header("Instrucciones")]
    [SerializeField] private TextMeshProUGUI instructionsText;
    
    [Header("Contador de Minas")]
    [SerializeField] private GameObject mineCounterPanel;
    [SerializeField] private TextMeshProUGUI mineCountText;

    [Header("Escudo")]
    [SerializeField] private GameObject shieldIndicator;

    private PowerUpManager powerUpManager;

    private void Start()
    {
        powerUpManager = PowerUpManager.Instance;
        
        if (powerUpManager != null)
        {
            powerUpManager.onPowerUpActivated.AddListener(OnPowerUpActivated);
            powerUpManager.onShieldActivated.AddListener(OnShieldActivated);
            powerUpManager.onShieldDeactivated.AddListener(OnShieldDeactivated);
        }

        // Ocultar todo al inicio
        if (powerUpPanel != null)
            powerUpPanel.SetActive(false);
            
        if (mineCounterPanel != null)
            mineCounterPanel.SetActive(false);
            
        if (shieldIndicator != null)
            shieldIndicator.SetActive(false);
    }

    private void Update()
    {
        if (powerUpManager == null) return;

        PowerUpType activePowerUp = powerUpManager.GetActivePowerUp();

        // Actualizar contador de minas si está activo
        if (activePowerUp == PowerUpType.Mines)
        {
            UpdateMineCounter();
        }
    }

    private void OnPowerUpActivated(PowerUpType powerUpType)
    {
        if (powerUpPanel != null)
            powerUpPanel.SetActive(true);

        UpdatePowerUpDisplay(powerUpType);
        UpdateInstructions(powerUpType);

        // Mostrar contador de minas si es necesario
        if (mineCounterPanel != null)
        {
            mineCounterPanel.SetActive(powerUpType == PowerUpType.Mines);
        }
    }

    private void UpdatePowerUpDisplay(PowerUpType powerUpType)
    {
        if (powerUpNameText != null)
        {
            powerUpNameText.text = GetPowerUpName(powerUpType);
        }

        if (powerUpStatusText != null)
        {
            powerUpStatusText.text = "Activo";
        }
    }

    private void UpdateInstructions(PowerUpType powerUpType)
    {
        if (instructionsText == null) return;

        string instructions = "";
        
        switch (powerUpType)
        {
            case PowerUpType.TemporalShield:
                instructions = "Doble click para activar escudo";
                break;
            case PowerUpType.Mines:
                instructions = "Click para colocar mina";
                break;
            case PowerUpType.IncreasedFireRate:
                instructions = "Disparo rápido activo";
                break;
            case PowerUpType.ExtraLife:
                instructions = "Vida extra añadida";
                break;
            case PowerUpType.RammingDamage:
                instructions = "¡Choca contra enemigos!";
                break;
            case PowerUpType.HealthRecovery:
                instructions = "Vida recuperada";
                break;
        }

        instructionsText.text = instructions;
    }

    private void UpdateMineCounter()
    {
        if (mineCountText != null && powerUpManager != null)
        {
            int remainingMines = powerUpManager.GetRemainingMines();
            mineCountText.text = $"Minas: {remainingMines}";
        }
    }

    private void OnShieldActivated()
    {
        if (shieldIndicator != null)
        {
            shieldIndicator.SetActive(true);
        }

        if (powerUpStatusText != null)
        {
            powerUpStatusText.text = "¡ESCUDO ACTIVO!";
        }
    }

    private void OnShieldDeactivated()
    {
        if (shieldIndicator != null)
        {
            shieldIndicator.SetActive(false);
        }

        if (powerUpStatusText != null)
        {
            powerUpStatusText.text = "Listo para activar";
        }
    }

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
                return "Escudo Temporal";
            case PowerUpType.Mines:
                return "Minas";
            case PowerUpType.HealthRecovery:
                return "Recuperar Vida";
            default:
                return "Sin PowerUp";
        }
    }
}
