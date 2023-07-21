Shader "Custom/4 texture shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Scale("Scale", Range(0,1.5))=0.5
        _Blend("blending", Range(0,1)) = 0.5
        [NoScaleOffset] _Texture1("Texture 1", 2D) = "white" {}
        [NoScaleOffset] _Texture2("Texture 2", 2D) = "white" {}
        [NoScaleOffset] _Texture3("Texture 3", 2D) = "white" {}
        [NoScaleOffset] _Texture4("Texture 4", 2D) = "white" {}
        [NoScaleOffset] _Texture5("Texture 5", 2D) = "white" {}
        [NoScaleOffset] _Texture6("Texture 6", 2D) = "white" {}
       
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" } 
        LOD 200

        CGPROGRAM
       
        #pragma surface surf Standard fullforwardshadows vertex:vert

       
        #pragma target 3.0

        sampler2D _MainTex;
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
            //c += lerp(tex2D(_Texture2, IN.worldPos.xz * _Scale), tex2D(_Texture4, IN.worldPos.xz * _Scale), 0.5 ) * IN.terrain.y * IN.terrain2.x*4;
           /* c += 4 * IN.terrain.y * IN.terrain2.x *
                (tex2D(_Texture2, IN.worldPos.xz * _Scale) * IN.terrain.y * IN.color.r*2 +
                    tex2D(_Texture4, IN.worldPos.xz * _Scale) * IN.terrain2.x * IN.color.r*2 );*/
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
