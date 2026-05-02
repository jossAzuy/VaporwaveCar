using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movimiento")]
    public float acceleration = 800f;
    public float maxSpeed = 15f;
    public float drag = 3f;

    [Header("Giro")]
    public float steering = 120f;

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
        inputV = Input.GetAxisRaw("Vertical");   // W / S
        inputH = Input.GetAxisRaw("Horizontal"); // A / D
    }

    void FixedUpdate()
    {
        Move();
        Steer();
        LimitSpeed();
    }

    void Move()
    {
        if (inputV != 0)
        {
            rb.AddForce(transform.forward * inputV * acceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    void Steer()
    {
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        speedFactor = Mathf.Clamp01(speedFactor);

        if (speedFactor > 0.05f)
        {
            float turn = inputH * steering * speedFactor * Time.fixedDeltaTime;
            Quaternion rot = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * rot);
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
}
