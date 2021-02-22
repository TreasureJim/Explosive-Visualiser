Shader "Custom/Texture3D"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 3D) = "" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        LOD 200

        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler3D _MainTex;

        struct vertInput
        {
            float4 pos: POSITION;
            float3 texcoord: TEXCOORD0;
        };

        struct vertOutput 
        {
            float4 pos: SV_POSITION;
            float3 texcoord: TEXCOORD0;
        };

        vertOutput vert(vertInput input)
        {
            vertOutput o;
            o.pos = UnityObjectToClipPos(input.pos);
            o.texcoord = input.texcoord;
            return o;
        }

        half4 frag(vertOutput o): SV_Target
        {
            half4 mainColor = tex3D(_MainTex, o.texcoord);
            return mainColor;
        }

        ENDCG
        }
    }
    FallBack "Diffuse"
}
