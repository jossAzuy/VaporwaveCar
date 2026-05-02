////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace FronkonGames.Retro.CRTTV
{
  /// <summary> CRT TV Volume. </summary>
  [Serializable, VolumeComponentMenu("Fronkon Games/Retro/CRT TV"), HelpURL(Constants.Support.Documentation)]
  public sealed class CRTTVVolume : VolumeComponent, IPostProcessComponent
  {
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Common settings.

    /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
    /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Controls the intensity of the effect [0, 1]. Default 1.")]
    public FloatSliderParameterLinear intensity = new(1.0f, 0.0f, 1.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Shadowmask.

    /// <summary> Shadowmask strength [0.0, 1.0]. Default 1.0. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Shadowmask strength [0.0, 1.0]. Default 1.0.")]
    public FloatSliderParameterNoInterpolation shadowmaskStrength = new(1.0f, 0.0f, 1.0f);

    /// <summary> Shadowmask luminosity [0.0, 2.0]. Default 1.0. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Shadowmask luminosity [0.0, 2.0]. Default 1.0.")]
    public FloatSliderParameterNoInterpolation shadowmaskLuminosity = new(1.0f, 0.0f, 2.0f);

    /// <summary> Shadowmask cells scale [0.0, 1.0]. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "Shadowmask cells scale [0.0, 1.0]. Default 0.5.")]
    public FloatSliderParameterNoInterpolation shadowmaskScale = new(0.5f, 0.0f, 1.0f);

    /// <summary> Shadowmask cells vertical gap hardness [0.0, 1.0]. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, 0.0f, 1.0f, "Shadowmask cells vertical gap hardness [0.0, 1.0]. Default 0.1.")]
    public FloatSliderParameterNoInterpolation shadowmaskVerticalGapHardness = new(0.1f, 0.0f, 1.0f);

    /// <summary> Shadowmask cells horizontal gap hardness [0.0, 1.0]. Default 0.8. </summary>
    [FloatSliderWithReset(0.8f, 0.0f, 1.0f, "Shadowmask cells horizontal gap hardness [0.0, 1.0]. Default 0.8.")]
    public FloatSliderParameterNoInterpolation shadowmaskHorizontalGapHardness = new(0.8f, 0.0f, 1.0f);

    /// <summary> Shadowmask cells color channels X-axis offset. Default (0.0, -0.3, -0.6). </summary>
    [Vector3SliderWithReset(new float[] { 0.0f, -0.3f, -0.6f }, -1.0f, 1.0f, "Shadowmask cells color channels X-axis offset. Default (0.0, -0.3, -0.6).")]
    public Vector3ParameterNoInterpolation shadowmaskColorOffset = new(DefaultShadowMaskColorOffset);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Fisheye & Distortion.

    /// <summary> Fisheye strength [-1.0, 5.0]. Default 0.3. </summary>
    [FloatSliderWithReset(0.2f, -1.0f, 5.0f, "Fisheye strength [-1.0, 5.0]. Default 0.3.")]
    public FloatSliderParameterNoInterpolation fishEyeStrength = new(0.2f, -1.0f, 5.0f);

    /// <summary> Fisheye zoom. Default (1, 1). </summary>
    public Vector2ParameterNoInterpolation fishEyeZoom = new(Vector2.one);

    /// <summary> Sinusoidal distortion of the vertical axis [0, 2.0]. Default 0.17. </summary>
    [FloatSliderWithReset(0.17f, 0.0f, 2.0f, "Sinusoidal distortion of the vertical axis [0, 2.0]. Default 0.17.")]
    public FloatSliderParameterNoInterpolation distortion = new(0.17f, 0.0f, 2.0f);

    /// <summary> Distortion speed [-10, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, -10.0f, 10.0f, "Distortion speed [-10, 10]. Default 1.")]
    public FloatSliderParameterNoInterpolation distortionSpeed = new(1.0f, -10.0f, 10.0f);

    /// <summary> Wave amplitude of the distortion [0, 10]. Default 2. </summary>
    [FloatSliderWithReset(2.0f, 0.0f, 10.0f, "Wave amplitude of the distortion [0, 10]. Default 2.")]
    public FloatSliderParameterNoInterpolation distortionAmplitude = new(2.0f, 0.0f, 10.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Vignette.

    /// <summary> Vignette smoothness [0.0, 2.0]. Default 0.7. </summary>
    [FloatSliderWithReset(0.7f, 0.0f, 2.0f, "Vignette smoothness [0.0, 2.0]. Default 0.7.")]
    public FloatSliderParameterNoInterpolation vignetteSmoothness = new(0.7f, 0.0f, 2.0f);

    /// <summary> Vignette rounding [0.0, 1.0]. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "Vignette rounding [0.0, 1.0]. Default 0.5.")]
    public FloatSliderParameterNoInterpolation vignetteRounding = new(0.5f, 0.0f, 1.0f);

    /// <summary> Screen borders. Default (0.0, 0.5). </summary>
    public Vector2ParameterNoInterpolation vignetteBorders = new(DefaultVignetteBorders);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Screen Shine.

    /// <summary> Screen shine strength [0.0, 1.0]. Default 0.3. </summary>
    [FloatSliderWithReset(0.3f, 0.0f, 1.0f, "Screen shine strength [0.0, 1.0]. Default 0.3.")]
    public FloatSliderParameterNoInterpolation shineStrength = new(0.3f, 0.0f, 1.0f);

    /// <summary> Screen shine color. Default (0.5, 0.5, 0.5, 0.5). </summary>
    [ColorWithReset(0x80808080, "Screen shine color. Default (0.5, 0.5, 0.5, 0.5).")]
    public ColorParameterNoInterpolation shineColor = new(DefaultScreenShineColor);

    /// <summary> Screen shine position. Default (0.5, 1.0). </summary>
    public Vector2ParameterNoInterpolation shinePosition = new(DefaultScreenShinePosition);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region RGB Offset & Color Bleeding.

    /// <summary> Color channels offset strength [0.0, 1.0]. Default 0.25. </summary>
    [FloatSliderWithReset(0.25f, 0.0f, 1.0f, "Color channels offset strength [0.0, 1.0]. Default 0.25.")]
    public FloatSliderParameterNoInterpolation rgbOffsetStrength = new(0.25f, 0.0f, 1.0f);

    /// <summary> Color bleeding strength [0.0, 1.0]. Default 0.25. </summary>
    [FloatSliderWithReset(0.25f, 0.0f, 1.0f, "Color bleeding strength [0.0, 1.0]. Default 0.25.")]
    public FloatSliderParameterNoInterpolation colorBleedingStrength = new(0.25f, 0.0f, 1.0f);

    /// <summary> Color bleeding distance [-1.0, 1.0]. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, -1.0f, 1.0f, "Color bleeding distance [-1.0, 1.0]. Default 0.5.")]
    public FloatSliderParameterNoInterpolation colorBleedingDistance = new(0.5f, -1.0f, 1.0f);

    /// <summary> Color curves adjustment. Default (0.95, 1.05, 0.95, 1.0). </summary>
    [ColorWithReset(0xF2FFF2FF, "Color curves adjustment. Default (0.95, 1.05, 0.95, 1.0).")]
    public ColorParameterNoInterpolation colorCurves = new(DefaultColorCurves);

    /// <summary> Gamma color tint. Default (0.9, 0.7, 1.2, 1.0). </summary>
    [ColorWithReset(0xE6B3F3FF, "Gamma color tint. Default (0.9, 0.7, 1.2, 1.0).")]
    public ColorParameterNoInterpolation gammaColor = new(DefaultGammaColor);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Scanlines.

    /// <summary> Scanlines strength [0.0, 1.0]. Default 1.0. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Scanlines strength [0.0, 1.0]. Default 1.0.")]
    public FloatSliderParameterNoInterpolation scanlines = new(1.0f, 0.0f, 1.0f);

    /// <summary> Scanlines density [0.0, 2.0]. Default 1.25. </summary>
    [FloatSliderWithReset(1.25f, 0.0f, 2.0f, "Scanlines density [0.0, 2.0]. Default 1.25.")]
    public FloatSliderParameterNoInterpolation scanlinesCount = new(1.25f, 0.0f, 2.0f);

    /// <summary> Scanlines velocity [-10.0, 10.0]. Default 3.5. </summary>
    [FloatSliderWithReset(3.5f, -10.0f, 10.0f, "Scanlines velocity [-10.0, 10.0]. Default 3.5.")]
    public FloatSliderParameterNoInterpolation scanlinesVelocity = new(3.5f, -10.0f, 10.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Interference.

    /// <summary> Signal interference strength [0.0, 1.0]. Default 0.2. </summary>
    [FloatSliderWithReset(0.2f, 0.0f, 1.0f, "Signal interference strength [0.0, 1.0]. Default 0.2.")]
    public FloatSliderParameterNoInterpolation interferenceStrength = new(0.2f, 0.0f, 1.0f);

    /// <summary> Peak interference strength [0.0, 1.0]. Default 0.3. </summary>
    [FloatSliderWithReset(0.3f, 0.0f, 1.0f, "Peak interference strength [0.0, 1.0]. Default 0.3.")]
    public FloatSliderParameterNoInterpolation interferencePeakStrength = new(0.3f, 0.0f, 1.0f);

    /// <summary> Peak interference y position [0.0, 1.0]. Default 0.2. </summary>
    [FloatSliderWithReset(0.2f, 0.0f, 1.0f, "Peak interference y position [0.0, 1.0]. Default 0.2.")]
    public FloatSliderParameterNoInterpolation interferencePeakPosition = new(0.2f, 0.0f, 1.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Shake & Movement.

    /// <summary> Vertical shake strength [0.0, 1.0]. Default 1.0. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Vertical shake strength [0.0, 1.0]. Default 1.0.")]
    public FloatSliderParameterNoInterpolation shakeStrength = new(1.0f, 0.0f, 1.0f);

    /// <summary> Vertical shake rate [0.0, 1.0]. Default 0.2. </summary>
    [FloatSliderWithReset(0.2f, 0.0f, 1.0f, "Vertical shake rate [0.0, 1.0]. Default 0.2.")]
    public FloatSliderParameterNoInterpolation shakeRate = new(0.2f, 0.0f, 1.0f);

    /// <summary> Vertical frame movement strength [0.0, 1.0]. Default 1.0. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Vertical frame movement strength [0.0, 1.0]. Default 1.0.")]
    public FloatSliderParameterNoInterpolation movementStrength = new(1.0f, 0.0f, 1.0f);

    /// <summary> Vertical frame movement rate [0.0, 1.0]. Default 0.2. </summary>
    [FloatSliderWithReset(0.2f, 0.0f, 1.0f, "Vertical frame movement rate [0.0, 1.0]. Default 0.2.")]
    public FloatSliderParameterNoInterpolation movementRate = new(0.2f, 0.0f, 1.0f);

    /// <summary> Vertical frame movement speed [0.0, 1.0]. Default 0.4. </summary>
    [FloatSliderWithReset(0.4f, 0.0f, 1.0f, "Vertical frame movement speed [0.0, 1.0]. Default 0.4.")]
    public FloatSliderParameterNoInterpolation movementSpeed = new(0.4f, 0.0f, 1.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Noise & Artifacts.

    /// <summary> Signal noise [0.0, 1.0]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Signal noise [0.0, 1.0]. Default 0.")]
    public FloatSliderParameterNoInterpolation grain = new(0.0f, 0.0f, 1.0f);

    /// <summary> Static noise [0.0, 1.0]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Static noise [0.0, 1.0]. Default 0.")]
    public FloatSliderParameterNoInterpolation staticNoise = new(0.0f, 0.0f, 1.0f);

    /// <summary> Bar effect strength [0.0, 1.0]. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, 0.0f, 1.0f, "Bar effect strength [0.0, 1.0]. Default 0.1.")]
    public FloatSliderParameterNoInterpolation barStrength = new(0.1f, 0.0f, 1.0f);

    /// <summary> Bar height [0.0, 10.0]. Default 6. </summary>
    [FloatSliderWithReset(6.0f, 0.0f, 10.0f, "Bar height [0.0, 10.0]. Default 6.")]
    public FloatSliderParameterNoInterpolation barHeight = new(6.0f, 0.0f, 10.0f);

    /// <summary> Bar speed [-10.0, 10.0]. Default 4. </summary>
    [FloatSliderWithReset(4.0f, -10.0f, 10.0f, "Bar speed [-10.0, 10.0]. Default 4.")]
    public FloatSliderParameterNoInterpolation barSpeed = new(4.0f, -10.0f, 10.0f);

    /// <summary> Bar overflow [0.0, 4.0]. Default 1.2. </summary>
    [FloatSliderWithReset(1.2f, 0.0f, 4.0f, "Bar overflow [0.0, 4.0]. Default 1.2.")]
    public FloatSliderParameterNoInterpolation barOverflow = new(1.2f, 0.0f, 4.0f);

    /// <summary> Flicker strength [0.0, 1.0]. Default 0.1. </summary>
    [FloatSliderWithReset(0.1f, 0.0f, 1.0f, "Flicker strength [0.0, 1.0]. Default 0.1.")]
    public FloatSliderParameterNoInterpolation flickerStrength = new(0.1f, 0.0f, 1.0f);

    /// <summary> Flicker speed [0.0, 100.0]. Default 50. </summary>
    [FloatSliderWithReset(50.0f, 0.0f, 100.0f, "Flicker speed [0.0, 100.0]. Default 50.")]
    public FloatSliderParameterNoInterpolation flickerSpeed = new(50.0f, 0.0f, 100.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Color settings.

    /// <summary> Brightness [-1, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, -1.0f, 1.0f, "Brightness [-1, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation brightness = new(0.0f, -1.0f, 1.0f);

    /// <summary> Contrast [0, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 10.0f, "Contrast [0, 10]. Default 1.")]
    public FloatSliderParameterNoInterpolation contrast = new(1.0f, 0.0f, 10.0f);

    /// <summary> Gamma [0.1, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.1f, 10.0f, "Gamma [0.1, 10]. Default 1.")]
    public FloatSliderParameterNoInterpolation gamma = new(1.0f, 0.1f, 10.0f);

    /// <summary> The color wheel [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "The color wheel [0, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation hue = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity of a colors [0, 2]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Intensity of a colors [0, 2]. Default 1.")]
    public FloatSliderParameterNoInterpolation saturation = new(1.0f, 0.0f, 2.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Advanced settings.

    /// <summary> Does it affect the Scene View? </summary>
    [ToggleWithReset(false, "Does it affect the Scene View?")]
    public BoolParameterNoInterpolation affectSceneView = new(false);

    /// <summary> Use scaled time. </summary>
    public BoolParameterNoInterpolation useScaledTime = new(true);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static Vector3 DefaultShadowMaskColorOffset = new(0.0f, -0.3f, -0.6f);
    public static Vector2 DefaultVignetteBorders = new(0.0f, 0.0f);
    public static Color DefaultScreenShineColor = new(0.5f, 0.5f, 0.5f, 0.5f);
    public static Vector2 DefaultScreenShinePosition = new(0.5f, 1.0f);
    public static Color DefaultColorCurves = new(0.95f, 1.05f, 0.95f, 1.0f);
    public static Color DefaultGammaColor = new(0.9f, 0.7f, 1.2f, 1.0f);

    /// <summary> Reset to default values. </summary>
    public void Reset()
    {
      intensity.value = 1.0f;

      shadowmaskStrength.value = 1.0f;
      shadowmaskLuminosity.value = 1.0f;
      shadowmaskScale.value = 0.5f;
      shadowmaskVerticalGapHardness.value = 0.1f;
      shadowmaskHorizontalGapHardness.value = 0.8f;
      shadowmaskColorOffset.value = DefaultShadowMaskColorOffset;

      fishEyeStrength.value = 0.2f;
      fishEyeZoom.value = Vector2.one;
      distortion.value = 0.17f;
      distortionSpeed.value = 1.0f;
      distortionAmplitude.value = 2.0f;

      vignetteSmoothness.value = 0.7f;
      vignetteRounding.value = 0.5f;
      vignetteBorders.value = DefaultVignetteBorders;

      shineStrength.value = 0.3f;
      shineColor.value = DefaultScreenShineColor;
      shinePosition.value = new Vector2(0.5f, 1.0f);

      rgbOffsetStrength.value = 0.25f;
      colorBleedingStrength.value = 0.25f;
      colorBleedingDistance.value = 0.5f;
      colorCurves.value = DefaultColorCurves;
      gammaColor.value = DefaultGammaColor;

      scanlines.value = 1.0f;
      scanlinesCount.value = 1.25f;
      scanlinesVelocity.value = 3.5f;

      interferenceStrength.value = 0.2f;
      interferencePeakStrength.value = 0.3f;
      interferencePeakPosition.value = 0.2f;

      shakeStrength.value = 1.0f;
      shakeRate.value = 0.2f;
      movementStrength.value = 1.0f;
      movementRate.value = 0.2f;
      movementSpeed.value = 0.4f;

      grain.value = 0.0f;
      staticNoise.value = 0.0f;
      barStrength.value = 0.1f;
      barHeight.value = 6.0f;
      barSpeed.value = 4.0f;
      barOverflow.value = 1.2f;
      flickerStrength.value = 0.1f;
      flickerSpeed.value = 50.0f;

      brightness.value = 0.0f;
      contrast.value = 1.0f;
      gamma.value = 1.0f;
      hue.value = 0.0f;
      saturation.value = 1.0f;

      affectSceneView.value = false;
      useScaledTime.value = true;
    }

    /// <summary> Is the effect active? </summary>
    public bool IsActive() => intensity.overrideState == true && intensity.value > 0.0f;

    /// <summary> Is the effect tile compatible? </summary>
    public bool IsTileCompatible() => false;
  }
}
