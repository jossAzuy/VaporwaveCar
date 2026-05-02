using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyCarAI2 : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Movimiento")]
    public float acceleration = 700f;
    public float maxSpeed = 14f;
    public float drag = 3f;

    [Header("Giro")]
    public float steering = 100f;

    [Header("Distancias")]
    public float stopDistance = 3f;
    public float slowDownDistance = 8f;

    [Header("Variación / Inteligencia")]
    [Tooltip("Distancia alrededor del jugador que este enemigo intenta ocupar")]
    public float orbitRadius = 4f;

    [Tooltip("Velocidad a la que rota alrededor del jugador")]
    public float orbitSpeed = 1f;

    [Tooltip("Cuánto error humano tiene el giro (0 = perfecto)")]
    [Range(0f, 1f)]
    public float steeringNoise = 0.15f;

    Rigidbody rb;
    float orbitOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearDamping = drag;
        rb.angularDamping = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Identidad única por enemigo
        orbitOffset = Random.Range(0f, 360f);
    }

    void FixedUpdate()
    {
        if (!target) return;

        // --- OBJETIVO VARIADO ---
        Vector3 baseTarget = target.position;

        float angle = Time.time * orbitSpeed + orbitOffset;
        Vector3 orbitOffsetVector = new Vector3(
            Mathf.Cos(angle),
            0f,
            Mathf.Sin(angle)
        ) * orbitRadius;

        Vector3 desiredTarget = baseTarget + orbitOffsetVector;

        Vector3 toTarget = desiredTarget - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;
        Vector3 dir = toTarget.normalized;

        HandleSteering(dir);
        HandleMovement(distance);
        LimitSpeed();
    }

    void HandleSteering(Vector3 dir)
    {
        float angleToTarget = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);

        float steerInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

        // Ruido para evitar comportamiento robótico
        float noise = (Mathf.PerlinNoise(Time.time, orbitOffset) - 0.5f) * 2f;
        steerInput += noise * steeringNoise;

        float turn = steerInput * steering * speedFactor * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
    }

    void HandleMovement(float distance)
    {
        if (distance <= stopDistance)
            return;

        float throttle = 1f;

        if (distance < slowDownDistance)
        {
            throttle = Mathf.InverseLerp(stopDistance, slowDownDistance, distance);
        }

        rb.AddForce(
            transform.forward * throttle * acceleration * Time.fixedDeltaTime,
            ForceMode.VelocityChange
        );
    }

    void LimitSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVelocity.magnitude > maxSpeed)
        {
            flatVelocity = flatVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flatVelocity.x, 0f, flatVelocity.z);
        }
    }
}
