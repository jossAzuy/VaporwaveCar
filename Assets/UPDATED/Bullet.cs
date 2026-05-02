using UnityEngine;

/// <summary>
/// Script para balas que pueden ser disparadas por el sistema de auto-fuego
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float projectileSpeed = 20f;

    [Header("Daño")]
    private float damageAmount = 1f;
    private Enemy targetEnemy;

    private Rigidbody rb;
    private bool hasHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void FixedUpdate()
    {
        if (hasHit)
            return;

        if (targetEnemy != null && targetEnemy.IsAlive)
        {
            // Moverse hacia el enemigo
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * projectileSpeed;

            // Rotar hacia el objetivo
            if (direction.magnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            // El objetivo murió o fue destruido, continuar en línea recta
            rb.linearVelocity = transform.forward * projectileSpeed;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (hasHit)
            return;

        Enemy enemy = collision.GetComponent<Enemy>();
        
        if (enemy != null && enemy == targetEnemy)
        {
            HitEnemy(enemy);
        }
    }

    private void HitEnemy(Enemy enemy)
    {
        if (hasHit)
            return;

        hasHit = true;
        enemy.TakeDamage(damageAmount);
        
        Debug.Log($"¡Bala golpeó a {enemy.name}! Daño: {damageAmount}");
        
        Destroy(gameObject);
    }

    public void SetTarget(Enemy target)
    {
        targetEnemy = target;
    }

    public void SetDamage(float damage)
    {
        damageAmount = damage;
    }

    public void SetSpeed(float speed)
    {
        projectileSpeed = speed;
    }
}
