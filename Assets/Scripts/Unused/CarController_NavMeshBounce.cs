using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class CarController_NavMeshBounce : MonoBehaviour
{
    [Header("Movimiento")]
    public float acceleration = 800f;
    public float maxSpeed = 15f;
    public float drag = 3f;

    [Header("Giro")]
    public float steering = 120f;

    [Header("Rebote en bordes")]
    [Tooltip("Qué tan elástico es el rebote (0 = no rebota, 1 = rebote perfecto)")]
    [Range(0f, 1f)]
    public float bounceElasticity = 0.6f;

    [Tooltip("Qué tan rápido se corrige la posición al tocar el borde")]
    public float positionCorrectionSpeed = 12f;

    [Tooltip("Distancia máxima para detectar borde del NavMesh")]
    public float navMeshCheckDistance = 1f;

    Rigidbody rb;
    float inputV;
    float inputH;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearDamping = drag;
        rb.angularDamping = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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
    }

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

    void HandleNavMeshBounce()
    {
        // Si estamos fuera del NavMesh, buscamos el punto más cercano
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshCheckDistance, NavMesh.AllAreas))
            return;

        Vector3 toNavMesh = hit.position - rb.position;
        float distance = toNavMesh.magnitude;

        if (distance < 0.01f)
            return;

        Vector3 normal = toNavMesh.normalized;

        // --- Rebote ---
        Vector3 flatVelocity = rb.linearVelocity;
        flatVelocity.y = 0f;

        // Solo rebotamos si vamos hacia afuera
        if (Vector3.Dot(flatVelocity, normal) < 0f)
        {
            Vector3 reflectedVelocity = Vector3.Reflect(flatVelocity, normal);
            rb.linearVelocity = Vector3.Lerp(
                flatVelocity,
                reflectedVelocity,
                bounceElasticity
            );
        }

        // --- Corrección suave de posición ---
        Vector3 correctedPos = Vector3.Lerp(
            rb.position,
            hit.position,
            positionCorrectionSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(correctedPos);
    }
}
