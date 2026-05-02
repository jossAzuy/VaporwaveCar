using UnityEngine;

/// <summary>
/// Script de ejemplo para configurar rápidamente botones de PowerUp
/// Este script puede ser usado como referencia o como helper
/// </summary>
public class PowerUpSetupExample : MonoBehaviour
{
    [Header("EJEMPLO - Configuración de PowerUps")]
    [Tooltip("Este script es solo de referencia, muestra cómo configurar los powerups")]
    
    [Header("Lista de PowerUps Disponibles:")]
    [SerializeField] private bool _IncreasedFireRate_Disparo_Rapido;
    [SerializeField] private bool _ExtraLife_Vida_Extra;
    [SerializeField] private bool _RammingDamage_Embestida;
    [SerializeField] private bool _TemporalShield_Escudo_DobleClick;
    [SerializeField] private bool _Mines_Colocar_Minas;
    [SerializeField] private bool _HealthRecovery_Recuperar_Vida;

    [Header("Instrucciones:")]
    [TextArea(5, 10)]
    [SerializeField] private string instrucciones = 
        "CONFIGURACIÓN DE POWERUPS:\n\n" +
        "1. PANEL DE POWERUPS:\n" +
        "   - Crea 6 botones en tu panel de PowerUpSelection\n" +
        "   - Cada botón representa un powerup diferente\n\n" +
        
        "2. CONFIGURAR CADA BOTÓN:\n" +
        "   a) Añade el componente 'PowerUpButton'\n" +
        "   b) En PowerUpType, selecciona el tipo:\n" +
        "      • IncreasedFireRate (Disparo Rápido)\n" +
        "      • ExtraLife (Vida Extra)\n" +
        "      • RammingDamage (Embestida)\n" +
        "      • TemporalShield (Escudo - Doble Click)\n" +
        "      • Mines (Colocar Minas)\n" +
        "      • HealthRecovery (Recuperar Vida)\n" +
        "   c) Configura 'newCreditsRequired' (ej: 15)\n" +
        "   d) Opcional: asigna textos e iconos\n" +
        "   e) En el componente Button:\n" +
        "      OnClick() → PowerUpButton.SelectPowerUp()\n\n" +
        
        "3. POWERUPMANAGER:\n" +
        "   - Crea un GameObject vacío: 'PowerUpManager'\n" +
        "   - Añade el componente 'PowerUpManager'\n" +
        "   - Configura:\n" +
        "     • Fire Rate Multiplier: 0.7\n" +
        "     • Extra Lives: 1\n" +
        "     • Ramming Damage: 999\n" +
        "     • Shield Duration: 5\n" +
        "     • Mine Prefab: (tu prefab de mina)\n" +
        "     • Max Mines: 3\n" +
        "     • Referencias: AutoFireSystem, PlayerHealth\n\n" +
        
        "4. PREFAB DE MINA:\n" +
        "   - Crea GameObject 'Mine'\n" +
        "   - Añade modelo 3D\n" +
        "   - Añade script 'Mine'\n" +
        "   - Añade SphereCollider (IsTrigger = true)\n" +
        "   - Configura radios y daño\n" +
        "   - Crea Prefab\n\n" +
        
        "5. TESTING:\n" +
        "   - Completa una ronda\n" +
        "   - Debería aparecer el panel de powerups\n" +
        "   - Selecciona uno\n" +
        "   - Verifica en consola los logs";

    void Start()
    {
        Debug.Log("<color=yellow>═══════════════════════════════════════</color>");
        Debug.Log("<color=cyan>SISTEMA DE POWERUPS - FENIX</color>");
        Debug.Log("<color=yellow>═══════════════════════════════════════</color>");
        Debug.Log("\n<color=lime>PowerUps Disponibles:</color>");
        Debug.Log("1. <b>Disparo Rápido</b> (IncreasedFireRate) - Aumenta tasa de disparo");
        Debug.Log("2. <b>Vida Extra</b> (ExtraLife) - Añade corazón extra");
        Debug.Log("3. <b>Embestida</b> (RammingDamage) - Destruye enemigos al chocar");
        Debug.Log("4. <b>Escudo</b> (TemporalShield) - Escudo temporal con DOBLE CLICK");
        Debug.Log("5. <b>Minas</b> (Mines) - Coloca minas explosivas con CLICK");
        Debug.Log("6. <b>Recuperar Vida</b> (HealthRecovery) - Recupera un corazón");
        Debug.Log("\n<color=yellow>Revisa el README en: Assets/Scripts/PowerUp/README_PowerUps.md</color>");
        Debug.Log("<color=yellow>═══════════════════════════════════════</color>\n");
    }

    // Métodos de ejemplo para testeo rápido desde Inspector
    [ContextMenu("Test - Activar Disparo Rápido")]
    void TestIncreasedFireRate()
    {
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.SelectPowerUp(PowerUpType.IncreasedFireRate);
            PowerUpManager.Instance.ActivatePowerUpForRound();
        }
    }

    [ContextMenu("Test - Activar Vida Extra")]
    void TestExtraLife()
    {
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.SelectPowerUp(PowerUpType.ExtraLife);
            PowerUpManager.Instance.ActivatePowerUpForRound();
        }
    }

    [ContextMenu("Test - Activar Embestida")]
    void TestRammingDamage()
    {
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.SelectPowerUp(PowerUpType.RammingDamage);
            PowerUpManager.Instance.ActivatePowerUpForRound();
        }
    }

    [ContextMenu("Test - Activar Escudo (preparar)")]
    void TestTemporalShield()
    {
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.SelectPowerUp(PowerUpType.TemporalShield);
            PowerUpManager.Instance.ActivatePowerUpForRound();
            Debug.Log("Haz DOBLE CLICK para activar el escudo");
        }
    }

    [ContextMenu("Test - Activar Minas")]
    void TestMines()
    {
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.SelectPowerUp(PowerUpType.Mines);
            PowerUpManager.Instance.ActivatePowerUpForRound();
            Debug.Log("Haz CLICK para colocar una mina");
        }
    }

    [ContextMenu("Test - Recuperar Vida")]
    void TestHealthRecovery()
    {
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.SelectPowerUp(PowerUpType.HealthRecovery);
            PowerUpManager.Instance.ActivatePowerUpForRound();
        }
    }

    [ContextMenu("Reset PowerUps")]
    void ResetPowerUps()
    {
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.ResetPowerUps();
            Debug.Log("PowerUps reseteados");
        }
    }
}
