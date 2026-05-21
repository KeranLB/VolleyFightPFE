Shader "Custom/InvertedHullOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.005

        [Header(Noise Deformation)]
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.0
        _NoiseScale     ("Noise Scale",     Range(0.1, 50)) = 5.0
        _NoiseSpeed     ("Noise Speed",     Range(0, 10))   = 1.0

        [Header(Stretch Mask)]
        _StretchIntensity ("Stretch Intensity", Range(0, 1)) = 0.0
        _StretchWidth     ("Stretch Width Max", Range(0, 0.5)) = 0.05
        _StretchMask      ("Stretch Mask (R)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "Outline"
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float  _OutlineWidth;
                float  _NoiseIntensity;
                float  _NoiseScale;
                float  _NoiseSpeed;
                float  _StretchIntensity;
                float  _StretchWidth;
                float4 _StretchMask_ST;
            CBUFFER_END

            TEXTURE2D(_StretchMask);
            SAMPLER(sampler_StretchMask);

            float2 hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                           dot(p, float2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
            }

            float gradientNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(
                    lerp(dot(hash2(i + float2(0,0)), f - float2(0,0)),
                         dot(hash2(i + float2(1,0)), f - float2(1,0)), u.x),
                    lerp(dot(hash2(i + float2(0,1)), f - float2(0,1)),
                         dot(hash2(i + float2(1,1)), f - float2(1,1)), u.x),
                    u.y);
            }

            float fbm(float2 p)
            {
                float v = 0.0;
                float a = 0.5;
                for (int i = 0; i < 3; i++)
                {
                    v += a * gradientNoise(p);
                    p  *= 2.0;
                    a  *= 0.5;
                }
                return v;
            }

            struct Attributes {
                float4 posOS  : POSITION;
                float3 normOS : NORMAL;
                float2 uv     : TEXCOORD0;
            };

            struct Varyings {
                float4 posHCS : SV_POSITION;
                float2 uv     : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 normWS = TransformObjectToWorldNormal(IN.normOS);
                float3 posWS  = TransformObjectToWorld(IN.posOS.xyz);

                // UV pour le mask, avec Tiling/Offset
                OUT.uv = TRANSFORM_TEX(IN.uv, _StretchMask);

                // Noise
                float  t    = _Time.y * _NoiseSpeed;
                float2 uvN  = posWS.xz * _NoiseScale + t;
                float  n    = fbm(uvN);
                float3 noiseDisplace = normWS * n * _NoiseIntensity * _OutlineWidth;

                // On sample le mask dans le vertex shader
                // Le canal R pilote combien ce vertex peut s'étirer
                float maskVal = SAMPLE_TEXTURE2D_LOD(_StretchMask, sampler_StretchMask, OUT.uv, 0).r;

                // Stretch local : chaque vertex s'étire proportionnellement au mask
                float localStretch = _StretchWidth * _StretchIntensity * maskVal;

                posWS += normWS * (_OutlineWidth + localStretch) + noiseDisplace;

                OUT.posHCS = TransformWorldToHClip(posWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}