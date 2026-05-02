using UnityEngine;

public class PoliceSirenManager : MonoBehaviour
{
    public float maxAudioDistance = 40f;

    private PoliceSirenController[] patrols;
    private Transform player;
    private PoliceSirenController currentActivePatrol;

    void Start()
    {
        patrols = FindObjectsByType<PoliceSirenController>(FindObjectsSortMode.None);

        // Its IMPORTANT that the player has the "Player" tag assigned.
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        PoliceSirenController closestPatrol = null;
        float closestDistance = Mathf.Infinity;

        foreach (var patrol in patrols)
        {
            float distance = Vector3.Distance(
                patrol.transform.position,
                player.position
            );

            if (distance < closestDistance && distance <= maxAudioDistance)
            {
                closestDistance = distance;
                closestPatrol = patrol;
            }
        }

        if (closestPatrol != currentActivePatrol)
        {
            if (currentActivePatrol != null)
                currentActivePatrol.StopSiren();

            currentActivePatrol = closestPatrol;

            if (currentActivePatrol != null)
                currentActivePatrol.StartSiren();
        }
    }
}
