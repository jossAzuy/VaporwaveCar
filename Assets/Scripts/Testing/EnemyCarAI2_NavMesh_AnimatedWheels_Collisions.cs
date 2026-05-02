using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCarAI_PhysicsNavMesh_Full : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Movimiento")]
    public float maxSpeed = 14f;
    public float acceleration = 25f;

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
    public Transform[] frontWheelPivots;
    public Transform[] frontWheelMeshes;
    public Transform[] rearWheelMeshes;

    public float wheelRotationSpeed = 300f;
    public float maxWheelSteerAngle = 30f;

    Rigidbody rb;
    NavMeshAgent agent;

    float orbitOffset;
    float steerInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        // ---------- Rigidbody ----------
        rb.useGravity = false;
        rb.linearDamping = 1.5f;
        rb.angularDamping = 999f;
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        // ---------- NavMesh ----------
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        orbitOffset = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (!target) return;

        // ⭐ REINTEGRADO
        // Objetivo orbital (tu lógica original)
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

        HandleRotation();
        //HandleSlowDown();
        AnimateWheels();
    }

    void FixedUpdate()
    {
        // ⭐ REINTEGRADO
        // Dirección base del NavMesh
        Vector3 desiredVel = agent.desiredVelocity;
        desiredVel.y = 0f;

        float desiredSpeed = maxSpeed;

        // ⭐ REINTEGRADO
        // Distancias → control de velocidad
        float distance = Vector3.Distance(transform.position, agent.destination);

        if (distance < stopDistance)
            desiredSpeed = 0f;
        else if (distance < slowDownDistance)
            desiredSpeed = Mathf.Lerp(0f, maxSpeed, distance / slowDownDistance);

        Vector3 desiredDir = desiredVel.normalized;

        // ⭐ REINTEGRADO
        // Variación / ruido de dirección
        float noise = (Mathf.PerlinNoise(Time.time, orbitOffset) - 0.5f) * 2f;
        Vector3 noiseDir = Quaternion.Euler(0f, noise * steeringNoise * 30f, 0f) * desiredDir;

        Vector3 targetVelocity = noiseDir * desiredSpeed;

        // ⭐ REINTEGRADO
        // Física real: acelerar hacia la velocidad objetivo
        Vector3 velocityDelta = targetVelocity - rb.linearVelocity;
        velocityDelta.y = 0f;

        rb.AddForce(velocityDelta * acceleration, ForceMode.Acceleration);

        // Limitar velocidad máxima
        Vector3 flatVel = rb.linearVelocity;
        flatVel.y = 0f;

        if (flatVel.magnitude > maxSpeed)
        {
            flatVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flatVel.x, 0f, flatVel.z);
        }

        // Sincronizar NavMesh
        agent.nextPosition = rb.position;
    }

    void HandleRotation()
    {
        Vector3 flatVel = rb.linearVelocity;
        flatVel.y = 0f;

        if (flatVel.sqrMagnitude < 0.1f) return;

        float angleToTarget = Vector3.SignedAngle(transform.forward, flatVel, Vector3.up);
        steerInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

        float turn = steerInput * steering * Time.deltaTime;

        transform.rotation = Quaternion.Euler(
            0f,
            transform.eulerAngles.y + turn,
            0f
        );
    }

    // ================= ANIMACIÓN LLANTAS =================

    void AnimateWheels()
    {
        Vector3 flatVelocity = rb.linearVelocity;
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
