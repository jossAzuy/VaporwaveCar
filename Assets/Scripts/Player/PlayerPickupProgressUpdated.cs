using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

public class PlayerPickupProgressUpdated : MonoBehaviour
{
    [Header("Progress Bar")]
    [SerializeField] private MMProgressBar progressBar;

    [Header("Credits")]
    // CAMBIO: ahora son privados y serializados
    // Motivo: este script NO decide valores, solo los usa
    [SerializeField] private int currentCredits = 0;
    [SerializeField] private int creditsRequired = 0;

    [Header("Round")]
    // Se mantiene público porque RoundManager necesita leerlo
    public int currentRound = 1;

    [Header("Events")]
    public UnityEvent onProgressCompleted;

    private bool completed = false;

    private void Start()
    {
        UpdateBar();
    }

    // Añade créditos al progreso actual
    public void AddCredit(int amount)
    {
        if (completed) return;

        currentCredits += amount;
        UpdateBar();

        if (currentCredits >= creditsRequired)
        {
            completed = true;
            onProgressCompleted?.Invoke();
            Debug.Log($"Progreso completado en ronda {currentRound}");
        }
    }

    // CAMBIO CLAVE:
    // Este método SOLO debe ser llamado por RoundManager
    // PlayerPickupProgress NO decide cuándo ni con cuánto se resetea
    public void ResetProgress(int newCreditsRequired)
    {
        completed = false;
        currentCredits = 0;
        creditsRequired = newCreditsRequired;

        // Reinicia visualmente la barra
        progressBar.SetBar01(progressBar.InitialFillValue);
    }

    // Avanza el número de ronda
    // (La decisión de CUÁNDO se avanza es externa)
    public void AdvanceRound()
    {
        currentRound++;
    }

    // Actualiza la barra según progreso actual
    private void UpdateBar()
    {
        float progress = creditsRequired <= 0
            ? 0f
            : (float)currentCredits / creditsRequired;

        progressBar.UpdateBar01(Mathf.Clamp01(progress));
    }

    // Útil para UI u otros sistemas
    public int GetCreditsRemaining()
    {
        return Mathf.Max(0, creditsRequired - currentCredits);
    }

    //  Reset SOLO visual (no cambia creditos requeridos)
    public void ResetProgressVisualOnly()
    {
        currentCredits = 0;
        completed = false;
        //UpdateBar();

        // progressBar.SetBar01(progressBar.InitialFillValue);
        //progressBar.SetBar01(progressBar.InitialFillValue);

        // Fuerza el valor REAL mínimo de la barra
        progressBar.SetBar(
         progressBar.MinimumBarFillValue,
         progressBar.MinimumBarFillValue,
         progressBar.MaximumBarFillValue
     );

    }
}



/* 
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;
using MoreMountains.Feedbacks;

public class PlayerPickupProgressUpdated : MonoBehaviour
{
    [Header("Progress Bar")]
    public MMProgressBar progressBar;

    [Header("Credits")]
    public int currentCredits = 0;
    public int creditsRequired = 0;

    [Header("Round")]
    public int currentRound = 1;

    [Header("Events")]
    public UnityEvent onProgressCompleted;

    private bool completed = false;

    public MMFeedbacks[] feedbacks;

    void Start()
    {
        UpdateBar();
    }

    // Añade créditos al progreso
    public void AddCredit(int amount)
    {
        if (completed) return;

        currentCredits += amount;
        UpdateBar();

        if (currentCredits >= creditsRequired)
        {
            completed = true;
            onProgressCompleted?.Invoke();
            Debug.Log($"Progreso completado en ronda {currentRound}");
        }
    }


    public void ResetProgress(int newCreditsRequired)
    {
        completed = false;
        currentCredits = 0;
        creditsRequired = newCreditsRequired;

        // Reinicia la barra al valor inicial inmediatamente
        //progressBar.SetBar01(progressBar.SetInitialFillValueOnStart ? progressBar.InitialFillValue : 0f);

        progressBar.SetBar01(progressBar.InitialFillValue);
    }


    // Avanza el número de ronda
    public void AdvanceRound()
    {
        currentRound++;
    }

    void UpdateBar()
    {
        float progress = creditsRequired <= 0 ? 0f : (float)currentCredits / creditsRequired;
        progressBar.UpdateBar01(Mathf.Clamp01(progress));
    }

    public int GetCreditsRemaining()
    {
        return Mathf.Max(0, creditsRequired - currentCredits);
    }

}
 */


