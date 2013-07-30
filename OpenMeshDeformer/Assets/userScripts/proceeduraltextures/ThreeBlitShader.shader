Shader "Custom/ThreeBlitShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D)= "white"{}
		_SecondaryTex ("Base (RGB)", 2D)= "white"{}
		_TertiaryTex ("Base (RGB)", 2D)= "white"{}
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
			Pass
			{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			
			#include "UnityCG.cginc"
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform sampler2D _SecondaryTex;
			uniform float4 _SecondaryTex_ST;
			uniform sampler2D _TertiaryTex;
			uniform float4 _TertiaryTex_ST;

			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};
			struct fragInput
			{
				float4 pos : SV_POSITION;
				//float2 uv : TEXCOORD0;
				float4 xpos : TEXCOORD1;
				
			};
			fragInput vert(vertexInput v)
			{
				fragInput o;
				o.pos = mul( UNITY_MATRIX_MVP, v.vertex);
				//o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.xpos = (v.vertex + float4(1.0, 1.0, 1.0f, 0.0))/2.0;
				return o;
			}
			float4 frag(fragInput o) : COLOR
			{
				//return float4( 1.0, 0.0, 0.0, 1.0);
				//return float4(o.xpos.x, o.xpos.y, o.xpos.z, 1.0);
				float4 x = tex2D(_MainTex, float2(o.xpos.x, o.xpos.y));
				float4 y = tex2D(_SecondaryTex, float2(o.xpos.x, o.xpos.z));
				float4 z = tex2D(_TertiaryTex, float2(o.xpos.y, o.xpos.z));
				return float4((x.x+y.x + z.x)/3.0, (x.y + y.y + z.y)/3.0, (x.z+y.z + z.z)/3.0 ,1.0);
			}
			ENDCG
			}
		}
		
	}