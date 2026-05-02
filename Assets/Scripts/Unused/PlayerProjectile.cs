using UnityEngine;

/// <summary>
/// Script para balas que pueden ser disparadas por el sistema de auto-fuego
/// </summary>
public class PlayerProjectile : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float bulletSpeed = 20f;

    [Header("Daño")]
    [SerializeField] private int damageAmount = 1;

    [SerializeField] private bool autoTargetForTesting = false;
    private EnemyBaseHealth targetHealth;

    private Rigidbody rb;
    private bool hasHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Asignar target al iniciar (para pruebas sin AutoFireSystem)
    void Start()
    {
        if (autoTargetForTesting && targetHealth == null)
        {
            targetHealth = FindClosestEnemy();
        }
    }


    private void FixedUpdate()
    {
        /*  if (hasHit)
             return; */

        if (targetHealth != null)
        {
            Vector3 direction = (targetHealth.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * bulletSpeed;

            if (direction.magnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
        }
    }

    /* private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;
 
        EnemyBaseHealth enemyHealth = other.GetComponent<EnemyBaseHealth>();

        if (enemyHealth != null && enemyHealth == targetHealth)
        {
            HitEnemy(enemyHealth);
        }
    } */

    void OnCollisionEnter(Collision collision)
    {
        EnemyBaseHealth enemy =
            collision.collider.GetComponentInParent<EnemyBaseHealth>();

        if (enemy == null) return;

        enemy.TakeDamage(damageAmount);

        Destroy(gameObject);
    }

    /* private void HitEnemy(EnemyBaseHealth enemyHealth)
    {
        if (hasHit)
            return;

        hasHit = true;

        enemyHealth.TakeDamage(damageAmount);

        Debug.Log($"¡Bala golpeo a {enemyHealth.name}! Daño: {damageAmount}");

        Destroy(gameObject);
    } */

    public void SetTarget(EnemyBaseHealth target)
    {
        targetHealth = target;
    }

    public void SetDamage(int damage)
    {
        damageAmount = damage;
    }

    public void SetSpeed(float speed)
    {
        bulletSpeed = speed;
    }

    // Para realizar pruebas sin necesidad de AutoFireSystem
    private EnemyBaseHealth FindClosestEnemy()
    {
        EnemyBaseHealth[] enemies =
            FindObjectsByType<EnemyBaseHealth>(FindObjectsSortMode.None);

        if (enemies.Length == 0)
            return null;

        EnemyBaseHealth closest = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }
}