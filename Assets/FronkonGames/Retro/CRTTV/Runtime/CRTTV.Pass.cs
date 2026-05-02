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
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace FronkonGames.Retro.CRTTV
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class CRTTV
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private CRTTVVolume volume;
      
      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");
        public static readonly int EffectTime = Shader.PropertyToID("_EffectTime");

        public static readonly int Frame = Shader.PropertyToID("_Frame");
        public static readonly int ShadowmaskStrength = Shader.PropertyToID("_ShadowmaskStrength");
        public static readonly int ShadowmaskLuminosity = Shader.PropertyToID("_ShadowmaskLuminosity");
        public static readonly int ShadowmaskScale = Shader.PropertyToID("_ShadowmaskScale");
        public static readonly int ShadowmaskColorOffset = Shader.PropertyToID("_ShadowmaskColorOffset");
        public static readonly int ShadowmaskVerticalGapHardness = Shader.PropertyToID("_ShadowmaskVerticalGapHardness");
        public static readonly int ShadowmaskHorizontalGapHardness = Shader.PropertyToID("_ShadowmaskHorizontalGapHardness");
        public static readonly int FishEyeStrength = Shader.PropertyToID("_FishEyeStrength");
        public static readonly int FishEyeZoom = Shader.PropertyToID("_FishEyeZoom");
        public static readonly int Distortion = Shader.PropertyToID("_Distortion");
        public static readonly int DistortionSpeed = Shader.PropertyToID("_DistortionSpeed");
        public static readonly int DistortionAmplitude = Shader.PropertyToID("_DistortionAmplitude");
        public static readonly int VignetteSmoothness = Shader.PropertyToID("_VignetteSmoothness");
        public static readonly int VignetteRounding = Shader.PropertyToID("_VignetteRounding");
        public static readonly int VignetteBorders = Shader.PropertyToID("_VignetteBorders");
        public static readonly int ShineStrength = Shader.PropertyToID("_ShineStrength");
        public static readonly int ShineColor = Shader.PropertyToID("_ShineColor");
        public static readonly int ShinePosition = Shader.PropertyToID("_ShinePosition");
        public static readonly int RGBOffsetStrength = Shader.PropertyToID("_RGBOffsetStrength");
        public static readonly int ColorBleedingStrength = Shader.PropertyToID("_ColorBleedingStrength");
        public static readonly int ColorBleedingDistance = Shader.PropertyToID("_ColorBleedingDistance");
        public static readonly int ColorCurves = Shader.PropertyToID("_ColorCurves");
        public static readonly int GammaColor = Shader.PropertyToID("_GammaColor");
        public static readonly int Scanlines = Shader.PropertyToID("_Scanlines");
        public static readonly int ScanlinesCount = Shader.PropertyToID("_ScanlinesCount");
        public static readonly int ScanlinesVelocity = Shader.PropertyToID("_ScanlinesVelocity");
        public static readonly int InterferenceStrength = Shader.PropertyToID("_InterferenceStrength");
        public static readonly int InterferencePeakStrength = Shader.PropertyToID("_InterferencePeakStrength");
        public static readonly int InterferencePeakPosition = Shader.PropertyToID("_InterferencePeakPosition");
        public static readonly int ShakeStrength = Shader.PropertyToID("_ShakeStrength");
        public static readonly int ShakeRate = Shader.PropertyToID("_ShakeRate");
        public static readonly int MovementStrength = Shader.PropertyToID("_MovementStrength");
        public static readonly int MovementRate = Shader.PropertyToID("_MovementRate");
        public static readonly int MovementSpeed = Shader.PropertyToID("_MovementSpeed");
        public static readonly int Grain = Shader.PropertyToID("_Grain");
        public static readonly int StaticNoise = Shader.PropertyToID("_StaticNoise");
        public static readonly int BarStrength = Shader.PropertyToID("_BarStrength");
        public static readonly int BarHeight = Shader.PropertyToID("_BarHeight");
        public static readonly int BarSpeed = Shader.PropertyToID("_BarSpeed");
        public static readonly int BarOverflow = Shader.PropertyToID("_BarOverflow");

        public static readonly int FlickerStrength = Shader.PropertyToID("_FlickerStrength");
        public static readonly int FlickerSpeed = Shader.PropertyToID("_FlickerSpeed");
        
        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");      
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass() : base()
      {
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
      }

      /// <summary> Destroy render pass. </summary>
      ~RenderPass() => material = null;

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, volume.intensity.value);

        float time = volume.useScaledTime.value == true ? Time.time : Time.unscaledTime;
        material.SetVector(ShaderIDs.EffectTime, new Vector4(time / 20.0f, time, time * 2.0f, time * 3.0f));

        material.SetInt(ShaderIDs.Frame, Time.frameCount);
        material.SetFloat(ShaderIDs.ShadowmaskStrength, volume.shadowmaskStrength.value);
        material.SetFloat(ShaderIDs.ShadowmaskLuminosity, volume.shadowmaskLuminosity.value * 300.0f);
        material.SetFloat(ShaderIDs.ShadowmaskScale, volume.shadowmaskScale.value);
        material.SetVector(ShaderIDs.ShadowmaskColorOffset, volume.shadowmaskColorOffset.value);
        material.SetFloat(ShaderIDs.ShadowmaskVerticalGapHardness, volume.shadowmaskVerticalGapHardness.value);
        material.SetFloat(ShaderIDs.ShadowmaskHorizontalGapHardness, volume.shadowmaskHorizontalGapHardness.value);
        material.SetFloat(ShaderIDs.FishEyeStrength, volume.fishEyeStrength.value);
        material.SetVector(ShaderIDs.FishEyeZoom, volume.fishEyeZoom.value);
        material.SetFloat(ShaderIDs.Distortion, volume.distortion.value * 0.01f);
        material.SetFloat(ShaderIDs.DistortionSpeed, volume.distortionSpeed.value);
        material.SetFloat(ShaderIDs.DistortionAmplitude, volume.distortionAmplitude.value * 10.0f);
        material.SetFloat(ShaderIDs.VignetteSmoothness, volume.vignetteSmoothness.value);
        material.SetFloat(ShaderIDs.VignetteRounding, volume.vignetteRounding.value);
        material.SetVector(ShaderIDs.VignetteBorders, volume.vignetteBorders.value);
        material.SetFloat(ShaderIDs.ShineStrength, volume.shineStrength.value);
        material.SetColor(ShaderIDs.ShineColor, volume.shineColor.value);
        material.SetVector(ShaderIDs.ShinePosition, volume.shinePosition.value);
        material.SetFloat(ShaderIDs.RGBOffsetStrength, volume.rgbOffsetStrength.value);
        material.SetFloat(ShaderIDs.ColorBleedingStrength, volume.colorBleedingStrength.value);
        material.SetFloat(ShaderIDs.ColorBleedingDistance, volume.colorBleedingDistance.value);
        material.SetColor(ShaderIDs.ColorCurves, volume.colorCurves.value);
        material.SetVector(ShaderIDs.GammaColor, volume.gammaColor.value);
        material.SetFloat(ShaderIDs.Scanlines, volume.scanlines.value);
        material.SetFloat(ShaderIDs.ScanlinesCount, volume.scanlinesCount.value);
        material.SetFloat(ShaderIDs.ScanlinesVelocity, volume.scanlinesVelocity.value);
        material.SetFloat(ShaderIDs.InterferenceStrength, volume.interferenceStrength.value);
        material.SetFloat(ShaderIDs.InterferencePeakStrength, volume.interferencePeakStrength.value);
        material.SetFloat(ShaderIDs.InterferencePeakPosition, volume.interferencePeakPosition.value);
        material.SetFloat(ShaderIDs.ShakeStrength, volume.shakeStrength.value);
        material.SetFloat(ShaderIDs.ShakeRate, volume.shakeRate.value);
        material.SetFloat(ShaderIDs.MovementStrength, volume.movementStrength.value);
        material.SetFloat(ShaderIDs.MovementRate, volume.movementRate.value);
        material.SetFloat(ShaderIDs.MovementSpeed, volume.movementSpeed.value);
        material.SetFloat(ShaderIDs.Grain, volume.grain.value);
        material.SetFloat(ShaderIDs.StaticNoise, volume.staticNoise.value);
        material.SetFloat(ShaderIDs.BarStrength, volume.barStrength.value);
        material.SetFloat(ShaderIDs.BarHeight, volume.barHeight.value);
        material.SetFloat(ShaderIDs.BarSpeed, volume.barSpeed.value);
        material.SetFloat(ShaderIDs.BarOverflow, volume.barOverflow.value);

        material.SetFloat(ShaderIDs.FlickerStrength, volume.flickerStrength.value);
        material.SetFloat(ShaderIDs.FlickerSpeed, volume.flickerSpeed.value);

        material.SetFloat(ShaderIDs.Brightness, volume.brightness.value);
        material.SetFloat(ShaderIDs.Contrast, volume.contrast.value);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / volume.gamma.value);
        material.SetFloat(ShaderIDs.Hue, volume.hue.value);
        material.SetFloat(ShaderIDs.Saturation, volume.saturation.value);
      }

      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        volume = VolumeManager.instance.stack.GetComponent<CRTTVVolume>();
        if (volume == null || volume.IsActive() == false || material == null)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && volume.affectSceneView.value == false || cameraData.postProcessEnabled == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureHandle destination = renderGraph.CreateTexture(source.GetDescriptor(renderGraph));

        UpdateMaterial();

        RenderGraphUtils.BlitMaterialParameters pass = new(source, destination, material, 0);
        renderGraph.AddBlitPass(pass, $"{Constants.Asset.AssemblyName}.Pass");

        resourceData.cameraColor = destination;
      }
    }
  }
}
