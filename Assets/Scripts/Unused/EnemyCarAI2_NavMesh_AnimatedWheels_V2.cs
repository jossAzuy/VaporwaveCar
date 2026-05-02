using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCarAI2_NavMesh_AnimatedWheels_V2 : MonoBehaviour
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

    [Header("Repath")]
    public float repathInterval = 0.3f;

    [Header("Llantas")]
    public Transform[] frontWheelPivots;
    public Transform[] frontWheelMeshes;
    public Transform[] rearWheelMeshes;

    public float wheelRotationSpeed = 300f;
    public float maxWheelSteerAngle = 30f;

    NavMeshAgent agent;
    float orbitOffset;
    float steerInput;
    float repathTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = maxSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = 0f;
        agent.stoppingDistance = stopDistance;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        orbitOffset = Random.Range(0f, 360f);
        repathTimer = Random.Range(0f, repathInterval);
    }

    void Update()
    {
        if (!target) return;

        UpdateDestination();
        HandleRotation();
        HandleSlowDown();
        AnimateWheels();
    }

    // ================= DESTINO ORBITAL =================
    void UpdateDestination()
    {
        repathTimer -= Time.deltaTime;
        if (repathTimer > 0f) return;

        repathTimer = repathInterval;

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
        if (agent.desiredVelocity.sqrMagnitude < 0.1f) return;

        Vector3 dir = agent.desiredVelocity.normalized;
        float angleToDir = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

        steerInput = Mathf.Clamp(angleToDir / 45f, -1f, 1f);

        float noise = (Mathf.PerlinNoise(Time.time, orbitOffset) - 0.5f) * 2f;
        steerInput += noise * steeringNoise;

        float turn = steerInput * steering * Time.deltaTime;
        transform.rotation = Quaternion.Euler(
            0f,
            transform.eulerAngles.y + turn,
            0f
        );
    }

    // ================= VELOCIDAD / DISTANCIA =================
    void HandleSlowDown()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float effectiveDistance = Mathf.Max(0f, distanceToTarget - orbitRadius);

        if (effectiveDistance < slowDownDistance)
        {
            agent.speed = Mathf.Lerp(0f, maxSpeed, effectiveDistance / slowDownDistance);
        }
        else
        {
            agent.speed = maxSpeed;
        }
    }

    // ================= ANIMACIÓN LLANTAS =================
    void AnimateWheels()
    {
        Vector3 flatVelocity = agent.velocity;
        flatVelocity.y = 0f;

        float signedSpeed = Vector3.Dot(flatVelocity, transform.forward);
        float rollRotation = signedSpeed * wheelRotationSpeed * Time.deltaTime;

        foreach (Transform wheel in frontWheelMeshes)
            wheel.Rotate(Vector3.right, rollRotation, Space.Self);

        foreach (Transform wheel in rearWheelMeshes)
            wheel.Rotate(Vector3.right, rollRotation, Space.Self);

        float steerAngle = steerInput * maxWheelSteerAngle;

        foreach (Transform pivot in frontWheelPivots)
        {
            Vector3 euler = pivot.localEulerAngles;
            euler.y = steerAngle;
            pivot.localEulerAngles = euler;
        }
    }
}
