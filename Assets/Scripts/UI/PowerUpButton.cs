using UnityEngine;
public class PowerUpButton : MonoBehaviour
{
    // public int newCreditsRequired = 15;

    public void SelectPowerUp()
    {
        Debug.Log("Power-Up seleccionado");

        // Notificamos al RoundManager, no al progreso directamente
        RoundManagerUpdated.Instance.OnPowerUpSelected();
    }
}
