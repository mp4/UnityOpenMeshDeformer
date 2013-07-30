Shader "Custom/cgTexture" {
	Properties {
		_MainTex ("Base texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass{
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		
		#include "UnityCG.cginc"
		
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		
		struct vertexInput{
		float4 vertex : POSITION;
		float4 texcoord : TEXCOORD0;
		};
		
		struct fragmentInput{
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		};

		fragmentInput vert(vertexInput v)
		{
			fragmentInput o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			return o;
		}
		
		float4 frag(fragmentInput o): COLOR
		{
			return tex2D(_MainTex, o.uv);//float4(1.0);
		}

		ENDCG
	} 
	}
	//FallBack "Diffuse"
}
