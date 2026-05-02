/* // Este script fue actualizado ahora soporta:
// - Rondas
// - Créditos requeridos dinámicos
// - Reset limpio

// Ahora: 
// La barra depende de creditsRequired
// Puedes cambiar la dificultad por ronda
//  El reset es limpio



using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

public class PlayerPickupProgressV2 : MonoBehaviour
{
    [Header("Progress Bar")]
    public MMProgressBar progressBar;

    [Header("Credits")]
    public int currentCredits = 0;
    public int creditsRequired = 10;

    [Header("Round")]
    public int currentRound = 1;

    [Header("Events")]
    public UnityEvent onProgressCompleted;

    private bool completed = false;

    void Start()
    {
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
        }
    }

    void UpdateBar()
    {
        float progress = (float)currentCredits / creditsRequired;
        progressBar.UpdateBar01(Mathf.Clamp01(progress));
    }

    public void ResetProgress(int newCreditsRequired)
    {
        currentCredits = 0;
        creditsRequired = newCreditsRequired;
        completed = false;
        currentRound++;

        UpdateBar();
    }
}
 */