using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCarAI2_NavMesh_AnimatedWheels : MonoBehaviour
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

    [Header("Llantas")]
    [Tooltip("Pivots de las llantas delanteras (solo giran en Y)")]
    public Transform[] frontWheelPivots;

    [Tooltip("Meshes de las llantas delanteras (solo ruedan en X)")]
    public Transform[] frontWheelMeshes;

    [Tooltip("Meshes de las llantas traseras (solo ruedan en X)")]
    public Transform[] rearWheelMeshes;

    public float wheelRotationSpeed = 300f;
    public float maxWheelSteerAngle = 30f;

    NavMeshAgent agent;
    float orbitOffset;
    float steerInput;

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

        // ---------- OBJETIVO ORBITAL ----------
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
        AnimateWheels();
    }

    void HandleRotation()
    {
        if (agent.velocity.sqrMagnitude < 0.1f) return;

        Vector3 dir = agent.velocity.normalized;

        float angleToTarget = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

        steerInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

        float noise = (Mathf.PerlinNoise(Time.time, orbitOffset) - 0.5f) * 2f;
        steerInput += noise * steeringNoise;

        float turn = steerInput * steering * Time.deltaTime;
        transform.rotation = Quaternion.Euler(
            0f,
            transform.eulerAngles.y + turn,
            0f
        );
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

    // ================= ANIMACIÓN LLANTAS =================
    void AnimateWheels()
    {
        Vector3 flatVelocity = agent.velocity;
        flatVelocity.y = 0f;

        // Velocidad firmada (adelante / reversa)
        float signedSpeed = Vector3.Dot(flatVelocity, transform.forward);
        float rollRotation = signedSpeed * wheelRotationSpeed * Time.deltaTime;

        // Rodado (SOLO meshes)
        foreach (Transform wheel in frontWheelMeshes)
            wheel.Rotate(Vector3.right, rollRotation, Space.Self);

        foreach (Transform wheel in rearWheelMeshes)
            wheel.Rotate(Vector3.right, rollRotation, Space.Self);

        // Dirección (SOLO pivots delanteros)
        float steerAngle = steerInput * maxWheelSteerAngle;

        foreach (Transform pivot in frontWheelPivots)
        {
            Vector3 euler = pivot.localEulerAngles;
            euler.y = steerAngle;
            pivot.localEulerAngles = euler;
        }
    }


}
