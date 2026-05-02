using UnityEngine;
using System.Collections;

public class PoliceSirenController : MonoBehaviour
{
    [Header("Lights")]
    public Light lightA;
    public Light lightB;
    public float switchTime = 0.3f;

    [Header("Audio")]
    public AudioSource sirenAudio;

    private Coroutine sirenCoroutine;
    public bool IsPlaying { get; private set; }

    void Awake()
    {
        // ACTUALIZACIÓN:
        // Auto-asignación de referencias para evitar pérdidas en prefabs
        if (sirenAudio == null)
        {
            sirenAudio = GetComponent<AudioSource>();

            if (sirenAudio == null)
                sirenAudio = GetComponentInChildren<AudioSource>();
        }

        if (lightA == null || lightB == null)
        {
            Light[] lights = GetComponentsInChildren<Light>();
            if (lights.Length >= 2)
            {
                lightA = lights[0];
                lightB = lights[1];
            }
        }

        // Estado inicial seguro
        if (sirenAudio != null)
            sirenAudio.loop = true;

        lightA.enabled = false;
        lightB.enabled = false;
    }

    private void Start()
    {
        StartSiren();
    }

    public void StartSiren()
    {
        if (IsPlaying) return;

        IsPlaying = true;
        sirenAudio.Play();
        sirenCoroutine = StartCoroutine(SirenEffect());
    }

    public void StopSiren()
    {
        if (!IsPlaying) return;

        IsPlaying = false;

        if (sirenCoroutine != null)
            StopCoroutine(sirenCoroutine);

        sirenCoroutine = null;
        sirenAudio.Stop();

        lightA.enabled = false;
        lightB.enabled = false;
    }

    IEnumerator SirenEffect()
    {
        while (true)
        {
            lightA.enabled = true;
            lightB.enabled = false;
            yield return new WaitForSeconds(switchTime);

            lightA.enabled = false;
            lightB.enabled = true;
            yield return new WaitForSeconds(switchTime);
        }
    }
}
