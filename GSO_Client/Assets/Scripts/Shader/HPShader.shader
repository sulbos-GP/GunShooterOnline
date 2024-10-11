Shader "Custom/RedTintOnLowHP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HPFactor ("HP Factor", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩 활성화

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _HPFactor; // 0 (완전 투명) ~ 0.2 (완전 붉음)

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

                // HP에 따라 붉은색과 투명도 조정
                fixed4 redColor = fixed4(1, 0, 0, 1); // 붉은색 (R, G, B, A)
                col.rgb = lerp(redColor.rgb, col.rgb, _HPFactor); // HP가 1일 때 완전 붉음
                
                // 소스 이미지의 알파값을 그대로 사용
                col.a = col.a * (1.0 - _HPFactor); // HP가 0일 때 완전 투명, HP가 1일 때 완전 불투명

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
