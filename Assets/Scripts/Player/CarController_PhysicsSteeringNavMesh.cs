using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class CarController_PhysicsSteeringNavMesh : MonoBehaviour
{
    [Header("Movimiento")]
    public float acceleration = 800f;
    public float maxSpeed = 15f;
    public float linearDrag = 3f;

    [Header("Giro")]
    public float steeringTorque = 80f;
    public float angularDrag = 6f;

    [Header("Agarre")]
    public float lateralDrag = 5f;

    [Header("Llantas")]
    [Tooltip("Pivots de las llantas delanteras (solo giran en Y)")]
    public Transform[] frontWheelPivots;

    [Tooltip("Meshes de las llantas delanteras (solo ruedan en X)")]
    public Transform[] frontWheelMeshes;

    [Tooltip("Meshes de las llantas traseras (solo ruedan en X)")]
    public Transform[] rearWheelMeshes;

    public float wheelRotationSpeed = 300f;
    public float maxWheelSteerAngle = 30f;

    [Header("Rebote en bordes (NavMesh)")]
    [Range(0f, 1f)]
    public float bounceElasticity = 0.6f;
    public float positionCorrectionSpeed = 12f;
    public float navMeshCheckDistance = 1f;

    Rigidbody rb;
    float inputV;
    float inputH;

    // ================= INIT =================

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Evitar vuelcos
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        rb.useGravity = false;
        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDrag;
    }

    void Start()
    {
        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDrag;
    }

    void Update()
    {
        inputV = Input.GetAxisRaw("Vertical");
        inputH = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        Move();
        Steer();
        ApplyLateralDrag();
        LimitSpeed();
        HandleNavMeshBounce();
        AnimateWheels();
    }

    // ================= MOVIMIENTO =================

    void Move()
    {
        if (Mathf.Abs(inputV) > 0.01f)
        {
            rb.AddForce(
                transform.forward * inputV * acceleration * Time.fixedDeltaTime,
                ForceMode.VelocityChange
            );
        }
    }

    // ================= GIRO FÍSICO =================

    void Steer()
    {
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);

        if (speedFactor > 0.05f)
        {
            float torque = inputH * steeringTorque * speedFactor;
            rb.AddTorque(Vector3.up * torque, ForceMode.Acceleration);
        }
    }

    // ================= AGARRE LATERAL =================

    void ApplyLateralDrag()
    {
        Vector3 velocity = rb.linearVelocity;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float forwardSpeed = Vector3.Dot(velocity, forward);
        float lateralSpeed = Vector3.Dot(velocity, right);

        Vector3 forwardVelocity = forward * forwardSpeed;
        Vector3 lateralVelocity = right * lateralSpeed;

        lateralVelocity *= Mathf.Clamp01(1f - lateralDrag * Time.fixedDeltaTime);

        rb.linearVelocity = forwardVelocity + lateralVelocity;
    }

    // ================= LIMITADOR DE VELOCIDAD =================

    void LimitSpeed()
    {
        Vector3 flatVelocity = rb.linearVelocity;
        flatVelocity.y = 0f;

        if (flatVelocity.magnitude > maxSpeed)
        {
            flatVelocity = flatVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flatVelocity.x, 0f, flatVelocity.z);
        }
    }

    // ================= ANIMACIÓN LLANTAS =================

    void AnimateWheels()
    {
        Vector3 flatVelocity = rb.linearVelocity;
        flatVelocity.y = 0f;

        float signedSpeed = Vector3.Dot(flatVelocity, transform.forward);
        float rollRotation = signedSpeed * wheelRotationSpeed * Time.fixedDeltaTime;

        foreach (Transform wheel in frontWheelMeshes)
            wheel.Rotate(Vector3.right, rollRotation, Space.Self);

        foreach (Transform wheel in rearWheelMeshes)
            wheel.Rotate(Vector3.right, rollRotation, Space.Self);

        float steerAngle = inputH * maxWheelSteerAngle;

        foreach (Transform pivot in frontWheelPivots)
        {
            Vector3 euler = pivot.localEulerAngles;
            euler.y = steerAngle;
            pivot.localEulerAngles = euler;
        }
    }

    // ================= NAVMESH BOUNCE =================

    void HandleNavMeshBounce()
    {
        if (!NavMesh.SamplePosition(
            transform.position,
            out NavMeshHit hit,
            navMeshCheckDistance,
            NavMesh.AllAreas))
            return;

        Vector3 toNavMesh = hit.position - rb.position;
        float distance = toNavMesh.magnitude;

        if (distance < 0.01f)
            return;

        Vector3 normal = toNavMesh.normalized;

        Vector3 flatVelocity = rb.linearVelocity;
        flatVelocity.y = 0f;

        if (Vector3.Dot(flatVelocity, normal) < 0f)
        {
            Vector3 reflectedVelocity = Vector3.Reflect(flatVelocity, normal);
            rb.linearVelocity = Vector3.Lerp(
                flatVelocity,
                reflectedVelocity,
                bounceElasticity
            );
        }

        Vector3 correctedPos = Vector3.Lerp(
            rb.position,
            hit.position,
            positionCorrectionSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(correctedPos);
    }
}
