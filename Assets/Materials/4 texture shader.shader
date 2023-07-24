Shader "Custom/4 texture shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Grid ("grid map", 2D) = "white" {}
        _BumpMap("Bumpmap", 2D) = "bump" {}
        _Scale("Scale", Range(0,1.5))=0.5
        
        [NoScaleOffset] _Texture1("Texture 1", 2D) = "white" {}
        [NoScaleOffset] _Texture2("Texture 2", 2D) = "white" {}
        [NoScaleOffset] _Texture3("Texture 3", 2D) = "white" {}
        [NoScaleOffset] _Texture4("Texture 4", 2D) = "white" {}
        [NoScaleOffset] _Texture5("Texture 5", 2D) = "white" {}
        [NoScaleOffset] _Texture6("Texture 6", 2D) = "white" {}
       
        _Glossiness ("Smoothness", Range(0,1)) = 0.05
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" } 
        LOD 200

        CGPROGRAM
       
        #pragma surface surf Standard fullforwardshadows vertex:vert

       
        #pragma target 3.0

        sampler2D _Grid;
        sampler2D _BumpMap;
        sampler2D _Texture1, _Texture2, _Texture3, _Texture4, _Texture5, _Texture6;
        half _Glossiness;
        half _Metallic;
        float _Blend;
        float _Scale;

        struct Input {
            float4 color : COLOR;
            
            float3 worldPos;
            float3 terrain;
            float3 terrain2;
            float2 uv_BumpMap;
        };

        void vert(inout appdata_full v, out Input data) {
            UNITY_INITIALIZE_OUTPUT(Input, data);
            data.terrain = v.texcoord.xyz;
            data.terrain2 = v.texcoord1.xyz;
          
        }
  

       
   
        UNITY_INSTANCING_BUFFER_START(Props)
           
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            
            
            fixed4 c = tex2D(_Texture1, IN.worldPos.xz * _Scale) * IN.terrain.x +
                tex2D(_Texture2, IN.worldPos.xz * _Scale) * IN.terrain.y +
                tex2D(_Texture3, IN.worldPos.xz * _Scale) * IN.terrain.z +
                tex2D(_Texture4, IN.worldPos.xz * _Scale) * IN.terrain2.x  +
                tex2D(_Texture5, IN.worldPos.xz * _Scale) * IN.terrain2.y +
                tex2D(_Texture6, IN.worldPos.xz * _Scale) * IN.terrain2.z ;
            float2 gridUV = IN.worldPos.xz;
            gridUV.x *= 1 / (4 * 8.66025404);
            gridUV.y *= 1 / (2 * 15.0); 
            fixed4 grid = tex2D(_Grid, IN.worldPos.xz );
           
            o.Albedo = c.rgb*IN.color;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
