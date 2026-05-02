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
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace FronkonGames.Retro.Spectrum
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Spectrum
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private SpectrumVolume volume;

      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");
        public static readonly int EffectTime = Shader.PropertyToID("_EffectTime");

        public static readonly int PixelSize = Shader.PropertyToID("_PixelSize");
        public static readonly int Dither = Shader.PropertyToID("_Dither");
        public static readonly int BrightnessFull = Shader.PropertyToID("_BrightnessFull");
        public static readonly int BrightnessHalf = Shader.PropertyToID("_BrightnessHalf");
        public static readonly int AdjustGamma = Shader.PropertyToID("_AdjustGamma");
      
        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");      
      }
    
      private static class ShaderKeywords
      {
        public const string SimulationFull = "SIMULATION_FULL";
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass() : base()
      {
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
      }

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass() => material = null;

      private void UpdateMaterial()
      {
        if (material != null && volume != null)
        {
          material.shaderKeywords = null;
          material.SetFloat(ShaderIDs.Intensity, volume.intensity.value);

          float time = volume.useScaledTime.value == true ? Time.time : Time.unscaledTime;
          material.SetVector(ShaderIDs.EffectTime, new Vector4(time / 20.0f, time, time * 2.0f, time * 3.0f));

          if (volume.simulation.value == Simulation.Full)
            material.EnableKeyword(ShaderKeywords.SimulationFull);

          material.SetFloat(ShaderIDs.PixelSize, volume.pixelSize.value);
          material.SetFloat(ShaderIDs.Dither, volume.dither.value);
          material.SetFloat(ShaderIDs.BrightnessFull, volume.brightnessFull.value);
          material.SetFloat(ShaderIDs.BrightnessHalf, volume.brightnessHalf.value);
          material.SetFloat(ShaderIDs.AdjustGamma, volume.adjustGamma.value);

          material.SetFloat(ShaderIDs.Brightness, volume.brightness.value);
          material.SetFloat(ShaderIDs.Contrast, volume.contrast.value);
          material.SetFloat(ShaderIDs.Gamma, 1.0f / volume.gamma.value);
          material.SetFloat(ShaderIDs.Hue, volume.hue.value);
          material.SetFloat(ShaderIDs.Saturation, volume.saturation.value);
        }
      }

      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        volume = VolumeManager.instance.stack.GetComponent<SpectrumVolume>();
        if (material == null || volume == null || volume.IsActive() == false)
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
