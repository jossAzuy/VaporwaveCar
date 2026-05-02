using UnityEngine;
using MoreMountains.Feedbacks;

/// <summary>
/// Componente para minas colocadas por el jugador
/// Explota cuando un enemigo entra en su rango
/// </summary>
public class Mine : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int damage = 3;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float armDelay = 0.5f; // Tiempo antes de que la mina esté activa

    [Header("Detección")]
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private float detectionInterval = 0.2f;

    [Header("Feedback")]
    [SerializeField] private MMFeedbacks explosionFeedback;
    [SerializeField] private GameObject explosionVFX; // Efecto visual de explosión

    [Header("Visual")]
    [SerializeField] private GameObject mineVisual;
    [SerializeField] private Color armedColor = Color.red;
    [SerializeField] private Color unarmedColor = Color.yellow;

    private bool isArmed = false;
    private bool hasExploded = false;
    private Renderer mineRenderer;

    private void Start()
    {
        // Obtener el renderer para cambiar color
        if (mineVisual != null)
        {
            mineRenderer = mineVisual.GetComponent<Renderer>();
        }
        else
        {
            mineRenderer = GetComponentInChildren<Renderer>();
        }

        // Iniciar desarmada
        UpdateVisual(false);

        // Armar la mina después del delay
        Invoke(nameof(ArmMine), armDelay);

        // Iniciar detección de enemigos
        InvokeRepeating(nameof(DetectEnemies), armDelay, detectionInterval);
    }

    private void ArmMine()
    {
        isArmed = true;
        UpdateVisual(true);
        Debug.Log("Mina armada");
    }

    private void DetectEnemies()
    {
        if (!isArmed || hasExploded)
        {
            CancelInvoke(nameof(DetectEnemies));
            return;
        }

        // Buscar enemigos en el rango de detección
        Collider[] enemiesInRange = Physics.OverlapSphere(
            transform.position,
            detectionRadius,
            enemyLayer
        );

        if (enemiesInRange.Length > 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;
        CancelInvoke(nameof(DetectEnemies));

        Debug.Log("¡Mina explotando!");

        // Buscar todos los enemigos en el radio de explosión
        Collider[] enemiesInExplosion = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            enemyLayer
        );

        // Aplicar daño a todos los enemigos afectados
        foreach (Collider enemyCollider in enemiesInExplosion)
        {
            EnemyBaseHealth enemyHealth = enemyCollider.GetComponentInParent<EnemyBaseHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Enemigo dañado por explosión de mina: {damage} de daño");
            }
        }

        // Reproducir feedback de explosión
        explosionFeedback?.PlayFeedbacks();

        // Instanciar efecto visual de explosión
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        // Destruir la mina después de un pequeño delay
        Destroy(gameObject, 0.1f);
    }

    private void UpdateVisual(bool armed)
    {
        if (mineRenderer != null)
        {
            mineRenderer.material.color = armed ? armedColor : unarmedColor;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar radio de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Dibujar radio de explosión
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // Trigger opcional para detección alternativa
    private void OnTriggerEnter(Collider other)
    {
        if (!isArmed || hasExploded) return;

        // Verificar si es un enemigo
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            Explode();
        }
    }
}
