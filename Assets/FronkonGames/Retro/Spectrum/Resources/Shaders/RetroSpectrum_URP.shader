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
Shader "Hidden/Fronkon Games/Retro/Spectrum URP"
{
  Properties
  {
    _MainTex("Main Texture", 2D) = "white" {}
  }

  SubShader
  {
    Tags
    {
      "RenderType" = "Opaque"
      "RenderPipeline" = "UniversalPipeline"
    }

    Pass
    {
      Name "Fronkon Games Retro Spectrum"

      HLSLPROGRAM
      #include "Retro.hlsl"

      #pragma vertex RetroVert
			#pragma fragment frag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL
      #pragma multi_compile ___ SIMULATION_FULL

      float _PixelSize;
      float _Dither;
      float _BrightnessFull;
      float _BrightnessHalf;
      float _BrightnessGamma;
      float _AdjustGamma;

      inline float4 SMap(float3 pixel)
      {
        pixel = pow(abs(pixel), (float3)_AdjustGamma);

        return ((pixel.r > _BrightnessHalf) || (pixel.g > _BrightnessHalf) || (pixel.b > _BrightnessHalf))
          ? float4(pixel, 1.0)
          : float4(min((pixel / _BrightnessHalf), (float3)1.0), 0.0);
      }

      inline float4 BMap(float3 pixel)
      {
        pixel = pow(abs(pixel), (float3)_AdjustGamma);

        return ((pixel.r > _BrightnessHalf) || (pixel.g > _BrightnessHalf) || (pixel.b > _BrightnessHalf))
          ? float4(floor(pixel + (float3)0.5), 1.0)
          : float4(min(floor((pixel / _BrightnessHalf) + (float3)0.5), (float3)1.0), 0.0); 
      }

      inline float3 FMap(float4 pixel)
      {
        return pixel.a >= 0.5 ? pixel.rgb * (float3)_BrightnessFull : pixel.rgb * (float3)_BrightnessFull;
      }

      inline float3 OnlyColors(const float2 uv)
      {
        float2 pv = floor((_ScreenParams.xy * uv) / _PixelSize);
        const float2 sv = floor(_ScreenParams.xy / _PixelSize);

        float4 cs = SMap(SAMPLE_MAIN(pv / sv).rgb);

        return mod(pv.x + pv.y, 2.0) == 1.0
          ? float3(FMap(float4(floor(cs.rgb + (float3)(0.5 + (_Dither * 0.3))), cs.a)))
          : float3(FMap(float4(floor(cs.rgb + (float3)(0.5 - (_Dither * 0.3))), cs.a)));
      }

      inline float3 Full(const float2 uv)
      {
        float2 pv = floor((_ScreenParams.xy * uv) / _PixelSize);
        const float2 bv = floor(pv / _PixelSize) * _PixelSize;
        const float2 sv = floor(_ScreenParams.xy / _PixelSize);

        float4 min_cs = (float4)1.0;
        float4 max_cs = (float4)0.0;
        float bright = 0.0;

        for (int py = 1; py < 8; ++py)
        {
          for (int px = 0; px < 8; ++px)
          {
            float4 cs = BMap(SAMPLE_MAIN((bv + float2(px, py)) / sv).rgb);
            bright += cs.a;
            min_cs = min(min_cs, cs);
            max_cs = max(max_cs, cs);
          }
        }

        bright = bright >= 24.0 ? 1.0 : 0.0;

        UNITY_BRANCH
        if (all(max_cs.rgb == min_cs.rgb))
          min_cs.rgb = (float3)0.0;

        UNITY_BRANCH
        if (all(max_cs.rgb == (float3)0.0))
        {
          bright = 0.0;
          max_cs.rgb = float3(0.0, 0.0, 1.0);
          min_cs.rgb = (float3)0.0;
        }

        float3 c1 = FMap(float4(max_cs.rgb, bright));
        float3 c2 = FMap(float4(min_cs.rgb, bright));
        float3 cs = SAMPLE_MAIN(pv / sv).rgb;

        float3 d = (cs + cs) - (c1 + c2);
        const float dd = d.r + d.g + d.b;

        return mod(pv.x + pv.y, 2.0) != 1.0
          ? float3(dd >= (_Dither * 0.5)
                     ? c1.r
                     : c2.r, dd >= (_Dither * 0.5)
                               ? c1.g
                               : c2.g, dd >= (_Dither * 0.5)
                                         ? c1.b
                                         : c2.b)
          : float3(dd >= -(_Dither * 0.5)
                     ? c1.r : c2.r, dd >= -(_Dither * 0.5)
                                      ? c1.g
                                      : c2.g, dd >= -(_Dither * 0.5)
                                                ? c1.b
                                                : c2.b);
      } 

      inline float3 Blend(const float3 s, const float3 d)
      {
        return (s < 0.5) ? 2.0 * s * d : 1.0 - 2.0 * (1.0 - s) * (1.0 - d);
      }

      float4 frag(const RetroVaryings input) : SV_Target 
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord.xy);

        const float4 color = SAMPLE_MAIN(uv);
        float4 pixel = color;

#if SIMULATION_FULL
        pixel.rgb = Blend(color.rgb, Full(uv));
#else
        pixel.rgb = Blend(color.rgb, OnlyColors(uv));
#endif
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

        return lerp(color, pixel, _Intensity);
      }
      ENDHLSL
    }
  }
  
  FallBack "Diffuse"
}
