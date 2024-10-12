Shader "Unlit/ConcentrationLogic"
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float2 ToPolar (float2 xy, float2 center)
            {
                xy -= center;
                float r = length(xy);
                float theta = atan2(xy.y, xy.x);
                return float2(r, theta);
            }

            float noise(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
            // sample the texture
            fixed4 col = tex2D(_MainTex, i.uv);
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, col);

            float2 center = float2(0.5, 0.5);
            float2 polar = ToPolar(i.uv, center);

            // Apply noise based on polar coordinates
            float radialNoise = noise(float2(polar.x * 10.0, polar.y * 10.0));

            // Create radial gradient with noise
            float gradient = smoothstep(0.4, 0.5, radialNoise);

            // Combine the gradient with the original color
            fixed4 result = fixed4(gradient, gradient, gradient, 1.0);

            return result; //RGBAに対応
            }

            ENDCG
        }
    }
}
