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

        Blend SrcAlpha OneMinusSrcAlpha // ���� ���� Ȱ��ȭ

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _HPFactor; // 0 (���� ����) ~ 0.2 (���� ����)

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

                // HP�� ���� �������� ���� ����
                fixed4 redColor = fixed4(1, 0, 0, 1); // ������ (R, G, B, A)
                col.rgb = lerp(redColor.rgb, col.rgb, _HPFactor); // HP�� 1�� �� ���� ����
                
                // �ҽ� �̹����� ���İ��� �״�� ���
                col.a = col.a * (1.0 - _HPFactor); // HP�� 0�� �� ���� ����, HP�� 1�� �� ���� ������

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
