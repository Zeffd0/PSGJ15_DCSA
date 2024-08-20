Shader "Unlit/FractalPyramid"
{
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

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float3 palette(float d){
                return lerp(float3(0.2,0.7,0.9),float3(1.,0.,1.),d);
            }

            float2 rotate(float2 p, float a) {
                float c = cos(a);
                float s = sin(a);
                float2x2 m = float2x2(c, -s, s, c);
                return mul(p, m);
            }

            float map(float3 p){
                for( int i = 0; i<8; ++i){
                    float t = _Time.y*0.2;
                    p.xz = rotate(p.xz,t);
                    p.xy = rotate(p.xy,t*1.89);
                    p.xz = abs(p.xz);
                    p.xz-=.5;
                }
                return dot(sign(p),p)/5.;
            }

            float4 rm (float3 ro, float3 rd){
                float t = 0.;
                float3 col = 0.;
                float d;
                for(float i =0.; i<64.; i++){
                    float3 p = ro + rd*t;
                    d = map(p)*.5;
                    if(d<0.02){
                        break;
                    }
                    if(d>100.){
                        break;
                    }
                    col+=palette(length(p)*.1)/(400.*(d));
                    t+=d;
                }
                return float4(col,1./(d*100.));
            }

            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = (i.vertex.xy / _ScreenParams.xy) * 2.0 - 1.0;
                uv.x *= _ScreenParams.x / _ScreenParams.y;

                float3 ro = float3(0.,0.,-50.);
                ro.xz = rotate(ro.xz,_Time.y);
                float3 cf = normalize(-ro);
                float3 cs = normalize(cross(cf,float3(0.,1.,0.)));
                float3 cu = normalize(cross(cf,cs));
                float3 uuv = ro+cf*3. + uv.x*cs + uv.y*cu;
                float3 rd = normalize(uuv-ro);

                float4 col = rm(ro,rd);
                return col;
            }
            ENDCG
        }
    }
}