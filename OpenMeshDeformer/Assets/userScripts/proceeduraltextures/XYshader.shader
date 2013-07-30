Shader "Custom/XYshader" {
	Properties {
		_MainTex ("Base (RGB)", 2D)= "white"{}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
			Pass
			{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			struct fragInput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 xpos : TEXCOORD1;
			};
			fragInput vert(vertexInput v)
			{
				fragInput o;
				o.pos = mul( UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.xpos = (v.vertex + float4(1.0, 1.0, 1.0f, 0.0))/2.0;
				return o;
			}
			float4 frag(fragInput o) : COLOR
			{
				//return float4( 1.0, 0.0, 0.0, 1.0);
				//return float4(o.xpos.x, o.xpos.y, o.xpos.z, 1.0);
				float4 x = tex2D(_MainTex, float2(o.xpos.x, o.xpos.y));
				return x;
			}
			ENDCG
			}
		}
		
	}