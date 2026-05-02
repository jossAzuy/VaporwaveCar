using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCarNavMeshDriver : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Movimiento")]
    public float maxSpeed = 14f;
    public float acceleration = 10f;

    [Header("Giro")]
    public float steeringSpeed = 120f;

    [Header("Distancias")]
    public float stopDistance = 3f;
    public float slowDownDistance = 8f;

    [Header("Comportamiento")]
    public float orbitRadius = 4f;
    public float orbitSpeed = 1f;

    [Range(0f, 1f)]
    public float steeringNoise = 0.1f;

    NavMeshAgent agent;
    float orbitOffset;
    float currentSpeed;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = maxSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = 0f;
        agent.stoppingDistance = stopDistance;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = false; // MODIFICADO: el movimiento se controla manualmente

        orbitOffset = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (!target) return;

        UpdateDestination();
        HandleRotation();
        HandleSpeed();
        HandleMovement();
    }

    // ================= DESTINO =================
    void UpdateDestination()
    {
        Vector3 baseTarget = target.position;

        float angle = Time.time * orbitSpeed + orbitOffset;
        Vector3 orbitOffsetVector = new Vector3(
            Mathf.Cos(angle),
            0f,
            Mathf.Sin(angle)
        ) * orbitRadius;

        Vector3 desiredTarget = baseTarget + orbitOffsetVector;

        if (NavMesh.SamplePosition(desiredTarget, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // ================= ROTACIÓN =================
    void HandleRotation()
    {
        Vector3 steeringTarget = agent.steeringTarget - transform.position;
        steeringTarget.y = 0f;

        if (steeringTarget.sqrMagnitude < 0.01f) return;

        float angleToTarget = Vector3.SignedAngle(
            transform.forward,
            steeringTarget.normalized,
            Vector3.up
        );

        float steerInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

        float noise = (Mathf.PerlinNoise(Time.time, orbitOffset) - 0.5f) * 2f;
        steerInput += noise * steeringNoise;

        float turn = steerInput * steeringSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(
            0f,
            transform.eulerAngles.y + turn,
            0f
        );
    }

    // ================= VELOCIDAD =================
    void HandleSpeed()
    {
        float distance = Vector3.Distance(transform.position, agent.destination);
        float targetSpeed = maxSpeed;

        if (distance < slowDownDistance)
            targetSpeed = Mathf.Lerp(0f, maxSpeed, distance / slowDownDistance);

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            acceleration * Time.deltaTime
        );
    }

    // ================= MOVIMIENTO =================
    void HandleMovement()
    {
        Vector3 move = transform.forward * currentSpeed * Time.deltaTime;
        Vector3 nextPos = transform.position + move;

        // AÑADIDO: se fuerza la posición dentro del NavMesh
        if (NavMesh.SamplePosition(nextPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        else
        {
            // AÑADIDO: fallback de seguridad si no se encuentra NavMesh cercano
            transform.position = agent.nextPosition;
        }

        // MODIFICADO: se mantiene sincronizado el agente con la posición real
        agent.nextPosition = transform.position;
    }
}
