using UnityEngine;

public class CinematicCarWheelsOnly : MonoBehaviour
{
    [Header("Movimiento del suelo")]
    public float velocidadSuelo = 10f;

    [Header("Llanta")]
    public float radioLlanta = 0.35f;

    [Range(-1f, 1f)]
    public float simulatedSteer = 0f;

    [Header("Referencias")]
    public Transform[] frontWheelPivots;
    public Transform[] frontWheelMeshes;
    public Transform[] rearWheelMeshes;

    [Header("Ajustes visuales")]
    public float maxWheelSteerAngle = 30f;

    private float wheelRollAngle;

    void Update()
    {
        AnimateWheels();
    }

    void AnimateWheels()
    {
        // -------- ROTACIÓN FÍSICAMENTE CORRECTA --------
        float angularSpeedRad = velocidadSuelo / radioLlanta;
        float angularSpeedDeg = angularSpeedRad * Mathf.Rad2Deg;

        wheelRollAngle += angularSpeedDeg * Time.deltaTime;
        wheelRollAngle %= 360f;

        Quaternion rollRotation = Quaternion.Euler(wheelRollAngle, 0f, 0f);

        foreach (Transform wheel in frontWheelMeshes)
            wheel.localRotation = rollRotation;

        foreach (Transform wheel in rearWheelMeshes)
            wheel.localRotation = rollRotation;

        // -------- GIRO DE LLANTAS DELANTERAS --------
        float steerAngle = simulatedSteer * maxWheelSteerAngle;

        foreach (Transform pivot in frontWheelPivots)
        {
            Vector3 euler = pivot.localEulerAngles;
            euler.y = steerAngle;
            pivot.localEulerAngles = euler;
        }
    }
}
