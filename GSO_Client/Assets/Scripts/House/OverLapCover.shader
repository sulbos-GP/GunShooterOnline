Shader "Custom/OverLapCover"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}  // �ؽ�ó ������Ƽ
        _Color ("Tint Color", Color) = (1,1,1,1)  // ���� ������Ƽ
        _Alpha ("Transparency", Range(0,1)) = 1.0 // ���� ������Ƽ
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" } // �����ϰ� ������
        Blend SrcAlpha OneMinusSrcAlpha   // ���� ���� ����
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;    // �ؽ�ó ���÷�
            float4 _Color;         // ����
            float _Alpha;          // ����

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
                // �ؽ�ó ���� �츮�� ������ ����� ���� ���� ����
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a *= _Alpha;  // ���� ��(����)�� ���ؼ� ����
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}