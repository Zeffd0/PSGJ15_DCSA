Shader "Unlit/CustomFragmentShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half3 palette(half t)
            {
                half3 a = half3(0.5, 0.5, 0.5);
                half3 b = half3(0.5, 0.5, 0.5);
                half3 c = half3(1.0, 1.0, 1.0);
                half3 d = half3(0.263, 0.416, 0.557);
                return a + b * cos(6.28318 * (c * t + d));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Modified UV calculation for 1:1 aspect ratio
                float2 uv = i.uv * 2.0 - 1.0;
                float2 uv0 = uv;
                half3 finalColor = half3(0.0, 0.0, 0.0);

                for (int j = 0; j < 4; j++)
                {
                    uv = frac(uv * 1.5) - 0.5;
                    half d = length(uv) * exp(-length(uv0));
                    half3 col = palette(length(uv0) + half(j) * 0.4 + _Time.y * 0.4);
                    d = sin(d * 8.0 + _Time.y) / 8.0;
                    d = abs(d);
                    d = pow(0.01 / d, 1.2);
                    finalColor += col * d;
                }

                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}