using UnityEngine;
using UnityEngine.AI;


public class EnemyReward : MonoBehaviour
{
    /*  [SerializeField] private int creditsOnDeath = 5;

     public void GiveReward()
     {
         PlayerPickupProgress_ progress = FindFirstObjectByType<PlayerPickupProgress_>();
         if (progress != null)
         {
             progress.AddCredit(creditsOnDeath);
         }
     } */

    /* [SerializeField] private GameObject creditPrefab;
    [SerializeField] private int creditAmount = 5;

    public void GiveReward()
    {
        for (int i = 0; i < creditAmount; i++)
        {
            Instantiate(
                creditPrefab,
                transform.position + Random.insideUnitSphere,
                Quaternion.identity
            );
        }
    } */

    [SerializeField] private GameObject creditPrefab;
    [SerializeField] private int creditAmount = 5;

    public void GiveReward()
    {
        for (int i = 0; i < creditAmount; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * 2f;
            randomPos.y = transform.position.y;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                Instantiate(
                    creditPrefab,
                    hit.position,
                    Quaternion.identity
                );
            }
        }
    }
}
