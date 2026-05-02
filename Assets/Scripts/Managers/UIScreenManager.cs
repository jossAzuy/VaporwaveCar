using UnityEngine;

#region UI Types & Data

public enum UIScreenType
{
    Gameplay,
    PowerUpSelection,
    Pause,
    GameOver
}

[System.Serializable]
public class UIScreenGroup
{
    public UIScreenType screenType;

    [Header("Objects to Activate")]
    public GameObject[] activate;

    [Header("Objects to Deactivate")]
    public GameObject[] deactivate;
}

#endregion

public class UIScreenManager : MonoBehaviour
{
    [Header("UI Screen Groups")]
    [SerializeField] private UIScreenGroup[] screenGroups;

    /* ==============================
     * PUBLIC METHODS (UI SAFE)
     * ============================== */

    // ----- GAMEPLAY -----
    public void ShowGameplay()
    {
        ShowScreenInternal(UIScreenType.Gameplay);
    }

    // ----- POWER UP -----
    public void ShowPowerUpSelection()
    {
        ShowScreenInternal(UIScreenType.PowerUpSelection);
    }

    // ----- PAUSE -----
    public void ShowPause()
    {
        ShowScreenInternal(UIScreenType.Pause);
    }

    public void HidePause()
    {
        ShowScreenInternal(UIScreenType.Gameplay);
    }

    // ----- GAME OVER -----
    public void ShowGameOver()
    {
        ShowScreenInternal(UIScreenType.GameOver);
    }

    /* ==============================
     * INTERNAL LOGIC
     * ============================== */

    private void ShowScreenInternal(UIScreenType type)
    {
        UIScreenGroup group = GetGroup(type);
        if (group == null)
        {
            Debug.LogWarning($"UIScreenGroup not found for {type}");
            return;
        }

        // Activa lo correspondiente a este estado
        foreach (GameObject obj in group.activate)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Desactiva lo que no debe verse
        foreach (GameObject obj in group.deactivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private UIScreenGroup GetGroup(UIScreenType type)
    {
        foreach (UIScreenGroup group in screenGroups)
        {
            if (group.screenType == type)
                return group;
        }
        return null;
    }
}
