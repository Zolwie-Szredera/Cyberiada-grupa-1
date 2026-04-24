Shader "Custom/SimpleInvert"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        // KLUCZ: Blend OneMinusDstColor OneMinusDstColor robi inwersję kolorów tła
        Blend OneMinusDstColor OneMinusDstColor
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                // Jeśli piksel jest przezroczysty, nie rysujemy nic (nie inwertujemy tła pod Alfą)
                if (col.a < 0.1) discard;
                return fixed4(1, 1, 1, 1);
            }
            ENDCG
        }
    }
}