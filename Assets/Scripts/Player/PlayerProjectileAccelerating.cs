using UnityEngine;

/// <summary>
/// Versión alternativa de PlayerProjectile que conserva la misma lógica,
/// añadiendo aceleración progresiva del proyectil.
/// </summary>
public class PlayerProjectileAccelerating : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float bulletSpeed = 20f;

    [Header("Aceleración (mejora)")]
    [SerializeField] private bool useAcceleration = true;
    [SerializeField] private float acceleration = 30f; // (m/s^2) Aumento de velocidad por segundo.
    [SerializeField] private float maxSpeed = 60f;     // Límite para que no se dispare indefinidamente.

    [Header("Daño")]
    [SerializeField] private int damageAmount = 1;

    [SerializeField] private bool autoTargetForTesting = false;
    private EnemyBaseHealth targetHealth;

    private Rigidbody rb;
    private bool hasHit = false;

    // MOD: velocidad actual independiente para poder acelerar sin romper SetSpeed().
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // MOD: inicializamos con bulletSpeed para mantener el comportamiento previo (si acceleration=0, no cambia nada).
        currentSpeed = bulletSpeed;

        // MOD: si el maxSpeed viene mal configurado en inspector, evitamos que sea menor que la velocidad inicial.
        if (maxSpeed < currentSpeed)
            maxSpeed = currentSpeed;
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
        if (hasHit)
            return;

        // MOD: aplicamos aceleración en FixedUpdate para sincronizar con la física.
        if (useAcceleration && acceleration > 0f)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);
        }
        else
        {
            // MOD: si no hay aceleración, usamos exactamente la velocidad “clásica”.
            currentSpeed = bulletSpeed;
        }

        if (targetHealth != null)
        {
            Vector3 direction = (targetHealth.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * currentSpeed; // MOD: antes era bulletSpeed.

            if (direction.magnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            rb.linearVelocity = transform.forward * currentSpeed; // MOD: antes era bulletSpeed.
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
        Debug.Log($"Colisión con: {collision.collider.name}");

        if (hasHit)
            return;

        hasHit = true;

        EnemyBaseHealth enemy =
            collision.collider.GetComponentInParent<EnemyBaseHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damageAmount);
        }

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
        // Mantiene la API original.
        bulletSpeed = speed;

        // MOD: hacemos que SetSpeed también reinicie la velocidad actual,
        // para que los disparos “respeten” inmediatamente el nuevo valor.
        currentSpeed = speed;

        // MOD: y corregimos maxSpeed si quedó por debajo.
        if (maxSpeed < currentSpeed)
            maxSpeed = currentSpeed;
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
