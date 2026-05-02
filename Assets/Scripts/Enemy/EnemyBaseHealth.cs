using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;

/// <summary>
/// Maneja la vida y muerte base de los enemigos
/// Permite reproducir feedbacks antes de destruir el objeto
/// </summary>
public class EnemyBaseHealth : MonoBehaviour
{
    [Header("Vida del enemigo")]
    public int maxHealth = 3;

    [SerializeField]
    private int currentHealth;

    [Header("Feedbacks")]
    [SerializeField] private MMFeedbacks damageFeedback;
    [SerializeField] private MMFeedbacks deathFeedbacks;

    // NUEVO: flag para evitar ejecuciones múltiples de muerte
    private bool isDead = false;

    private void Awake()
    {
        // Inicialización explícita de feedbacks (recomendado por MoreMountains)
        damageFeedback?.Initialization();
        deathFeedbacks?.Initialization();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Aplica daño al enemigo
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        damageFeedback?.PlayFeedbacks();
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            EnemyDeath();
        }
    }

    /// <summary>
    /// Maneja la muerte lógica y visual del enemigo
    /// </summary>
    private void EnemyDeath()
    {
        if (isDead)
            return;

        isDead = true;

        // OTORGAR RECOMPENSA AL MORIR
        EnemyReward reward = GetComponent<EnemyReward>();
        if (reward != null)
        {
            reward.GiveReward();
        }

        // NUEVO:
        // Desactivamos inmediatamente toda la lógica del enemigo
        // para que deje de interactuar con el juego
        DisableEnemyLogic();

        // NUEVO:
        // Reproducimos feedbacks de muerte y destruimos el objeto
        // solo cuando hayan terminado
        if (deathFeedbacks != null)
        {
            deathFeedbacks.PlayFeedbacks();
            // StartCoroutine(DestroyAfterFeedback(deathFeedbacks.TotalDuration));
            StartCoroutine(DestroyAfterFeedback(damageFeedback.TotalDuration));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// NUEVO:
    /// Desactiva movimiento, colisiones y físicas del enemigo
    /// para simular su muerte inmediata a nivel gameplay
    /// </summary>
    private void DisableEnemyLogic()
    {
        // IA / Movimiento
        EnemyCarNavMesh nav = GetComponent<EnemyCarNavMesh>();
        if (nav != null)
            nav.enabled = false;

        // Renderizado
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var rend in renderers)
        {
            rend.enabled = false;
        }

        // Colisiones
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Física
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
    }

    /// <summary>
    /// NUEVO:
    /// Espera a que terminen los feedbacks antes de destruir el objeto
    /// </summary>
    private IEnumerator DestroyAfterFeedback(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}


/*
using UnityEngine;
using System;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class EnemyBaseHealth : MonoBehaviour
{
    [Header("Vida del enemigo")]
    public int maxHealth = 3;

    [SerializeField]
    private int currentHealth;
    [Header("Feedbacks")]
    [SerializeField] private MMFeedbacks damageFeedback;
    [SerializeField] private MMFeedbacks deathFeedbacks;

    private bool isDead = false;

    void Awake()
    {
        damageFeedback?.Initialization();
        deathFeedbacks?.Initialization();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }


    public void TakeDamage(int damage)
    {
        Debug.Log($"Enemy TakeDamage");

        if (isDead && currentHealth <= 0)
            return;

        damageFeedback?.PlayFeedbacks();
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            // Debug.Log("El enemigo se quedó sin vida");
            isDead = true;
            EnemyDeath();
        }
    }

    //public static event Action OnEnemyDeath;

    void EnemyDeath()
    {
        // OnEnemyDeath?.Invoke();

        // Otorgar recompensa al jugador al morir el enemigo
        EnemyReward reward = GetComponent<EnemyReward>();
        if (reward != null)
        {
            reward.GiveReward();
        }
        // Reproductor de feedbacks de muerte
        deathFeedbacks?.PlayFeedbacks(); // Al momento no tiene feedbacks asignados
        //damageFeedback?.PlayFeedbacks();

        Destroy(gameObject);

    }
}
*/