using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Botón para controlar la pausa del juego
/// </summary>
public class PauseButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("PauseButton: El GameObject debe tener un componente Button");
            return;
        }

        button.onClick.AddListener(OnPauseButtonClicked);
    }

    private void OnPauseButtonClicked()
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.TogglePause();
        }
        else
        {
            Debug.LogError("PauseManager no encontrado");
        }
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnPauseButtonClicked);
    }
}
