using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarAudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource engineSource;
    public AudioSource brakeSource;
    public AudioSource skidSource;

    [Header("Engine Settings")]
    public float minEnginePitch = 0.8f;
    public float maxEnginePitch = 1.6f;
    public float engineVolume = 0.6f;

    [Header("Brake Settings")]
    public float brakeVolume = 0.7f;

    [Header("Skid Settings")]
    public float skidVolume = 0.8f;
    public float skidThreshold = 0.4f; // Qué tan lateral debe ser el movimiento

    [Header("Speed Reference")]
    public float maxSpeed = 15f;

    Rigidbody rb;
    float inputV;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Asegurar loops
        engineSource.loop = true;
        brakeSource.loop = true;
        skidSource.loop = true;

        engineSource.volume = 0f;
        brakeSource.volume = 0f;
        skidSource.volume = 0f;

        engineSource.Play();
        brakeSource.Play();
        skidSource.Play();
    }

    void Update()
    {
        inputV = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        UpdateEngineSound();
        UpdateBrakeSound();
        UpdateSkidSound();
    }

    // ================= ENGINE =================

    void UpdateEngineSound()
    {
        float speed = rb.linearVelocity.magnitude;
        float speed01 = Mathf.Clamp01(speed / maxSpeed);

        // Pitch dinámico
        engineSource.pitch = Mathf.Lerp(
            minEnginePitch,
            maxEnginePitch,
            speed01
        );

        // Volumen según aceleración
        float targetVolume = Mathf.Abs(inputV) > 0.1f ? engineVolume : engineVolume * 0.5f;
        engineSource.volume = Mathf.Lerp(
            engineSource.volume,
            targetVolume,
            5f * Time.fixedDeltaTime
        );
    }

    // ================= BRAKE =================

    void UpdateBrakeSound()
    {
        float speed = rb.linearVelocity.magnitude;

        bool braking =
            speed > 2f &&
            (
                inputV < -0.1f ||   // reversa
                Mathf.Abs(inputV) < 0.05f // soltar acelerador
            );

        float targetVolume = braking ? brakeVolume : 0f;

        brakeSource.volume = Mathf.Lerp(
            brakeSource.volume,
            targetVolume,
            8f * Time.fixedDeltaTime
        );
    }

    // ================= SKID =================

    void UpdateSkidSound()
    {
        Vector3 flatVelocity = rb.linearVelocity;
        flatVelocity.y = 0f;

        if (flatVelocity.magnitude < 2f)
        {
            skidSource.volume = Mathf.Lerp(skidSource.volume, 0f, 10f * Time.fixedDeltaTime);
            return;
        }

        Vector3 forward = transform.forward.normalized;
        Vector3 velocityDir = flatVelocity.normalized;

        // Producto cruz = derrape
        float lateralSlip = Mathf.Abs(Vector3.Dot(transform.right, velocityDir));

        bool skidding = lateralSlip > skidThreshold;

        float targetVolume = skidding ? skidVolume * lateralSlip : 0f;

        skidSource.volume = Mathf.Lerp(
            skidSource.volume,
            targetVolume,
            10f * Time.fixedDeltaTime
        );

        skidSource.pitch = Mathf.Lerp(0.9f, 1.2f, lateralSlip);
    }
}
