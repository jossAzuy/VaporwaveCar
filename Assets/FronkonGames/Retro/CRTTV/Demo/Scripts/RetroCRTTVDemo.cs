using UnityEngine;
using FronkonGames.Retro.CRTTV;
using UnityEngine.Rendering;

/// <summary> Retro: CRT TV demo. </summary>
/// <remarks> This code is designed for a simple demo, not for production environments. </remarks>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public sealed class RetroCRTTVDemo : MonoBehaviour
{
  [Header("This code is only for demo, not for production environments.")]

  [Space(20.0f), SerializeField]
  private VolumeProfile volumeProfile;

  [Space]

  [SerializeField]
  private Transform floor;

  [SerializeField, Range(0.0f, 10.0f)]
  private float angularVelocity;

  private CRTTVVolume volume;

  private GUIStyle styleFont;
  private GUIStyle styleButton;
  private GUIStyle styleLogo;
  private Vector2 scrollView;

  private const float BoxWidth = 500.0f;
  private const float Margin = 20.0f;
  private const float LabelSize = 250.0f;
  private const float OriginalScreenWidth = 1920.0f;

  private int Slider(string label, int value, int left, int right)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Space(Margin);
      
      GUILayout.Label(label, styleFont, GUILayout.Width(LabelSize));
      
      value = (int)GUILayout.HorizontalSlider(value, left, right, GUILayout.ExpandWidth(true));
      
      GUILayout.Space(Margin);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private float Slider(string label, float value, float left, float right)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Space(Margin);
      
      GUILayout.Label(label, styleFont, GUILayout.Width(LabelSize));
      
      value = GUILayout.HorizontalSlider(value, left, right, GUILayout.ExpandWidth(true));
      
      GUILayout.Space(Margin);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private bool Toggle(string label, bool value)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Space(Margin);
      
      GUILayout.Label(label, styleFont, GUILayout.Width(LabelSize));
      
      value = GUILayout.Toggle(value, string.Empty);
      
      GUILayout.Space(Margin);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private void Awake()
  {
    if (CRTTV.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (UnityEditor.EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    styleFont = styleButton = styleLogo = null;

    volume = volumeProfile != null && volumeProfile.TryGet(out CRTTVVolume vol) ? vol : null;
    this.enabled = CRTTV.IsInRenderFeatures();
  }

  private void OnEnable() => volume?.Reset();

  private void Update()
  {
    if (floor != null && angularVelocity > 0.0f)
      floor.rotation = Quaternion.Euler(0.0f, floor.rotation.eulerAngles.y + Time.deltaTime * angularVelocity * 10.0f, 0.0f);
  }

  private void OnGUI()
  {
    Matrix4x4 guiMatrix = GUI.matrix;
    GUI.matrix = Matrix4x4.Scale(Vector3.one * (Screen.width / OriginalScreenWidth));

    styleFont ??= new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.UpperLeft,
      fontStyle = FontStyle.Bold,
      fontSize = 28
    };

    styleButton ??= new GUIStyle(GUI.skin.button)
    {
      fontStyle = FontStyle.Bold,
      fontSize = 28
    };

    styleLogo ??= new GUIStyle(GUI.skin.label)
    {
      alignment = TextAnchor.MiddleCenter,
      fontStyle = FontStyle.Bold,
      fontSize = 36
    };

    if (volume != null)
    {
      GUILayout.BeginVertical("box", GUILayout.Width(BoxWidth), GUILayout.ExpandHeight(true));
      {
        GUILayout.Space(Margin);
        
        GUILayout.BeginHorizontal();
        {
          GUILayout.FlexibleSpace();
          GUILayout.Label("Retro: CRT TV", styleLogo);
          GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(Margin * 0.5f);
        
        scrollView = GUILayout.BeginScrollView(scrollView);
        {
          volume.intensity.value = Slider("Intensity", volume.intensity.value, 0.0f, 1.0f);
          volume.shadowmaskStrength.value = Slider("Shadowmask", volume.shadowmaskStrength.value, 0.0f, 1.0f);
          volume.fishEyeStrength.value = Slider("Fisheye", volume.fishEyeStrength.value, 0.0f, 1.0f);
          volume.vignetteSmoothness.value = Slider("Vignette", volume.vignetteSmoothness.value, 0.0f, 2.0f);
          volume.shineStrength.value = Slider("Shine", volume.shineStrength.value, 0.0f, 1.0f);
          volume.rgbOffsetStrength.value = Slider("RGB offset", volume.rgbOffsetStrength.value, 0.0f, 1.0f);
          volume.colorBleedingStrength.value = Slider("Color bleeding", volume.colorBleedingStrength.value, 0.0f, 1.0f);
          volume.scanlines.value = Slider("Scanlines", volume.scanlines.value, 0.0f, 2.0f);
          volume.interferenceStrength.value = Slider("Interference", volume.interferenceStrength.value, 0.0f, 1.0f);
          volume.shakeRate.value = Slider("Shaking", volume.shakeRate.value, 0.0f, 1.0f);
          volume.movementRate.value = Slider("Movement", volume.movementRate.value, 0.0f, 1.0f);
          volume.grain.value = Slider("Grain", volume.grain.value, 0.0f, 1.0f);
          volume.staticNoise.value = Slider("Noise", volume.staticNoise.value, 0.0f, 1.0f);
          volume.barStrength.value = Slider("Bar", volume.barStrength.value, 0.0f, 1.0f);
          volume.flickerStrength.value = Slider("Flicker", volume.flickerStrength.value, 0.0f, 1.0f);
        }
        GUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("RESET", styleButton) == true)
          volume?.Reset();

        GUILayout.Space(4.0f);

        if (GUILayout.Button("ONLINE DOCUMENTATION", styleButton) == true)
          Application.OpenURL(Constants.Support.Documentation);

        GUILayout.Space(4.0f);

        if (GUILayout.Button("❤️ LEAVE A REVIEW ❤️", styleButton) == true)
          Application.OpenURL(Constants.Support.Store);

        GUILayout.Space(Margin * 2.0f);
      }
      GUILayout.EndVertical();
    }
    else
      GUILayout.Label($"URP not available or '{Constants.Asset.Name}' is not correctly configured, please consult the documentation", styleLogo);

    GUI.matrix = guiMatrix;
  }
}
