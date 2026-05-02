using UnityEngine;
using MoreMountains.Tools;

public class MMFaderUITrigger : MonoBehaviour
{
    [Header("Target Fader")]
    public int FaderID = 0;

    [Header("Fade Settings")]
    public float Duration = 0.3f;
    public bool IgnoreTimeScale = true;
    public MMTweenType Tween = null;

    public void FadeIn()
    {
        MMFadeInEvent.Trigger(Duration, Tween, FaderID, IgnoreTimeScale, Vector3.zero);
    }

    public void FadeOut()
    {
        MMFadeOutEvent.Trigger(Duration, Tween, FaderID, IgnoreTimeScale, Vector3.zero);
    }

    public void Toggle()
    {
        MMFadeEvent.Trigger(Duration, 1f, Tween, FaderID, IgnoreTimeScale, Vector3.zero);
    }
}
