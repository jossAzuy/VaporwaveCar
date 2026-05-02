using UnityEngine;

public class SueloInfinito : MonoBehaviour
{
    public float velocidad = 10f;
    public float limiteZ = -10f;

    [Header("Corrección")]
    public float ajuste = 0.01f; // offset corrector

    private float largoSuelo;

    void Start()
    {
        largoSuelo = ObtenerLargoSuelo();
    }

    void Update()
    {
        foreach (Transform suelo in transform)
        {
            suelo.Translate(Vector3.back * velocidad * Time.deltaTime, Space.World);

            if (suelo.position.z <= limiteZ)
            {
                ReposicionarSuelo(suelo);
            }
        }
    }

    float ObtenerLargoSuelo()
    {
        Renderer r = transform.GetChild(0).GetComponent<Renderer>();
        return r.bounds.size.z;
    }

    void ReposicionarSuelo(Transform suelo)
    {
        float maxZ = float.MinValue;

        foreach (Transform s in transform)
        {
            if (s.position.z > maxZ)
                maxZ = s.position.z;
        }

        suelo.position = new Vector3(
            suelo.position.x,
            suelo.position.y,
            maxZ + largoSuelo - ajuste
        );
    }
}
