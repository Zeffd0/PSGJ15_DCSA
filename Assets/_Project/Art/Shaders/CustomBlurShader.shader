Shader "Unlit/BlurShader"
{
    Properties
    {
        _BlurSize ("Blur Size", Float) = 1.0
        _BlurType ("Blur Type", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _BlurSize;
            int _BlurType;
            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 GaussianBlur(float2 uv, float2 texelSize)
            {
                float4 color = float4(0, 0, 0, 0);
                float weights[5] = { 0.227, 0.194, 0.122, 0.054, 0.016 };

                for (int i = -2; i <= 2; ++i)
                {
                    color += tex2D(_MainTex, uv + float2(i, 0) * texelSize.x) * weights[abs(i)];
                    color += tex2D(_MainTex, uv + float2(0, i) * texelSize.y) * weights[abs(i)];
                }
                return color;
            }

            fixed4 BoxBlur(float2 uv, float2 texelSize)
            {
                float4 color = float4(0, 0, 0, 0);
                for (int i = -1; i <= 1; ++i)
                {
                    for (int j = -1; j <= 1; ++j)
                    {
                        color += tex2D(_MainTex, uv + float2(i, j) * texelSize);
                    }
                }
                return color / 9.0;
            }

            fixed4 BokehBlur(float2 uv, float2 texelSize)
            {
                float4 color = float4(0, 0, 0, 0);
                float bokehWeights[5] = { 0.30, 0.20, 0.15, 0.10, 0.05 };

                for (int i = -2; i <= 2; ++i)
                {
                    for (int j = -2; j <= 2; ++j)
                    {
                        float weight = bokehWeights[abs(i)] * bokehWeights[abs(j)];
                        color += tex2D(_MainTex, uv + float2(i, j) * texelSize) * weight;
                    }
                }
                return color;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texelSize = _BlurSize / _ScreenParams.xy;

                if (_BlurType == 1)
                {
                    return BoxBlur(i.uv, texelSize);
                }
                else if (_BlurType == 2)
                {
                    return BokehBlur(i.uv, texelSize);
                }
                else
                {
                    return GaussianBlur(i.uv, texelSize);
                }
            }

            ENDCG
        }
    }
}
