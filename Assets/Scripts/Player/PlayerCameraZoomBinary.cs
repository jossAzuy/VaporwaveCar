using UnityEngine;
using MoreMountains.Feedbacks;

public class PlayerCameraZoomBinary : MonoBehaviour
{
    public MMFeedbacks zoomInFeedback;
    public MMFeedbacks zoomOutFeedback;

    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public float deadZone = 0.1f;

    private bool _hadInputLastFrame = false;

    void Update()
    {
        bool hasInput =
            Mathf.Abs(Input.GetAxisRaw(horizontalAxis)) > deadZone ||
            Mathf.Abs(Input.GetAxisRaw(verticalAxis)) > deadZone;

        // CAMBIO: sin input → con input
        if (hasInput && !_hadInputLastFrame)
        {
            zoomOutFeedback?.PlayFeedbacks();
        }

        // CAMBIO: con input → sin input
        if (!hasInput && _hadInputLastFrame)
        {
            zoomInFeedback?.PlayFeedbacks();
        }

        _hadInputLastFrame = hasInput;
    }
}
