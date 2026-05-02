// Este script fue actualizado ahora soporta:
// - Rondas
// - Créditos requeridos dinámicos con multiplicador por ronda
// - Reset limpio

// Ahora: 
// La barra depende de creditsRequired
// Puedes cambiar la dificultad por ronda
// El reset es limpio
// NUEVO: Los requisitos se multiplican por cada ronda

using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

public class PlayerPickupProgress_ : MonoBehaviour
{
    [Header("Progress Bar")]
    public MMProgressBar progressBar;

    [Header("Credits")]
    public int currentCredits = 0;
    public int creditsRequired = 10;
    public int baseCreditsRequired = 10; // Requisito base para la ronda 1

    [Header("Round")]
    public int currentRound = 1;

    [Header("Dificultad")]
    [SerializeField] private float roundMultiplier = 1.2f; // Multiplica los requisitos cada ronda

    [Header("Events")]
    public UnityEvent onProgressCompleted;

    private bool completed = false;

    void Start()
    {
        baseCreditsRequired = creditsRequired;
        UpdateBar();
    }

    public void AddCredit(int amount)
    {
        if (completed) return;

        currentCredits += amount;
        UpdateBar();

        if (currentCredits >= creditsRequired)
        {
            completed = true;
            onProgressCompleted?.Invoke();

            Debug.Log("Progreso completado en ronda " + currentRound);
        }
    }

    void UpdateBar()
    {
        float progress = (float)currentCredits / creditsRequired;
        progressBar.UpdateBar01(Mathf.Clamp01(progress));
    }

    /// <summary>
    /// Resetea el progreso y avanza a la siguiente ronda
    /// Los requisitos se multiplican automáticamente
    /// </summary>
   /*  public void ResetProgress()
    {
        currentCredits = 0;
        completed = false;
        currentRound++;

        // Calcular nuevos requisitos multiplicados
        creditsRequired = Mathf.RoundToInt(baseCreditsRequired * Mathf.Pow(roundMultiplier, currentRound - 1));

        UpdateBar();

        Debug.Log($"Ronda {currentRound} iniciada. Créditos requeridos: {creditsRequired}");
    } */

    /// <summary>
    /// Resetea el progreso con requisitos especificados manualmente
    /// </summary>
    public void ResetProgress(int newCreditsRequired)
    {
        currentCredits = 0;
        creditsRequired = newCreditsRequired;
        baseCreditsRequired = newCreditsRequired;
        completed = false;
        
        //currentRound = 1;

        UpdateBar();
    }

    public float GetProgress()
    {
        return (float)currentCredits / creditsRequired;
    }

    public int GetCreditsRemaining()
    {
        return creditsRequired - currentCredits;
    }

    public void SetRoundMultiplier(float multiplier)
    {
        roundMultiplier = multiplier;
    }
}
