Shader "Tamir/2D/SpriteEmissionNoise"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        [Header(Emission)]
        [HDR] _EmissionColor    ("Emission Color",     Color)        = (1, 0.7, 0.25, 1)
              _EmissionIntensity("Emission Intensity", Range(0, 8))  = 2.0

        [Header(Scrolling Noise)]
              _NoiseScale       ("Noise Scale",        Range(0.5, 20)) = 4.0
              _NoiseSpeed       ("Noise Speed (xy)",   Vector)         = (0.30, 0.20, 0, 0)
              _NoiseContrast    ("Noise Contrast",     Range(0.5, 4))  = 1.5
              _NoiseFloor       ("Noise Floor",        Range(0, 1))    = 0.15
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
            Name "SpriteEmissionNoise"

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

            CBUFFER_START(UnityPerMaterial)
                float4 _EmissionColor;
                float  _EmissionIntensity;
                float  _NoiseScale;
                float4 _NoiseSpeed;
                float  _NoiseContrast;
                float  _NoiseFloor;
            CBUFFER_END

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float valueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p)
            {
                float n = valueNoise(p);
                n = lerp(n, valueNoise(p * 2.07 + 17.0), 0.5);
                return n;
            }

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
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;

                float2 nUV = IN.uv * _NoiseScale + _Time.y * _NoiseSpeed.xy;
                float  n   = fbm(nUV);
                n = pow(saturate(n), _NoiseContrast);
                n = max(n, _NoiseFloor);

                half3 emission = _EmissionColor.rgb * _EmissionIntensity * n * tex.a;
                half3 rgb      = tex.rgb + emission;

                return half4(rgb, tex.a);
            }
            ENDHLSL
        }
    }
}
