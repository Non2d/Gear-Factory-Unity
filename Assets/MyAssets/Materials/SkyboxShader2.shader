Shader "Custom/PanoramicGaussianBlur" {
    Properties {
        _MainTex ("Panoramic Texture", 2D) = "" {}
        _Exposure ("Exposure", Float) = 1
        _BlurSize ("Blur Size", Float) = 1.0
    }
    SubShader {
        Tags { "Queue" = "Background" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Exposure;
            float _BlurSize;

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy * 0.5 + 0.5;
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;
                half4 col = half4(0, 0, 0, 0);

                // Gaussian kernel weights
                float kernel[5];
                kernel[0] = 0.227027;
                kernel[1] = 0.1945946;
                kernel[2] = 0.1216216;
                kernel[3] = 0.054054;
                kernel[4] = 0.016216;

                // Apply Gaussian blur
                for (int x = -4; x <= 4; ++x) {
                    for (int y = -4; y <= 4; ++y) {
                        float2 offset = float2(x, y) * _BlurSize / _ScreenParams.xy;
                        col += tex2D(_MainTex, uv + offset) * kernel[abs(x)] * kernel[abs(y)];
                    }
                }

                col *= _Exposure;
                return col;
            }
            ENDCG
        }
    }
}