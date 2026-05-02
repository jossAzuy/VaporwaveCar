using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Events;

public class PlayerPickupProgress : MonoBehaviour
{
    [Header("Progress Bar")]
    public MMProgressBar progressBar;

    [Range(0f, 1f)]
    public float currentProgress = 0f;

    public float pickupIncrement = 0.1f;

    [Header("Events")]
    public UnityEvent onProgressCompleted;

    private bool completed = false;

    void Start()
    {
        progressBar.SetBar01(currentProgress);
    }

    public void AddProgress(float amount)
    {
        currentProgress = Mathf.Clamp01(currentProgress + amount);
        progressBar.UpdateBar01(currentProgress);

        if (!completed && currentProgress >= 1f)
        {
            completed = true;
            onProgressCompleted?.Invoke();
        }
    }
}
