Shader "Custom/S_ReticleTail"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _ColorStart ("Start Color", Color) = (1, 1, 0, 1)     
        _ColorMid ("Mid Color", Color) = (1, 0.5, 0, 1)        
        _ColorEnd ("End Color", Color) = (1, 0, 0, 0)           
        _Radius ("Disk Radius", Float) = 0.2
        _OrbitRadius ("Orbit Radius", Float) = 0.3
        _Speed ("Speed", Float) = 2.0
        _TrailLength ("Trail Length", Float) = 0.5
        _TrailSteps ("Trail Steps", Int) = 12
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Lighting Off

        Pass
        {
            Name "UI"
            Tags { "LightMode"="Always" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            float4 _ColorStart;
            float4 _ColorMid;
            float4 _ColorEnd;
            float _Radius;
            float _OrbitRadius;
            float _Speed;
            float _TrailLength;
            int _TrailSteps;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 finalColor = float4(0,0,0,0);

                for (int j = 0; j < _TrailSteps; j++) {
                    float age = j / (float)_TrailSteps;
                    float tOffset = _TrailLength * age;
                    float angle = (_Time - tOffset) * _Speed;

                    float2 center = float2(0.5, 0.5) + float2(cos(angle), sin(angle)) * _OrbitRadius;
                    float dist = distance(i.uv, center);

                    float alpha = pow(smoothstep(_Radius, _Radius * 0.8, dist), 2.0);
                    alpha *= 1.0 - age;

                    float4 gradientColor = lerp(_ColorStart, _ColorMid, age * 2);
                    if (age > 0.5) {
                        gradientColor = lerp(_ColorMid, _ColorEnd, (age - 0.5) * 2);
                    }

                    finalColor.rgb += gradientColor.rgb * alpha;
                    finalColor.a += alpha * gradientColor.a;
                }
                return saturate(finalColor) * i.color; 
            }
            ENDCG
        }
    }
}
