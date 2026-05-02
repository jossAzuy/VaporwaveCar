using UnityEngine;
using System.Collections;

public class PoliceSirenLights : MonoBehaviour
{
    public Light lightA;
    public Light lightB;
    public float switchTime = 0.3f;

    void Start()
    {
        StartCoroutine(SirenEffect());
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
