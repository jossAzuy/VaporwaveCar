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
using UnityEditor;
using UnityEngine;

namespace FronkonGames.Retro.CRTTV.Editor
{
  /// <summary> CRT TV Volume inspector. </summary>
  [CustomEditor(typeof(CRTTVVolume))]
  public class CRTTVVolumeInspector : Inspector
  {
    public override void OnEnable()
    {
      if (serializedObject == null)
        return;
    }

    protected override void InspectorGUI()
    {
      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      DrawFloatSliderWithReset("intensity");

      /////////////////////////////////////////////////
      // Shadowmask.
      /////////////////////////////////////////////////
      if (Foldout("Shadow Mask") == true)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("shadowmaskStrength", "Strength");
        DrawFloatSliderWithReset("shadowmaskLuminosity", "Luminosity");
        DrawFloatSliderWithReset("shadowmaskScale", "Scale");
        DrawFloatSliderWithReset("shadowmaskVerticalGapHardness", "Vertical Gap Hardness");
        DrawFloatSliderWithReset("shadowmaskHorizontalGapHardness", "Horizontal Gap Hardness");
        DrawVector3SliderWithReset("shadowmaskColorOffset", "Color Offset", CRTTVVolume.DefaultShadowMaskColorOffset);
        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Fisheye & Distortion.
      /////////////////////////////////////////////////
      if (Foldout("Fisheye & Distortion") == true)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("fishEyeStrength", "Strength");
        DrawVector2SliderWithReset("fishEyeZoom", "Zoom", Vector2.one);
        DrawFloatSliderWithReset("distortion", "Distortion");
        DrawFloatSliderWithReset("distortionSpeed", "Speed");
        DrawFloatSliderWithReset("distortionAmplitude", "Amplitude");
        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Vignette.
      /////////////////////////////////////////////////
      if (Foldout("Vignette") == true)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("vignetteSmoothness", "Smoothness");
        DrawFloatSliderWithReset("vignetteRounding", "Rounding");
        DrawVector2SliderWithReset("vignetteBorders", "Borders", CRTTVVolume.DefaultVignetteBorders);
        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Screen Shine.
      /////////////////////////////////////////////////
      if (Foldout("Screen Shine") == true)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("shineStrength", "Strength");
        DrawColorWithReset("shineColor", "Color", CRTTVVolume.DefaultScreenShineColor);
        DrawVector2SliderWithReset("shinePosition", "Position", CRTTVVolume.DefaultScreenShinePosition);
        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // RGB Offset & Color Bleeding.
      /////////////////////////////////////////////////
      if (Foldout("RGB Offset & Color Bleeding") == true)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("rgbOffsetStrength", "RGB Offset");
        DrawFloatSliderWithReset("colorBleedingStrength", "Color Bleeding");
        IndentLevel++;
        DrawFloatSliderWithReset("colorBleedingDistance", "Distance");
        IndentLevel--;
        DrawColorWithReset("colorCurves", "Color Curves", CRTTVVolume.DefaultColorCurves);
        DrawColorWithReset("gammaColor", "Gamma Color", CRTTVVolume.DefaultGammaColor);
        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Scan lines.
      /////////////////////////////////////////////////
      if (Foldout("Scan Lines") == true)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("scanlines", "Count");
        DrawFloatSliderWithReset("scanlinesCount", "Count");
        DrawFloatSliderWithReset("scanlinesVelocity", "Velocity");
        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Glitches.
      /////////////////////////////////////////////////
      if (Foldout("Glitches") == true)
      {
        IndentLevel++;

        // Interference.
        DrawFloatSliderWithReset("interferenceStrength", "Interferences");
        IndentLevel++;
        DrawFloatSliderWithReset("interferencePeakStrength", "Peak Strength");
        DrawFloatSliderWithReset("interferencePeakPosition", "Peak Position");
        IndentLevel--;

        // Shake & Movement.
        DrawFloatSliderWithReset("shakeStrength", "Shake");
        IndentLevel++;
        DrawFloatSliderWithReset("shakeRate", "Rate");
        DrawFloatSliderWithReset("movementStrength", "Movement Strength");
        DrawFloatSliderWithReset("movementRate", "Movement Rate");
        DrawFloatSliderWithReset("movementSpeed", "Movement Speed");
        IndentLevel--;

        // Noise & Artifacts.
        DrawFloatSliderWithReset("grain", "Grain");

        DrawFloatSliderWithReset("staticNoise", "Static Noise");

        DrawFloatSliderWithReset("barStrength", "Bar");
        IndentLevel++;
        DrawFloatSliderWithReset("barHeight", "Height");
        DrawFloatSliderWithReset("barSpeed", "Speed");
        DrawFloatSliderWithReset("barOverflow", "Overflow");
        IndentLevel--;

        DrawFloatSliderWithReset("flickerStrength", "Flicker");
        IndentLevel++;
        DrawFloatSliderWithReset("flickerSpeed", "Speed");
        IndentLevel--;
        IndentLevel--;
      }
    }

    protected override void ResetValues() => ((CRTTVVolume)target).Reset();

    protected override void CheckForErrors()
    {
      if (CRTTV.IsInAnyRenderFeatures() == false)
      {
        Separator();

        EditorGUILayout.HelpBox($"Renderer Feature '{Constants.Asset.Name}' not found. You must add it as a Render Feature.", MessageType.Error);
      }
      else
      {
        CRTTV[] effects = CRTTV.Instances;

        bool anyEnabled = false;
        for (int i = 0; i < effects.Length; i++)
        {
          if (effects[i].isActive == true)
          {
            anyEnabled = true;
            break;
          }
        }

        if (anyEnabled == false)
        {
          Separator();

          EditorGUILayout.HelpBox($"No Renderer Feature '{Constants.Asset.Name}' is active. You must activate it in the Render Features.", MessageType.Warning);
        }
      }
    }
  }
}
