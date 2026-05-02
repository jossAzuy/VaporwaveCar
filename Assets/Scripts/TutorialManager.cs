using UnityEngine;
using System.Collections;

/// <summary>
/// Gestor del tutorial que muestra un panel al inicio y lo desactiva después de un tiempo
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [Header("Panel de Tutorial")]
    [SerializeField] private GameObject tutorialPanel;

    [Header("Duración")]
    [SerializeField] private float displayDuration = 5f;

    [Header("Animación")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        if (tutorialPanel == null)
        {
            Debug.LogWarning("TutorialManager: No se asignó tutorialPanel");
            return;
        }

        // Obtener o crear CanvasGroup para animaciones
        canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();
        }

        StartCoroutine(ShowTutorialCoroutine());
    }

    private IEnumerator ShowTutorialCoroutine()
    {
        // Mostrar panel
        tutorialPanel.SetActive(true);

        if (useAnimation)
        {
            yield return StartCoroutine(FadeIn());
        }
        else
        {
            canvasGroup.alpha = 1f;
        }

        // Esperar duración especificada
        yield return new WaitForSeconds(displayDuration);

        // Desaparecer
        if (useAnimation)
        {
            yield return StartCoroutine(FadeOut());
        }
        else
        {
            canvasGroup.alpha = 0f;
        }

        // Desactivar panel
        tutorialPanel.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Método para ocultar el tutorial manualmente si es necesario
    /// </summary>
    public void HideTutorial()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// Método para mostrar el tutorial nuevamente si es necesario
    /// </summary>
    public void ShowTutorial()
    {
        if (!tutorialPanel.activeInHierarchy)
        {
            StartCoroutine(ShowTutorialCoroutine());
        }
    }
}
