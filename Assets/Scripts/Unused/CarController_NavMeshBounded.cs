using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class CarController_NavMeshBounded : MonoBehaviour
{
    [Header("Movimiento")]
    public float acceleration = 800f;
    public float maxSpeed = 15f;
    public float drag = 3f;

    [Header("Giro")]
    public float steering = 120f;

    [Header("NavMesh Límite")]
    [Tooltip("Distancia máxima permitida fuera del NavMesh antes de corregir")]
    public float maxNavMeshOffset = 0.5f;

    [Tooltip("Qué tan fuerte se corrige la posición")]
    public float correctionSpeed = 15f;

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
        ClampToNavMesh();
    }

    void Move()
    {
        if (inputV != 0)
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

    void ClampToNavMesh()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxNavMeshOffset, NavMesh.AllAreas))
        {
            float distance = Vector3.Distance(transform.position, hit.position);

            // Si está fuera del NavMesh → corregimos suavemente
            if (distance > 0.01f)
            {
                Vector3 correctedPos = Vector3.Lerp(
                    rb.position,
                    hit.position,
                    correctionSpeed * Time.fixedDeltaTime
                );

                rb.MovePosition(correctedPos);

                // Cancelamos velocidad hacia afuera
                Vector3 flatVel = rb.linearVelocity;
                flatVel.y = 0f;

                Vector3 dirToNavMesh = (hit.position - rb.position).normalized;
                if (Vector3.Dot(flatVel, dirToNavMesh) < 0f)
                {
                    rb.linearVelocity = Vector3.zero;
                }
            }
        }
    }
}
