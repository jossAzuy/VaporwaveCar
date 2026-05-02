using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class CarController_NavMeshBounceAnimatedWheels : MonoBehaviour
{
    [Header("Movimiento")]
    public float acceleration = 800f;
    public float maxSpeed = 15f;
    public float drag = 3f;

    [Header("Giro")]
    public float steering = 120f;

    [Header("Llantas")]
    [Tooltip("Pivots de las llantas delanteras (solo giran en Y)")]
    public Transform[] frontWheelPivots;

    [Tooltip("Meshes de las llantas delanteras (solo ruedan en X)")]
    public Transform[] frontWheelMeshes;

    [Tooltip("Meshes de las llantas traseras (solo ruedan en X)")]
    public Transform[] rearWheelMeshes;

    public float wheelRotationSpeed = 300f;
    public float maxWheelSteerAngle = 30f;

    [Header("Rebote en bordes")]
    [Range(0f, 1f)]
    public float bounceElasticity = 0.6f;
    public float positionCorrectionSpeed = 12f;
    public float navMeshCheckDistance = 1f;

    Rigidbody rb;
    float inputV;
    float inputH;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearDamping = drag;
        rb.angularDamping = 0f;

        // Congelar rotaciones para evitar vuelcos
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
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

    void Steer()
    {
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);

        if (speedFactor > 0.05f)
        {
            float turn = inputH * steering * speedFactor * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }
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

    // ================= ANIMACIÓN LLANTAS =================

    void AnimateWheels()
    {
        Vector3 flatVelocity = rb.linearVelocity;
        flatVelocity.y = 0f;

        // Velocidad firmada (adelante / reversa)
        float signedSpeed = Vector3.Dot(flatVelocity, transform.forward);
        float rollRotation = signedSpeed * wheelRotationSpeed * Time.fixedDeltaTime;

        // Rodado (SOLO meshes)
        foreach (Transform wheel in frontWheelMeshes)
            wheel.Rotate(Vector3.right, rollRotation, Space.Self);

        foreach (Transform wheel in rearWheelMeshes)
            wheel.Rotate(Vector3.right, rollRotation, Space.Self);

        // Dirección (SOLO pivots delanteros)
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
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshCheckDistance, NavMesh.AllAreas))
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
