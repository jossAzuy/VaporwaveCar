using UnityEngine;
using UnityEngine.AI;

public class EnemyWheelAnimator : MonoBehaviour
{
    public EnemyCarNavMesh car;

    public Transform[] frontWheelPivots;
    public Transform[] frontWheelMeshes;
    public Transform[] rearWheelMeshes;

    public float wheelRotationMultiplier = 360f; // grados por metro
    public float maxWheelSteerAngle = 30f;

    Vector3 lastPosition;
    NavMeshAgent agent;

    void Awake()
    {
        if (!car)
            car = GetComponent<EnemyCarNavMesh>();

        agent = car.Agent;
        lastPosition = transform.position;
    }

    void Update()
    {
        AnimateWheelRoll();
        AnimateWheelSteer();
    }

    // ================= RODAR =================

    void AnimateWheelRoll()
    {
        Vector3 delta = transform.position - lastPosition;
        delta.y = 0f;

        float distance = delta.magnitude;
        float direction = Mathf.Sign(Vector3.Dot(delta, transform.forward));

        float roll = distance * direction * wheelRotationMultiplier;

        foreach (Transform wheel in frontWheelMeshes)
            wheel.Rotate(Vector3.right, roll, Space.Self);

        foreach (Transform wheel in rearWheelMeshes)
            wheel.Rotate(Vector3.right, roll, Space.Self);

        lastPosition = transform.position;
    }

    // ================= GIRO =================

    /*  void AnimateWheelSteer()
     {
         Vector3 steeringTarget = agent.steeringTarget - transform.position;
         steeringTarget.y = 0f;

         if (steeringTarget.sqrMagnitude < 0.01f)
             return;

         float angleToTarget = Vector3.SignedAngle(
             transform.forward,
             steeringTarget.normalized,
             Vector3.up
         );

         float steer = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
         float steerAngle = steer * maxWheelSteerAngle;

         foreach (Transform pivot in frontWheelPivots)
         {
             Vector3 euler = pivot.localEulerAngles;
             euler.y = steerAngle;
             pivot.localEulerAngles = euler;
         }
     } */

    void AnimateWheelSteer()
    {
        if (agent == null)
            return;

        Vector3 steeringTarget = agent.steeringTarget - transform.position;
        steeringTarget.y = 0f;

        if (steeringTarget.sqrMagnitude < 0.01f)
            return;

        float angleToTarget = Vector3.SignedAngle(
            transform.forward,
            steeringTarget.normalized,
            Vector3.up
        );

        float steer = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
        float steerAngle = steer * maxWheelSteerAngle;

        foreach (Transform pivot in frontWheelPivots)
        {
            if (!pivot) continue;
            Vector3 euler = pivot.localEulerAngles;
            euler.y = steerAngle;
            pivot.localEulerAngles = euler;
        }
    }
}
