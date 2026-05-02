using UnityEngine;

public class CreditsPickup : MonoBehaviour
{
    
    [SerializeField] private int creditValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerPickupProgressUpdated progress = other.GetComponent<PlayerPickupProgressUpdated>();

        if (progress != null)
        {
            // Debug.Log("Crédito recogido");
            progress.AddCredit(creditValue);
        }

        Destroy(gameObject);

    }
}
