using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyCarAI : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Movimiento")]
    public float acceleration = 700f;
    public float maxSpeed = 14f;
    public float drag = 3f;

    [Header("Giro")]
    public float steering = 100f;

    [Header("Comportamiento")]
    public float stopDistance = 3f;      // Distancia mínima al jugador
    public float slowDownDistance = 8f;  // Empieza a frenar aquí

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearDamping = drag;
        rb.angularDamping = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (!target) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0;

        float distance = toTarget.magnitude;
        Vector3 dir = toTarget.normalized;

        HandleSteering(dir);
        HandleMovement(distance);
        LimitSpeed();
    }

    void HandleSteering(Vector3 dir)
    {
        float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);

        float steerInput = Mathf.Clamp(angle / 45f, -1f, 1f);
        float turn = steerInput * steering * speedFactor * Time.fixedDeltaTime;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
    }

    void HandleMovement(float distance)
    {
        float throttle = 1f;

        if (distance < slowDownDistance)
        {
            throttle = Mathf.InverseLerp(stopDistance, slowDownDistance, distance);
        }

        if (distance > stopDistance)
        {
            rb.AddForce(transform.forward * throttle * acceleration * Time.fixedDeltaTime,
                        ForceMode.VelocityChange);
        }
    }

    void LimitSpeed()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > maxSpeed)
        {
            flatVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flatVel.x, 0f, flatVel.z);
        }
    }
}
