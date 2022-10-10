// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/transparent/Diamond"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_ReflectTex("Reflection Texture", Cube) = ""{}
		_RefractTex("Refraction Texture", Cube) = ""{}
	}
	SubShader
	{
		Tags
		{ "Queue" = "Transparent" }

		CGINCLUDE
#pragma vertex vert
#include "UnityCG.cginc"
		float4      _Color;
		samplerCUBE _ReflectTex;
		samplerCUBE _RefractTex;

		struct v2f
		{
			float4 pos    : SV_POSITION;
			float3 uv     : TEXCOORD0;
		};
        v2f vert (appdata_base v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            // calculate world space reflection vector 
            float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
            float3 worldN = normalize(UnityObjectToWorldNormal( v.normal ));
            o.uv = reflect( -viewDir, worldN );
            o.uv = mul(UNITY_MATRIX_MV, float4(o.uv, 0));
            return o;
		}
		half4 fragBack(v2f i) : SV_Target
		{
			float4 col = texCUBE(_RefractTex, i.uv) * _Color;
			return col;
		}
		half4 fragFront(v2f i) : SV_Target
		{
			float4 reflcol = texCUBE(_ReflectTex, i.uv);
			float4 col = texCUBE(_RefractTex, i.uv) * _Color + reflcol;
			return col;
		}
			ENDCG
		//---------------------------------------------------------------------------------------------
		// First pass - here we render the backfaces of the diamonds. Since those diamonds are more-or-less
		// convex objects, this is effectively rendering the inside of them
        // First pass - here we render the backfaces of the diamonds. Since those diamonds are more-or-less
        // convex objects, this is effectively rendering the inside of them
        //Pass
        //{
        //    Cull Front
        //    ZWrite Off
        //    CGPROGRAM
        //    #pragma fragment fragBack
        //    ENDCG
        //}
        // Second pass - here we render the front faces of the diamonds.      
        Pass
        {
            ZWrite On
        //    Blend One One
            CGPROGRAM
            #pragma fragment fragFront
            ENDCG
        }
	}
	Fallback "Legacy Shaders/VertexLit"
}