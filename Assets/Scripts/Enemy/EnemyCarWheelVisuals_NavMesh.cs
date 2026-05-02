using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyCarWheelVisuals_NavMesh : MonoBehaviour
{
    [Header("Llantas")]
    [Tooltip("Pivots de las llantas delanteras (solo giran en Y)")]
    public Transform[] frontWheelPivots;

    [Tooltip("Meshes de las llantas delanteras (solo ruedan en X)")]
    public Transform[] frontWheelMeshes;

    [Tooltip("Meshes de las llantas traseras (solo ruedan en X)")]
    public Transform[] rearWheelMeshes;

    [Header("Parámetros Visuales")]
    public float wheelRotationSpeed = 300f;
    public float maxWheelSteerAngle = 30f;
    public float steerSmoothing = 8f;

    NavMeshAgent agent;
    float currentSteer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        AnimateWheels();
    }

    // ================= ANIMACIÓN LLANTAS =================
    void AnimateWheels()
    {
        Vector3 flatVelocity = agent.velocity;
        flatVelocity.y = 0f;

        // -------- RODADO --------
        float signedSpeed = Vector3.Dot(flatVelocity, transform.forward);
        float rollRotation = signedSpeed * wheelRotationSpeed * Time.deltaTime;

        foreach (Transform wheel in frontWheelMeshes)
            wheel.Rotate(Vector3.right, rollRotation, Space.Self);

        foreach (Transform wheel in rearWheelMeshes)
            wheel.Rotate(Vector3.right, rollRotation, Space.Self);

        // -------- GIRO VISUAL --------
        float targetSteer = 0f;

        if (flatVelocity.sqrMagnitude > 0.05f)
        {
            Vector3 moveDir = flatVelocity.normalized;
            float angle = Vector3.SignedAngle(transform.forward, moveDir, Vector3.up);

            targetSteer = Mathf.Clamp(
                angle / 45f,
                -1f,
                1f
            ) * maxWheelSteerAngle;
        }

        currentSteer = Mathf.Lerp(
            currentSteer,
            targetSteer,
            steerSmoothing * Time.deltaTime
        );

        foreach (Transform pivot in frontWheelPivots)
        {
            Vector3 euler = pivot.localEulerAngles;
            euler.y = currentSteer;
            pivot.localEulerAngles = euler;
        }
    }
}
