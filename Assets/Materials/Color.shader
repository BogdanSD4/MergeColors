Shader "Custom/Color"
{
    Properties{
                _Color("Color", Color) = (1,1,1,1)
                _MainTex("Base (RGB)", 2D) = "white" {}
                _AddColor("Color", Color) = (1,1,1,1)
                _AddTex("Additional (ARGB)", 2D) = "white" {}
    }
        SubShader{
                Tags { "RenderType" = "Opaque" }
                LOD 200

                CGPROGRAM
                #pragma surface surf Standard fullforwardshadows

                #pragma target 3.0

                sampler2D _MainTex;
                sampler2D _AddTex;

                struct Input {
                        float2 uv_MainTex :   TEXCOORD0;
                        float2 uv_AddTex   :   TEXCOORD1;
                };

                fixed4 _Color;
                fixed4 _AddColor;

                void surf(Input IN, inout SurfaceOutputStandard o) {
                        half4 c = tex2D(_MainTex, IN.uv_MainTex.xy) * _Color;
                        half4 add = tex2D(_AddTex, IN.uv_AddTex.xy) * _AddColor;
                        o.Albedo = add.a == 0 ? c.rgb : add.rgb;
                        o.Alpha = c.a;
                }
                ENDCG
    }
        FallBack "Diffuse"
}
