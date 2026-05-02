using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCarAI2_NavMesh : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Movimiento")]
    public float maxSpeed = 14f;
    public float acceleration = 8f;

    [Header("Giro")]
    public float steering = 120f;

    [Header("Distancias")]
    public float stopDistance = 3f;
    public float slowDownDistance = 8f;

    [Header("Variación / Inteligencia")]
    public float orbitRadius = 4f;
    public float orbitSpeed = 1f;

    [Range(0f, 1f)]
    public float steeringNoise = 0.15f;

    NavMeshAgent agent;
    float orbitOffset;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = maxSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = 0f; // rotación manual
        agent.stoppingDistance = stopDistance;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        orbitOffset = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (!target) return;

        // --- OBJETIVO ORBITAL ---
        Vector3 baseTarget = target.position;

        float angle = Time.time * orbitSpeed + orbitOffset;
        Vector3 orbitOffsetVector = new Vector3(
            Mathf.Cos(angle),
            0f,
            Mathf.Sin(angle)
        ) * orbitRadius;

        Vector3 desiredTarget = baseTarget + orbitOffsetVector;

        // Asegura que el destino esté dentro del NavMesh
        if (NavMesh.SamplePosition(desiredTarget, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        HandleRotation();
        HandleSlowDown();
    }

    void HandleRotation()
    {
        if (agent.velocity.sqrMagnitude < 0.1f) return;

        Vector3 dir = agent.velocity.normalized;

        float angleToTarget = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

        float steerInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

        float noise = (Mathf.PerlinNoise(Time.time, orbitOffset) - 0.5f) * 2f;
        steerInput += noise * steeringNoise;

        float turn = steerInput * steering * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + turn, 0f);
    }

    void HandleSlowDown()
    {
        float distance = Vector3.Distance(transform.position, agent.destination);

        if (distance < slowDownDistance)
        {
            agent.speed = Mathf.Lerp(0f, maxSpeed, distance / slowDownDistance);
        }
        else
        {
            agent.speed = maxSpeed;
        }
    }
}
