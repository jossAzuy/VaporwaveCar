using UnityEngine;

public class CreditMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    [SerializeField] private float magnetRadius = 5f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float pickupDistance = 0.3f;

    private Transform player;
    private PlayerPickupProgressUpdated progress;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        progress = FindFirstObjectByType<PlayerPickupProgressUpdated>();
    }

    void Update()
    {
        if (player == null || progress == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= magnetRadius)
        {
            // Movimiento magnético (suave)
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );

            // Pickup
            if (distance <= pickupDistance)
            {
                progress.AddCredit(1);
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
