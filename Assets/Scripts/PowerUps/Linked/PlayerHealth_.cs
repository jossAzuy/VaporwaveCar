using UnityEngine;
using System;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class PlayerHealth_ : MonoBehaviour
{
    [Header("Vida del jugador")]
    public int maxHealth = 3;
    private int currentHealth;
    private int extraLivesAdded = 0; // Vidas extra añadidas por powerups

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
            // Verificar si el escudo está activo
            if (PowerUpManager.Instance != null && PowerUpManager.Instance.IsShieldActive())
            {
                Debug.Log("Daño bloqueado por escudo");
                return;
            }

            // Verificar si el ramming está activo para destruir al enemigo
            if (PowerUpManager.Instance != null && PowerUpManager.Instance.IsRammingActive())
            {
                EnemyBaseHealth enemyHealth = collision.gameObject.GetComponent<EnemyBaseHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(PowerUpManager.Instance.GetRammingDamage());
                    Debug.Log("¡Enemigo destruido por ramming!");
                    return; // No recibir daño si destruimos al enemigo
                }
            }

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

    /*   void UpdateHearts()
      {
          for (int i = 0; i < hearts.Length; i++)
          {
              hearts[i].enabled = i < currentHealth;
          }
      } */

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < maxHealth)
            {
                hearts[i].gameObject.SetActive(true);
                hearts[i].enabled = i < currentHealth;
            }
            else
            {
                hearts[i].gameObject.SetActive(false);
            }
        }
    }


    public static event Action OnPlayerDeath;

    void GameOver()
    {
        OnPlayerDeath?.Invoke();

        // Reproductor de feedbacks de muerte
        deathFeedbacks?.PlayFeedbacks();
    }

    /// <summary>
    /// Añade vidas extra (aumenta maxHealth y currentHealth)
    /// </summary>
    public void AddExtraLives(int amount)
    {
        extraLivesAdded += amount;
        maxHealth += amount;
        currentHealth += amount;

        Debug.Log($"Vidas extra añadidas: +{amount}. Vida actual: {currentHealth}/{maxHealth}");
        UpdateHearts();
    }

    /// <summary>
    /// Recupera vida del jugador (hasta el máximo)
    /// </summary>
    public void RecoverHealth(int amount)
    {
        if (currentHealth >= maxHealth)
        {
            Debug.Log("La vida ya está al máximo");
            return;
        }

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Vida recuperada: +{amount}. Vida actual: {currentHealth}/{maxHealth}");
        UpdateHearts();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}