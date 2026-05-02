using UnityEngine;
using MoreMountains.Feedbacks;

public class AutoFireSystem : MonoBehaviour
{
    [Header("Rango")]
    [SerializeField] private float fireRange = 15f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Disparo")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Feedback")]
    [SerializeField] private MMFeedbacks fireFeedback;

    private float fireTimer;
    private Collider[] enemyColliders = new Collider[50];

    private void Update()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            TryFire();
            fireTimer = fireRate;
        }
    }

    void TryFire()
    {
        EnemyBaseHealth target = FindNearestEnemy();
        if (target == null) return;

        FireAt(target);
    }

    EnemyBaseHealth FindNearestEnemy()
    {
        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            fireRange,
            enemyColliders,
            enemyLayer
        );

        float nearest = float.MaxValue;
        EnemyBaseHealth result = null;

        for (int i = 0; i < count; i++)
        {
            EnemyBaseHealth enemy =
                enemyColliders[i].GetComponentInParent<EnemyBaseHealth>();

            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < nearest)
            {
                nearest = dist;
                result = enemy;
            }
        }

        return result;
    }

    void FireAt(EnemyBaseHealth target)
    {
        fireFeedback?.PlayFeedbacks();

        Vector3 spawnPos = firePoint.position;
        Vector3 dir = (target.transform.position - spawnPos).normalized;

        GameObject bullet = Instantiate(
            projectilePrefab,
            spawnPos,
            Quaternion.LookRotation(dir)
        );

        
        // Asignar target y daño al proyectil.
        /* if (bullet.TryGetComponent<PlayerProjectile>(out var projectile))
        {
            projectile.SetTarget(target);
            projectile.SetDamage(bulletDamage);
        } */

        if (bullet.TryGetComponent<PlayerProjectileAccelerating>(out var projectileAccel))
        {
            projectileAccel.SetTarget(target);
            projectileAccel.SetDamage(bulletDamage);
        }

    }
}
