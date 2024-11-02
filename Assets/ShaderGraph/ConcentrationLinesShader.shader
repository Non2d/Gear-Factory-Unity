Shader "Custom/PolarNoiseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineColor ("Line Color", Color) = (0.5,0.5,0.5,0.5)
        _TimeSpeed ("Time Speed", Vector) = (0, 0, 0, 0)
        _NoiseScale ("Noise Scale", Float) = 1.0
        _UVOffset ("UV Offset", Vector) = (0, 0, 0, 0)
        _LineRegion ("Line Region", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _LineColor;
            float4 _TimeSpeed; // (time, speed, unused, unused)
            float _NoiseScale;
            float4 _UVOffset; // (u offset, v offset, unused, unused)
            float _LineRegion;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 GradientNoiseDir(float2 p)
            {
                p = fmod(p, 289);
                float x = fmod((34 * p.x + 1) * p.x, 289) + p.y;
                x = fmod((34 * x + 1) * x, 289);
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float GradientNoise(float2 p)
            {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(GradientNoiseDir(ip), fp);
                float d01 = dot(GradientNoiseDir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(GradientNoiseDir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(GradientNoiseDir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            float GradientNoiseScale(float2 UV, float Scale)
            {
                return GradientNoise(UV * Scale) + 0.5;
            }

            float2 ToPolar(float2 xy)
            {
                float r = length(xy);
                float rad = atan2(xy.x, xy.y);
                return float2(r, rad);
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = ToPolar(i.uv - _UVOffset.xy);
                float length = uv.x;
                float rad = (uv.y / (3.1415926535) + 1.0f) / 2.0f;

                float time = _TimeSpeed.x;
                float speed = _TimeSpeed.y;

                float noise = GradientNoiseScale(float2(rad, rad) + float2(time * speed, 0.0f), _NoiseScale);

                float y = 1.0f - length + noise;
                float region = smoothstep(_LineRegion, 1.0f, y);

                float3 color = tex2D(_MainTex, i.uv).rgb;
                color = lerp(_LineColor.rgb, color, region);
                return float4(color, 1.0f);
            }
            ENDCG
        }
    }
}
