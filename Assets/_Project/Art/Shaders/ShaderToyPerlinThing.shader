Shader "Custom/URPNoiseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags {"RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" "Queue"="Transparent"}
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Name "Unlit"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
            CBUFFER_END

            float hash3(float3 p)
            {
                return frac(sin(1e3 * dot(p, float3(1, 57, -13.7))) * 4375.5453);
            }

            float noise3(float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);

                float n = p.x + p.y * 57.0 + 113.0 * p.z;
                return lerp(
                    lerp(lerp(hash3(p + float3(0, 0, 0)), hash3(p + float3(1, 0, 0)), f.x),
                         lerp(hash3(p + float3(0, 1, 0)), hash3(p + float3(1, 1, 0)), f.x), f.y),
                    lerp(lerp(hash3(p + float3(0, 0, 1)), hash3(p + float3(1, 0, 1)), f.x),
                         lerp(hash3(p + float3(0, 1, 1)), hash3(p + float3(1, 1, 1)), f.x), f.y),
                    f.z);
            }

            float noise(float3 x)
            {
                return (noise3(x) + noise3(x + 11.5)) / 2.0;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float time = _Time.y;

                float2 R = float2(1, 1); // You might want to expose this as a property
                float n = noise(float3(uv * 8.0 / R.y, 0.1 * time));
                float v = sin(6.28 * 10.0 * n);

                float2 dx = ddx(uv * 8.0 / R.y);
                float2 dy = ddy(uv * 8.0 / R.y);
                float fwidth = length(float2(length(dx), length(dy))) * 10.0;
                v = smoothstep(1.0, 0.0, 0.5 * abs(v) / fwidth);

                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, (uv + float2(1, sin(time))) / R);
                half4 noiseColor = 0.5 + 0.5 * sin(12.0 * n + float4(0, 2.1, -2.1, 0));

                return lerp(exp(-33.0 / R.y) * texColor, noiseColor, v);
            }
            ENDHLSL
        }
    }
}