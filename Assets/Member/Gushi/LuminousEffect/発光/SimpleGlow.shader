Shader "Custom/SimpleGlow"
{
    Properties
    {
        [HDR]_BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        [HDR]_GlowColor ("Glow Color", Color) = (0, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 1.0
        _PulseSpeed ("Pulse Speed", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

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

            float4 _BaseColor;
            float4 _GlowColor;
            float _GlowIntensity;
            float _PulseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                float dist = length(uv);
                
                // 円形グラデーション
                float circle = smoothstep(0.5, 0.2, dist);
                
                // ゆっくりしたパルス演出
                float pulse = sin(_Time.y * _PulseSpeed) * 0.2 + 0.8;
                
                float4 col = lerp(_BaseColor, _GlowColor, circle);
                col.rgb *= _GlowIntensity * pulse;
                col.a = circle * pulse;

                return col;
            }
            ENDCG
        }
    }
}
