Shader "Unlit/BlurryTransparentTiled"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0, 10)) = 5
        _Alpha ("Alpha", Range(0, 1)) = 0.5
        _TileCount ("Tile Count", Float) = 4
        _RandomSeed ("Random Seed", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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
            float _BlurAmount;
            float _Alpha;
            float _TileCount;
            float _RandomSeed;

            // Hash function for generating random numbers based on a seed
            float hash(float seed)
            {
                return frac(sin(seed) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texelSize = 1.0 / _ScreenParams.xy;
                float2 blurOffset = texelSize * _BlurAmount;

                // Calculate the tile size based on the tile count
                float2 tileSize = 1.0 / _TileCount;

                // Calculate the current tile index based on the UV coordinates
                float2 tileIndex = floor(i.uv * _TileCount);

                // Generate a random offset for each tile using the tile index and random seed
                float2 randomOffset = float2(hash(tileIndex.x + _RandomSeed), hash(tileIndex.y + _RandomSeed)) * tileSize;

                // Modify the UV coordinates to create the tiling effect with random offsets
                float2 tiledUV = frac(i.uv * _TileCount) + randomOffset;

                float4 color = float4(0, 0, 0, 0);
                color += tex2D(_MainTex, tiledUV + float2(-blurOffset.x, -blurOffset.y)) * 0.077847;
                color += tex2D(_MainTex, tiledUV + float2(0, -blurOffset.y)) * 0.123317;
                color += tex2D(_MainTex, tiledUV + float2(blurOffset.x, -blurOffset.y)) * 0.077847;
                color += tex2D(_MainTex, tiledUV + float2(-blurOffset.x, 0)) * 0.123317;
                color += tex2D(_MainTex, tiledUV) * 0.195346;
                color += tex2D(_MainTex, tiledUV + float2(blurOffset.x, 0)) * 0.123317;
                color += tex2D(_MainTex, tiledUV + float2(-blurOffset.x, blurOffset.y)) * 0.077847;
                color += tex2D(_MainTex, tiledUV + float2(0, blurOffset.y)) * 0.123317;
                color += tex2D(_MainTex, tiledUV + float2(blurOffset.x, blurOffset.y)) * 0.077847;

                color.a *= _Alpha;
                return color;
            }
            ENDCG
        }
    }
}