using UnityEngine;

/// <summary>
/// Interfaz base para enemigos (para compatibilidad con AutoFireSystem)
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float health = 1f;
    
    protected bool isAlive = true;

    public bool IsAlive => isAlive;
    public virtual float Health => health;

    /// <summary>
    /// Inflige daño al enemigo
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Mata al enemigo
    /// </summary>
    protected virtual void Die()
    {
        isAlive = false;
        Destroy(gameObject);
    }
}
