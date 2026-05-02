using UnityEngine;

/// <summary>
/// Script opcional para animar y gestionar el visual del escudo
/// Puede ser usado en el GameObject del escudo visual
/// </summary>
public class ShieldVisual : MonoBehaviour
{
    [Header("Animación")]
    [SerializeField] private bool rotateShield = true;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Header("Pulsación")]
    [SerializeField] private bool pulseEffect = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseMinScale = 0.9f;
    [SerializeField] private float pulseMaxScale = 1.1f;

    [Header("Material")]
    [SerializeField] private Material shieldMaterial;
    [SerializeField] private Color activeColor = new Color(0, 0.5f, 1f, 0.3f); // Azul translúcido
    [SerializeField] private string colorProperty = "_Color"; // O "_BaseColor" para URP

    private Vector3 initialScale;
    private float pulseTimer = 0f;

    private void Start()
    {
        initialScale = transform.localScale;

        // Configurar color del material si está asignado
        if (shieldMaterial != null)
        {
            shieldMaterial.SetColor(colorProperty, activeColor);
        }
        else
        {
            // Intentar obtener el material del renderer
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                shieldMaterial = renderer.material;
                shieldMaterial.SetColor(colorProperty, activeColor);
            }
        }
    }

    private void Update()
    {
        // Rotación
        if (rotateShield)
        {
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        }

        // Efecto de pulsación
        if (pulseEffect)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float scale = Mathf.Lerp(pulseMinScale, pulseMaxScale, 
                (Mathf.Sin(pulseTimer) + 1f) / 2f);
            transform.localScale = initialScale * scale;
        }
    }

    private void OnDisable()
    {
        // Resetear escala al desactivarse
        if (pulseEffect)
        {
            transform.localScale = initialScale;
        }
    }

    /// <summary>
    /// Cambia el color del escudo
    /// </summary>
    public void SetColor(Color color)
    {
        if (shieldMaterial != null)
        {
            shieldMaterial.SetColor(colorProperty, color);
        }
    }

    /// <summary>
    /// Cambia la transparencia del escudo
    /// </summary>
    public void SetAlpha(float alpha)
    {
        if (shieldMaterial != null)
        {
            Color currentColor = shieldMaterial.GetColor(colorProperty);
            currentColor.a = alpha;
            shieldMaterial.SetColor(colorProperty, currentColor);
        }
    }
}
