// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VARP/DiamondGold"
{
	Properties
	{
		_CubeMap("CubeMap", CUBE) = "white" {}
		_MainTex("MainTex", 2D) = "white" {}
		_NormalMap("NormalMap", 2D) = "bump" {}
		_ReflectionColor("ReflectionColor", Color) = (1,0,0,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform samplerCUBE _CubeMap;
		uniform float4 _ReflectionColor;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 tex2DNode15 = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			o.Normal = tex2DNode15;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode2 = tex2D( _MainTex, uv_MainTex );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			o.Albedo = ( tex2DNode2 + ( texCUBE( _CubeMap, reflect( -ase_worldViewDir , tex2DNode15 ) ) * _ReflectionColor * ( 1.0 - tex2DNode2.r ) ) ).rgb;
			o.Metallic = tex2DNode2.r;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
-376;1082;1906;697;1453.053;233.9319;1.3;True;True
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;12;-996.6901,214.8104;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NegateNode;11;-784.2939,236.7823;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;15;-964.8535,449.8638;Float;True;Property;_NormalMap;NormalMap;2;0;Create;True;0;0;False;0;c3a5b320093562d4eae0791f43ee79ca;c3a5b320093562d4eae0791f43ee79ca;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ReflectOpNode;13;-632.9311,336.8771;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;2;-310.9152,-182.6768;Float;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;False;0;82e9f4d4e5a19ef4b91c68b7650b0bf6;aab31f177234f1a4aad2031488777ed9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;9;12.33948,230.9349;Float;False;Property;_ReflectionColor;ReflectionColor;3;0;Create;True;0;0;False;0;1,0,0,0;0.2352941,0.2470588,0.5058824,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;6;-7.249729,25.5211;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-403.272,280.2552;Float;True;Property;_CubeMap;CubeMap;0;0;Create;True;0;0;False;0;c155d079fdb0f0d4296504928918a667;eec7739d48e8bca4a8334a55b9f3ed4e;True;0;False;white;Auto;False;Object;-1;Auto;Cube;6;0;SAMPLER2D;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;200.728,25.75525;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;218.728,-183.2448;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-287.6149,54.44104;Float;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;True;0;1;0.603;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;10;-1165.718,417.8825;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;382,-168;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;VARP/DiamondGold;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;12;0
WireConnection;13;0;11;0
WireConnection;13;1;15;0
WireConnection;6;0;2;1
WireConnection;4;1;13;0
WireConnection;7;0;4;0
WireConnection;7;1;9;0
WireConnection;7;2;6;0
WireConnection;5;0;2;0
WireConnection;5;1;7;0
WireConnection;0;0;5;0
WireConnection;0;1;15;0
WireConnection;0;3;2;1
WireConnection;0;4;14;0
ASEEND*/
//CHKSM=2E6DBDFE5154BF7FC2EC27848760580230A3898C