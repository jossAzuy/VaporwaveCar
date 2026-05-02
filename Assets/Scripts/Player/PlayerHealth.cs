using UnityEngine;
using System;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida del jugador")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Corazones UI")]
    public Image[] hearts;

    // References to feedbacks
    public MMFeedbacks damageFeedback;
    public MMFeedbacks deathFeedbacks;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);

            damageFeedback?.PlayFeedbacks();
        }
    }

    void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        UpdateHearts();

        if (currentHealth <= 0)
        {
            Debug.Log("El jugador se quedó sin vida (Game Over)");
            GameOver();
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentHealth;
        }
    }

    public static event Action OnPlayerDeath;

    void GameOver()
    {
        OnPlayerDeath?.Invoke();

        // Reproductor de feedbacks de muerte
        deathFeedbacks?.PlayFeedbacks();
    }
}