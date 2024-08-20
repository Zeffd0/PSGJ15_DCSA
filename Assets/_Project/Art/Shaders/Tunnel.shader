Shader "Custom/Tunnel"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BumpTex ("Bump Texture", 2D) = "bump" {}
        _BumpHeight ("Bump Height", Float) = 5.0
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
            sampler2D _BumpTex;
            float _BumpHeight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = (2.0 * i.uv - 1.0) * _ScreenParams.y / _ScreenParams.x;
                float3 color = float3(0, 0, 0);
                float rInv = 1.0 / length(uv * 2.0);
                uv = uv * rInv;

                float time = _Time.y;

                float v1 = tex2D(_BumpTex, uv + float2(0.005, 0.0) - float2(rInv + time, 0.0)).r;
                float v2 = tex2D(_BumpTex, uv + float2(-0.005, 0.0) - float2(rInv + time, 0.0)).r;
                float v3 = tex2D(_BumpTex, uv + float2(0.0, 0.005) - float2(rInv + time, 0.0)).r;
                float v4 = tex2D(_BumpTex, uv + float2(0.0, -0.005) - float2(rInv + time, 0.0)).r;

                float bumpx = (v2 - v1) * _BumpHeight;
                float bumpy = (v3 - v4) * _BumpHeight;

                float light = 1.0 - length(float2(uv + rInv - 1.0) - float2(bumpx, bumpy));
                color = tex2D(_MainTex, uv - float2(rInv + time, 0.0)).rgb * light * 3.0;

                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}