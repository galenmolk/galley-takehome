Shader "Custom/S_BW_Reveal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RevealPercentage ("Reveal Percentage", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha

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
            float _RevealPercentage;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                if (i.uv.y < _RevealPercentage) {
                    return col;
                } else {
                    float averageColor = (col.r + col.g + col.b) / 3.0;
                    return fixed4(averageColor, averageColor, averageColor, col.a);
                }
            }
            ENDCG
        }
    }
}
