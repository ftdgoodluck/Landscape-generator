Shader "Custom/4 texture shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _Texture1("Texture 1", 2D) = "white" {}
        [NoScaleOffset] _Texture2("Texture 2", 2D) = "white" {}
        [NoScaleOffset] _Texture3("Texture 3", 2D) = "white" {}
        [NoScaleOffset] _Texture4("Texture 4", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Texture1, _Texture2, _Texture3, _Texture4;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        struct Input {
            float4 color : COLOR;
            float3 worldPos;
            float2 uv:UV;
        };

        void vert(inout appdata_full v, out Input data) {
            UNITY_INITIALIZE_OUTPUT(Input, data);
            

        }
        
   
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
           
            
            fixed4 c = tex2D(_Texture1, IN.worldPos.xz * 0.5) * IN.color.r +
                tex2D(_Texture2, IN.worldPos.xz * 0.5) * IN.color.g +
                tex2D(_Texture3, IN.worldPos.xz * 0.5) * IN.color.b +
                tex2D(_Texture4, IN.worldPos.xz * 0.5) * (1 - IN.color.r - IN.color.g - IN.color.b);


            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
