Shader "Custom/InvertedHullOutline"
{
    Properties
    {
        // Couleur de l'outline (noir par défaut)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        // Épaisseur de base, utilisée quand les vertex colors sont désactivées
        _OutlineWidth ("Outline Width (base)", Range(0, 0.1)) = 0.005

        [Header(Vertex Color Line Control)] //utilisation du channel vert des vertex pour géré l'intensitée sur les meshs pour plus de précision
        [Space(4)]
        // Épaisseur quand le vertex est peint en G = 0
        _LineWidthMin  ("Line Width Min  (vert.col G=0)", Range(0, 0.1)) = 0.0
        // Épaisseur quand le vertex est peint en G = 1
        _LineWidthMax  ("Line Width Max  (vert.col G=1)", Range(0, 0.1)) = 0.008
        [Toggle(_USE_VERTEX_COLOR)] _UseVertexColor ("Use Vertex Color G Channel", Float) = 1 // Recuperation du channel vert des vertex 

        [Header(Camera Distance Scaling)]
        [Space(4)]
        // Active la compensation de distance : l'outline garde la même taille à l'écran
        [Toggle(_SCREEN_SPACE_WIDTH)] _ScreenSpaceWidth ("Constant Screen-Space Width", Float) = 0
        // Multiplicateur de l'épaisseur en mode screen-space
        _ScreenWidthScale ("Screen Width Scale", Range(0, 5)) = 1.0

        [Header(Noise Deformation)]
        [Space(4)]
        // Force de la déformation (0 = outline propre, plus = tremblote / dessin à la main)
        _NoiseIntensity ("Noise Intensity", Range(0, 5)) = 0.0
        // Fréquence du bruit : petit = grosses ondulations, grand = détails fins
        _NoiseScale     ("Noise Scale",     Range(0.1, 50)) = 5.0
        // Vitesse d'animation du bruit
        _NoiseSpeed     ("Noise Speed",     Range(0, 10))   = 1.0

        [Header(Stretch Mask)]
        [Space(4)]
        // Force globale de l'épaississement localisé par la texture
        _StretchIntensity ("Stretch Intensity", Range(0, 1)) = 0.0
        // Épaisseur ajoutée max là où le masque est opaque (alpha = 1)
        _StretchWidth     ("Stretch Width Max", Range(0, 7)) = 0.05
        // Texture dont le canal alpha dit où l'outline doit être plus épais
        _StretchMask      ("Stretch Mask (A)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "Outline"
            // Inverted hull : on ne garde que les faces arrière du mesh gonflé
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
                float  _LineWidthMin;
                float  _LineWidthMax;
                float  _ScreenWidthScale;
                float  _NoiseIntensity;
                float  _NoiseScale;
                float  _NoiseSpeed;
                float  _StretchIntensity;
                float  _StretchWidth;
                float4 _StretchMask_ST; // tiling/offset 
            CBUFFER_END

            TEXTURE2D(_StretchMask);
            SAMPLER(sampler_StretchMask);

            // Vecteur 2D aléatoire 
            float2 hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                           dot(p, float2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
            }

            // Gradient noise 
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
                return 0.5 * gradientNoise(p);
            }

            struct Attributes
            {
                float4 posOS  : POSITION;
                float3 normOS : NORMAL;
                float2 uv     : TEXCOORD0;  // pour lire le stretch mask
                float4 color  : COLOR;      // vertex colors
            };

            struct Varyings
            {
                float4 posHCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 normWS = TransformObjectToWorldNormal(IN.normOS);
                float3 posWS  = TransformObjectToWorld(IN.posOS.xyz);

                // l'épaisseur finale
                float finalWidth = _OutlineWidth;

            #if _USE_VERTEX_COLOR
                // Canal G : 0 = min, 1 = max
                finalWidth = lerp(_LineWidthMin, _LineWidthMax, IN.color.g);
            #endif

            #if _SCREEN_SPACE_WIDTH
                // Plus on est loin, plus on épaissit : taille constante à l'écran
                finalWidth *= length(posWS - GetCameraPositionWS()) * _ScreenWidthScale * 0.1;
            #endif

                // Noise : position world (plan XZ) décalée dans le temps,   
                float n = fbm(posWS.xz * _NoiseScale + _Time.y * _NoiseSpeed);
                float3 noiseDisplace = normWS * n * _NoiseIntensity * finalWidth;

                // Stretch mask 
                float2 uv = TRANSFORM_TEX(IN.uv, _StretchMask);
                float maskVal = SAMPLE_TEXTURE2D_LOD(_StretchMask, sampler_StretchMask, uv, 0).a;
                float localStretch = _StretchWidth * _StretchIntensity * maskVal;

                // On pousse le vertex le long de sa normale
                posWS += normWS * (finalWidth + localStretch) + noiseDisplace;

                OUT.posHCS = TransformWorldToHClip(posWS);
                return OUT;
            }

            // Couleur unie, sans éclairage
            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
