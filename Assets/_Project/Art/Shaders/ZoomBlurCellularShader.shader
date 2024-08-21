Shader "Custom/CombinedAuroraShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Animation Speed", Float) = 0.1
        _NoiseScale ("Noise Scale", Float) = 1.5
        _NoiseDistortion ("Noise Distortion", Float) = 0.5
        _PerspectiveIntensity ("Perspective Intensity", Float) = 1.3
        _AuroraIntensity ("Aurora Intensity", Float) = 1.0
        _AuroraColor1 ("Aurora Color 1", Color) = (0.1, 0.3, 0.9, 1.0)
        _AuroraColor2 ("Aurora Color 2", Color) = (0.1, 0.8, 0.4, 1.0)
    }

    SubShader
    {
        Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
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
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Speed;
                float _NoiseScale;
                float _NoiseDistortion;
                float _PerspectiveIntensity;
                float _AuroraIntensity;
                float4 _AuroraColor1;
                float4 _AuroraColor2;
            CBUFFER_END

            #define HASHSCALE3 float3(.1031, .1030, .0973)
            #define UVSCALE 100.

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            float4 permute_3d(float4 x) { return fmod(((x * 34.0) + 1.0) * x, 289.0); }
            float4 taylorInvSqrt3d(float4 r) { return 1.79284291400159 - 0.85373472095314 * r; }

            float simplexNoise3d(float3 v)
            {
                const float2 C = float2(1.0 / 6.0, 1.0 / 3.0);
                const float4 D = float4(0.0, 0.5, 1.0, 2.0);

                float3 i = floor(v + dot(v, C.yyy));
                float3 x0 = v - i + dot(i, C.xxx);

                float3 g = step(x0.yzx, x0.xyz);
                float3 l = 1.0 - g;
                float3 i1 = min(g.xyz, l.zxy);
                float3 i2 = max(g.xyz, l.zxy);

                float3 x1 = x0 - i1 + C.xxx;
                float3 x2 = x0 - i2 + C.yyy;
                float3 x3 = x0 - D.yyy;

                i = fmod(i, 289.0);
                float4 p = permute_3d(permute_3d(permute_3d(
                    i.z + float4(0.0, i1.z, i2.z, 1.0))
                    + i.y + float4(0.0, i1.y, i2.y, 1.0))
                    + i.x + float4(0.0, i1.x, i2.x, 1.0));

                float n_ = 0.142857142857;
                float3 ns = n_ * D.wyz - D.xzx;

                float4 j = p - 49.0 * floor(p * ns.z * ns.z);

                float4 x_ = floor(j * ns.z);
                float4 y_ = floor(j - 7.0 * x_);

                float4 x = x_ * ns.x + ns.yyyy;
                float4 y = y_ * ns.x + ns.yyyy;
                float4 h = 1.0 - abs(x) - abs(y);

                float4 b0 = float4(x.xy, y.xy);
                float4 b1 = float4(x.zw, y.zw);

                float4 s0 = floor(b0) * 2.0 + 1.0;
                float4 s1 = floor(b1) * 2.0 + 1.0;
                float4 sh = -step(h, float4(0.0, 0.0, 0.0, 0.0));

                float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
                float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;

                float3 p0 = float3(a0.xy, h.x);
                float3 p1 = float3(a0.zw, h.y);
                float3 p2 = float3(a1.xy, h.z);
                float3 p3 = float3(a1.zw, h.w);

                float4 norm = taylorInvSqrt3d(float4(dot(p0, p0), dot(p1, p1), dot(p2, p2), dot(p3, p3)));
                p0 *= norm.x;
                p1 *= norm.y;
                p2 *= norm.z;
                p3 *= norm.w;

                float4 m = max(0.6 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
                m = m * m;
                return 42.0 * dot(m * m, float4(dot(p0, x0), dot(p1, x1), dot(p2, x2), dot(p3, x3)));
            }

            float fbm3d(float3 x, const in int it)
            {
                float v = 0.0;
                float a = 0.5;
                float3 shift = float3(100, 100, 100);
                for (int i = 0; i < 32; ++i)
                {
                    if (i < it)
                    {
                        v += a * simplexNoise3d(x);
                        x = x * 2.0 + shift;
                        a *= 0.5;
                    }
                }
                return v;
            }

            float3 colorize(float t)
            {
                t = clamp(t, 0.0, 1.0);
                if (t < 0.5)
                    return lerp(float3(0.0, 0.0, 0.0), float3(1.0, 1.0, 1.0), t * 2.0);
                else
                    return lerp(float3(1.0, 1.0, 1.0), float3(0.0, 0.0, 0.0), (t - 0.5) * 2.0);
            }

            float2 applyPerspective(float2 uv, float intensity)
            {
                float factor = 1.0 / (1.0 + uv.y * intensity);
                uv.x = (uv.x - 0.5) * factor + 0.5;
                uv.y = (uv.y - 0.5) * factor + 0.5;
                return uv;
            }

            float4 zoomBlur(TEXTURE2D_PARAM(tex, samplerTex), float2 uv, float2 resolution, float2 center, float strength, int samples)
            {
                float4 color = float4(0.0, 0.0, 0.0, 0.0);
                float2 direction = uv - center;
                float total = 0.0;
                for (int i = 0; i < samples; i++)
                {
                    float t = (float)i / (float)(samples - 1);
                    float2 offset = direction * t * strength / resolution;
                    color += SAMPLE_TEXTURE2D(tex, samplerTex, uv + offset);
                    total += 1.0;
                }
                color /= total;
                return color;
            }

            float2 hash2d(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * HASHSCALE3);
                p3 += dot(p3, p3.yzx + 19.19);
                return frac((p3.xx + p3.yz) * p3.zy);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float2 uvOrigin = uv;
                float2 resolution = _ScreenParams.xy;
                float time = _Time.y * _Speed;
        
                // Improved Aurora effect
                uv = applyPerspective(float2(uv.x, uv.y + 0.6), _PerspectiveIntensity);
                float3 p = float3(uv.x * 2.0, uv.y, time * 0.1);
                
                float q = fbm3d(p, 10);
                float qx = fbm3d(p + float3(5.2, 1.3, time * 0.1), 10);
                float qz = fbm3d(p + float3(1.7, 9.2, time * 0.3), 10);
                
                float r = fbm3d(p + 4.0 * float3(qx, q, qz), 10);
                float g = fbm3d(p + 4.0 * float3(qz, qx, q), 10);
                float b = fbm3d(p + 4.0 * float3(q, qz, qx), 10);
                
                float3 auroraColor = lerp(_AuroraColor1.rgb, _AuroraColor2.rgb, float3(r, g, b));
                auroraColor = auroraColor * smoothstep(0.0, 1.5, uv.y) * _AuroraIntensity;
                
                // Star field effect
                float3 starColor = float3(0.0, 0.0, 0.0);
                for(int i = 0; i < 8; i++)
                {
                    float2 mouseCoords = float2(1.0, 1.0);
                    float screenRatio = _ScreenParams.x / _ScreenParams.y;
                    float2 ratioScale = float2(1.0 * screenRatio, 1);
                    float2 uvScale = float2((float)i * 1.0 + UVSCALE, (float)i * 1.0 + UVSCALE);
                    float2 starUV = (uvOrigin * ratioScale * _ScreenParams.xy) * uvScale;
                    float2 CellUVs = floor(starUV + float2(i * 1199, i * 1199));
                    float2 hash = (hash2d(CellUVs) * 2.0 - 1.0) * mouseCoords.x * 2.0;
                    float hash_magnitude = (1.0 - length(hash));
                    float2 UVgrid = frac(starUV) - 0.5;
                    float radius = clamp(hash_magnitude - 0.5, 0.0, 1.0);
                    float radialGradient = length(UVgrid - hash) / radius;
                    radialGradient = clamp(1.0 - radialGradient, 0.0, 1.0);
                    radialGradient *= radialGradient;
                    starColor += float3(radialGradient, radialGradient, radialGradient);
                }
                starColor = lerp(float3(0.227, 0.373, 0.714), starColor, uvOrigin.y / 3.0 + 0.4);
                starColor = lerp(float3(0.0, 0.0, 0.0), starColor, uvOrigin.y + 0.2);
        
                // Combine effects
                float3 finalColor = auroraColor + starColor;
        
                // Output to screen
                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}