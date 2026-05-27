Shader "Custom/InvertedHullOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width (base)", Range(0, 0.1)) = 0.005

        [Header(Vertex Color Line Control)]
        [Space(4)]
        // Canal G des vertex colors : 0 = outline invisible, 1 = pleine épaisseur
        // Peindre directement sur le mesh dans Blender / Maya / Unity pour
        // affiner les zones (doigts, cheveux fins) ou épaissir (contours principaux).
        _LineWidthMin  ("Line Width Min  (vert.col G=0)", Range(0, 0.1)) = 0.0
        _LineWidthMax  ("Line Width Max  (vert.col G=1)", Range(0, 0.1)) = 0.008
        [Toggle(_USE_VERTEX_COLOR)] _UseVertexColor ("Use Vertex Color G Channel", Float) = 1

        [Header(Camera Distance Scaling)]
        [Space(4)]
        // Comme GG, on peut garder une épaisseur constante en écran
        // en divisant par la distance caméra.
        [Toggle(_SCREEN_SPACE_WIDTH)] _ScreenSpaceWidth ("Constant Screen-Space Width", Float) = 0
        _ScreenWidthScale ("Screen Width Scale", Range(0, 5)) = 1.0

        [Header(Noise Deformation)]
        [Space(4)]
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.0
        _NoiseScale     ("Noise Scale",     Range(0.1, 50)) = 5.0
        _NoiseSpeed     ("Noise Speed",     Range(0, 10))   = 1.0

        [Header(Stretch Mask)]
        [Space(4)]
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
            #pragma shader_feature_local _USE_VERTEX_COLOR
            #pragma shader_feature_local _SCREEN_SPACE_WIDTH

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float  _OutlineWidth;

                // Vertex color line control (style Guilty Gear)
                float  _LineWidthMin;
                float  _LineWidthMax;

                // Camera distance scaling
                float  _ScreenWidthScale;

                // Noise
                float  _NoiseIntensity;
                float  _NoiseScale;
                float  _NoiseSpeed;

                // Stretch mask
                float  _StretchIntensity;
                float  _StretchWidth;
                float4 _StretchMask_ST;
            CBUFFER_END

            TEXTURE2D(_StretchMask);
            SAMPLER(sampler_StretchMask);

            // ── Noise helpers ────────────────────────────────────────────────
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
                float v = 0.0, a = 0.5;
                for (int i = 0; i < 3; i++)
                {
                    v += a * gradientNoise(p);
                    p  *= 2.0;
                    a  *= 0.5;
                }
                return v;
            }

            // ── Structures ───────────────────────────────────────────────────
            struct Attributes
            {
                float4 posOS   : POSITION;
                float3 normOS  : NORMAL;
                float2 uv      : TEXCOORD0;
                float4 color   : COLOR;   // vertex colors
                //   R = AO / autre usage
                //   G = LineThickness  (0 → LineWidthMin, 1 → LineWidthMax)
                //   B = libre
                //   A = libre
            };

            struct Varyings
            {
                float4 posHCS : SV_POSITION;
                float2 uv     : TEXCOORD0;
            };

            // ── Vertex ───────────────────────────────────────────────────────
            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 normWS = TransformObjectToWorldNormal(IN.normOS);
                float3 posWS  = TransformObjectToWorld(IN.posOS.xyz);

                // ── 1. Calcul de l'épaisseur finale ──────────────────────────
                float finalWidth = _OutlineWidth;

            #if _USE_VERTEX_COLOR
                // Canal G : 0 = min, 1 = max (comme Arc System Works)
                float vcG = IN.color.g;
                finalWidth = lerp(_LineWidthMin, _LineWidthMax, vcG);
            #endif

                // ── 2. Compensation distance caméra (écran constant) ──────────
            #if _SCREEN_SPACE_WIDTH
                // On calcule la distance world-space caméra → vertex
                float3 camPosWS  = GetCameraPositionWS();
                float  camDist   = length(posWS - camPosWS);
                finalWidth *= camDist * _ScreenWidthScale * 0.1;
            #endif

                // ── 3. Noise ──────────────────────────────────────────────────
                float  t   = _Time.y * _NoiseSpeed;
                float2 uvN = posWS.xz * _NoiseScale + t;
                float  n   = fbm(uvN);
                float3 noiseDisplace = normWS * n * _NoiseIntensity * finalWidth;

                // ── 4. Stretch mask ───────────────────────────────────────────
                OUT.uv = TRANSFORM_TEX(IN.uv, _StretchMask);
                float maskVal    = SAMPLE_TEXTURE2D_LOD(_StretchMask, sampler_StretchMask, OUT.uv, 0).r;
                float localStretch = _StretchWidth * _StretchIntensity * maskVal;

                // ── 5. Extrusion ──────────────────────────────────────────────
                posWS += normWS * (finalWidth + localStretch) + noiseDisplace;

                OUT.posHCS = TransformWorldToHClip(posWS);
                return OUT;
            }

            // ── Fragment ─────────────────────────────────────────────────────
            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }

    CustomEditor "UnityEditor.ShaderGUI"
}