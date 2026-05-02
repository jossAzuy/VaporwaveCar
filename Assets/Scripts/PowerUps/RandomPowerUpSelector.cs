using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestiona la selección aleatoria de 3 powerups para cada ronda
/// Se encarga de asignar los powerups a los botones dinámicamente
/// </summary>
public class RandomPowerUpSelector : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int powerUpsPerRound = 3;
    [SerializeField] private PowerUpButton[] powerUpButtons;

    private List<PowerUpType> availablePowerUps = new List<PowerUpType>();
    private List<PowerUpType> selectedPowerUps = new List<PowerUpType>();

    private void Start()
    {
        // Buscar los botones si no están asignados
        if (powerUpButtons == null || powerUpButtons.Length == 0)
        {
            powerUpButtons = GetComponentsInChildren<PowerUpButton>();
        }

        if (powerUpButtons.Length < powerUpsPerRound)
        {
            Debug.LogError($"Hay {powerUpButtons.Length} botones pero se necesitan al menos {powerUpsPerRound}");
        }

        // Inicializar la lista de powerups disponibles (excluir None)
        InitializeAvailablePowerUps();
    }

    /// <summary>
    /// Inicializa la lista de powerups disponibles
    /// </summary>
    private void InitializeAvailablePowerUps()
    {
        availablePowerUps.Clear();

        // Agregar todos los powerups excepto None
        availablePowerUps.Add(PowerUpType.IncreasedFireRate);
        availablePowerUps.Add(PowerUpType.ExtraLife);
        availablePowerUps.Add(PowerUpType.RammingDamage);
        availablePowerUps.Add(PowerUpType.TemporalShield);
        availablePowerUps.Add(PowerUpType.Mines);
        availablePowerUps.Add(PowerUpType.HealthRecovery);
    }

    /// <summary>
    /// Selecciona 3 powerups aleatorios y los asigna a los botones
    /// Se llama cuando aparece el panel de powerups
    /// </summary>
    public void SelectRandomPowerUps()
    {
        selectedPowerUps.Clear();

        // Crear una copia de la lista disponible para no modificar la original
        List<PowerUpType> tempList = new List<PowerUpType>(availablePowerUps);

        // Seleccionar N powerups aleatorios sin repetición
        for (int i = 0; i < powerUpsPerRound && tempList.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            selectedPowerUps.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        // Asignar los powerups seleccionados a los botones
        AssignPowerUpsToButtons();

        Debug.Log($"PowerUps seleccionados para esta ronda: {string.Join(", ", selectedPowerUps)}");
    }

    /// <summary>
    /// Asigna los powerups seleccionados a los botones correspondientes
    /// </summary>
    private void AssignPowerUpsToButtons()
    {
        for (int i = 0; i < selectedPowerUps.Count && i < powerUpButtons.Length; i++)
        {
            if (powerUpButtons[i] != null)
            {
                // powerUpButtons[i].SetPowerUpType(selectedPowerUps[i]); ************************
                powerUpButtons[i].gameObject.SetActive(true);

                Debug.Log($"Botón {i + 1}: {selectedPowerUps[i]}");
            }
        }

        // Desactivar botones restantes si hay menos de 3 powerups disponibles
        for (int i = selectedPowerUps.Count; i < powerUpButtons.Length; i++)
        {
            if (powerUpButtons[i] != null)
            {
                powerUpButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Obtiene los powerups seleccionados actualmente
    /// </summary>
    public List<PowerUpType> GetSelectedPowerUps()
    {
        return new List<PowerUpType>(selectedPowerUps);
    }

    /// <summary>
    /// Obtiene un powerup específico por índice
    /// </summary>
    public PowerUpType GetPowerUpAtIndex(int index)
    {
        if (index >= 0 && index < selectedPowerUps.Count)
            return selectedPowerUps[index];

        return PowerUpType.None;
    }

    /// <summary>
    /// Configura los botones manualmente si es necesario
    /// </summary>
    public void SetPowerUpButtons(PowerUpButton[] buttons)
    {
        powerUpButtons = buttons;
    }

    /// <summary>
    /// Permite excluir o incluir ciertos powerups (opcional)
    /// </summary>
    public void SetAvailablePowerUps(List<PowerUpType> powerUps)
    {
        availablePowerUps = new List<PowerUpType>(powerUps);
    }
}
