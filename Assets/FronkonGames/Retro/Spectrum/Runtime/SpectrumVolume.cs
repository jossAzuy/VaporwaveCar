////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine.Rendering;

namespace FronkonGames.Retro.Spectrum
{
  /// <summary> Simulation modes. </summary>    
  public enum Simulation
  {
    /// <summary> Only colors and dither (faster but less realistic). </summary>      
    Simple,

    /// <summary> Full simulation. </summary>      
    Full
  }

  /// <summary> Spectrum Volume. </summary>
  [Serializable, VolumeComponentMenu("Fronkon Games/Retro/Spectrum")]
  public sealed class SpectrumVolume : VolumeComponent, IPostProcessComponent
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
    #region Spectrum settings.

    /// <summary> Simulation mode. Default OnlyColors. </summary>
    [EnumDropdown((int)Simulation.Simple, "Simulation mode. Default Simple.")]
    public EnumParameterNoInterpolation<Simulation> simulation = new(Simulation.Simple);

    /// <summary> Resolution [1 - 10]. Default 4. </summary>
    [IntSliderWithReset(4, 1, 10, "Resolution [1 - 10]. Default 4.")]
    public ClampedIntParameterNoInterpolation pixelSize = new(4, 1, 10);

    /// <summary> Dither amount [0.0 - 1.0]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Dither amount [0.0 - 1.0]. Default 1.")]
    public FloatSliderParameterNoInterpolation dither = new(1.0f, 0.0f, 1.0f);

    /// <summary> Color brightness full level [0.0 - 1.0]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Color brightness full level [0.0 - 1.0]. Default 1.")]
    public FloatSliderParameterNoInterpolation brightnessFull = new(1.0f, 0.0f, 1.0f);

    /// <summary> Color brightness half level [0.0 - 1.0]. Default 0.8. </summary>
    [FloatSliderWithReset(0.8f, 0.0f, 1.0f, "Color brightness half level [0.0 - 1.0]. Default 0.8.")]
    public FloatSliderParameterNoInterpolation brightnessHalf = new(0.8f, 0.0f, 1.0f);

    /// <summary> Adjust input gamma [0.0 - ...]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Adjust input gamma [0.0 - 2.0]. Default 1.")]
    public FloatSliderParameterNoInterpolation adjustGamma = new(1.0f, 0.0f, 2.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Color settings.

    /// <summary> Brightness [-1.0, 1.0]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, -1.0f, 1.0f, "Brightness [-1.0, 1.0]. Default 0.")]
    public FloatSliderParameterNoInterpolation brightness = new(0.0f, -1.0f, 1.0f);

    /// <summary> Contrast [0.0, 10.0]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 10.0f, "Contrast [0.0, 10.0]. Default 1.")]
    public FloatSliderParameterNoInterpolation contrast = new(1.0f, 0.0f, 10.0f);

    /// <summary>Gamma [0.1, 10.0]. Default 1. </summary>      
    [FloatSliderWithReset(1.0f, 0.1f, 10.0f, "Gamma [0.1, 10.0]. Default 1.")]
    public FloatSliderParameterNoInterpolation gamma = new(1.0f, 0.1f, 10.0f);

    /// <summary> The color wheel [0.0, 1.0]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "The color wheel [0.0, 1.0]. Default 0.")]
    public FloatSliderParameterNoInterpolation hue = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity of a colors [0.0, 2.0]. Default 1. </summary>      
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Intensity of a colors [0.0, 2.0]. Default 1.")]
    public FloatSliderParameterNoInterpolation saturation = new(1.0f, 0.0f, 2.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Advanced settings.

    /// <summary> Does it affect the Scene View? </summary>
    [ToggleWithReset(false, "Does it affect the Scene View?")]
    public BoolParameterNoInterpolation affectSceneView = new(false);

    /// <summary> Use scaled time. </summary>
    [ToggleWithReset(true, "Use scaled time.")]
    public BoolParameterNoInterpolation useScaledTime = new(true);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary> Reset to default values. </summary>
    public void Reset()
    {
      intensity.value = 1.0f;

      simulation.value = Simulation.Simple;
      pixelSize.value = 4;
      dither.value = 1.0f;
      brightnessFull.value = 1.0f;
      brightnessHalf.value = 0.8f;
      adjustGamma.value = 1.0f;

      brightness.value = 0.0f;
      contrast.value = 1.0f;
      gamma.value = 1.0f;
      hue.value = 0.0f;
      saturation.value = 1.0f;

      affectSceneView.value = false;
      useScaledTime.value = true;
    }

    /// <summary> Is the effect active? </summary>
    public bool IsActive() => intensity.overrideState && intensity.value > 0.0f;

    /// <summary> Is the effect tile compatible? </summary>
    public bool IsTileCompatible() => false;
  }
}
