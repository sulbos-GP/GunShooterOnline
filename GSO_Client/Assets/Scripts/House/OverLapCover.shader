Shader "Custom/OverLapCover"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}  // 텍스처 프로퍼티
        _Color ("Tint Color", Color) = (1,1,1,1)  // 색상 프로퍼티
        _Alpha ("Transparency", Range(0,1)) = 1.0 // 투명도 프로퍼티
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" } // 투명하게 렌더링
        Blend SrcAlpha OneMinusSrcAlpha   // 알파 블렌딩 설정
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;    // 텍스처 샘플러
            float4 _Color;         // 색상
            float _Alpha;          // 투명도

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
                // 텍스처 색상에 우리가 설정한 색상과 알파 값을 곱함
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a *= _Alpha;  // 알파 값(투명도)을 곱해서 조정
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}