Shader "Tamir/2D/VeinReveal"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture (auto from SpriteRenderer)", 2D) = "white" {}

        [Header(Line Mask)]
                          _LineMask     ("Line Mask (white = glow, black = none)", 2D) = "black" {}

        [Header(Reveal)]
        [Enum(Bottom To Top, 0, Top To Bottom, 1, Left To Right, 2, Right To Left, 3, Radial Out, 4, Radial In, 5, Brightness First, 6)]
                          _RevealDirection ("Reveal Direction",    Float)              = 0
                          _Reveal       ("Reveal (0..1)",          Range(0, 1))        = 0
                          _EdgeSoftness ("Edge Softness",          Range(0.001, 0.2))  = 0.03

        [Header(Glow)]
        [HDR]             _GlowColor    ("Glow Tint A",            Color)              = (1, 1, 1, 1)
        [HDR]             _GlowColor2   ("Glow Tint B",            Color)              = (1, 1, 1, 1)
                          _Intensity    ("Glow Intensity (HDR)",   Range(0, 10))       = 2.0

        [Header(Flowing Pulse)]
                          _FlowSpeed    ("Flow Speed",             Range(0, 5))        = 0.5
                          _FlowBands    ("Band Count",             Range(0.1, 20))     = 3
                          _FlowAmount   ("Pulse Amount (subtle)",  Range(0, 1))        = 0.3

        [Header(Domain Warp)]
                          _WarpStrength ("Warp Strength",          Range(0, 0.3))      = 0.04
                          _WarpSpeed    ("Warp Speed",             Range(0, 5))        = 0.5
                          _WarpFreq     ("Warp Frequency",         Range(0.5, 30))     = 6

        [Header(Normal Map)]
        [Normal]          _NormalMap     ("Normal Map",            2D)                 = "bump" {}
                          _NormalStrength("Normal Strength",       Range(0, 3))        = 1
                          _LightDir      ("Light Direction (XYZ)", Vector)             = (0.4, 0.6, 0.7, 0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"        = "Transparent"
            "Queue"             = "Transparent"
            "RenderPipeline"    = "UniversalPipeline"
            "PreviewType"       = "Plane"
            "CanUseSpriteAtlas" = "True"
            "IgnoreProjector"   = "True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "VeinReveal"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color       : COLOR;
                float2 uv          : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_LineMask);
            SAMPLER(sampler_LineMask);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _GlowColor;
                float4 _GlowColor2;
                float4 _LightDir;
                float  _RevealDirection;
                float  _Reveal;
                float  _EdgeSoftness;
                float  _Intensity;
                float  _FlowSpeed;
                float  _FlowBands;
                float  _FlowAmount;
                float  _WarpStrength;
                float  _WarpSpeed;
                float  _WarpFreq;
                float  _NormalStrength;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv    = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 sprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;

                // normal map relief on the base sprite. relief = NdotL - NdotL_flat, so a default/flat normal leaves sprite unchanged.
                half3 normalTS   = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv));
                half3 lightDir   = normalize(_LightDir.xyz);
                half  relief     = dot(normalTS, lightDir) - lightDir.z;
                sprite.rgb      *= saturate(1.0h + relief * _NormalStrength);

                // domain warp: offset the line-mask UVs with time-varying sines, two octaves, organic motion
                half t = _Time.y * _WarpSpeed;
                half2 warp;
                warp.x = sin(IN.uv.y * _WarpFreq          + t)         * _WarpStrength
                       + sin(IN.uv.y * _WarpFreq * 2.3h   + t * 0.7h)  * _WarpStrength * 0.5h;
                warp.y = cos(IN.uv.x * _WarpFreq          + t * 0.9h)  * _WarpStrength
                       + cos(IN.uv.x * _WarpFreq * 1.9h   + t * 0.5h)  * _WarpStrength * 0.5h;
                half2 warpedUV = IN.uv + warp;

                half4 mask = SAMPLE_TEXTURE2D(_LineMask, sampler_LineMask, warpedUV);

                // luminance: white = full glow, black = no glow. works on any B/W texture.
                half maskValue = dot(mask.rgb, half3(0.299h, 0.587h, 0.114h));

                // reveal direction selects how 'flow' is derived (0..1, fully revealed when flow <= _Reveal)
                half flow;
                if      (_RevealDirection < 0.5) flow = IN.uv.y;                                    // 0 Bottom -> Top
                else if (_RevealDirection < 1.5) flow = 1.0h - IN.uv.y;                             // 1 Top -> Bottom
                else if (_RevealDirection < 2.5) flow = IN.uv.x;                                    // 2 Left -> Right
                else if (_RevealDirection < 3.5) flow = 1.0h - IN.uv.x;                             // 3 Right -> Left
                else if (_RevealDirection < 4.5) flow = length(IN.uv - 0.5h) * 1.41421356h;         // 4 Center -> Edges
                else if (_RevealDirection < 5.5) flow = 1.0h - length(IN.uv - 0.5h) * 1.41421356h;  // 5 Edges -> Center
                else                              flow = 1.0h - maskValue;                           // 6 Brightness First

                half visible = 1.0h - smoothstep(_Reveal, _Reveal + _EdgeSoftness, flow);

                // bands of brighter light scrolling along the chosen direction, looping forever
                half phase = flow * _FlowBands - _Time.y * _FlowSpeed;
                half wave  = sin(6.28318530718h * phase) * 0.5h + 0.5h;
                half pulse = lerp(1.0h - _FlowAmount, 1.0h + _FlowAmount, wave);

                // consecutive bands alternate between Tint A and Tint B
                half  colorMix = cos(3.14159265359h * (phase - 0.25h)) * 0.5h + 0.5h;
                half3 tint     = lerp(_GlowColor.rgb, _GlowColor2.rgb, colorMix);

                // glow contribution: line pattern * reveal mask * pulse
                half  lineGlow         = maskValue * visible * pulse;
                half3 glowContribution = tint * _Intensity * lineGlow;

                // sprite color preserved. glow added on top, clamped to sprite shape (no bleeding outside).
                half3 finalRGB = sprite.rgb + glowContribution * sprite.a;
                return half4(finalRGB, sprite.a);
            }
            ENDHLSL
        }
    }
}
