Shader "Custom/cgDualTexture" {
	Properties {
		_MainTex ("Base texture", 2D) = "white" {}
		_SecondTex ("Base texture", 2D) = "white" {}
		_LerpValue("amount to blend by", Range(0.0, 1.0)) = 0.5
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
		uniform sampler2D _SecondTex;
		uniform float4 _SecondTex_ST;
		uniform half _LerpValue;
		
		struct vertexInput{
		float4 vertex : POSITION;
		float4 texcoord : TEXCOORD0;
		};
		
		struct fragmentInput{
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half2 uv2 : TEXCOORD1;
		};

		fragmentInput vert(vertexInput v)
		{
			fragmentInput o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.uv2 = TRANSFORM_TEX(v.texcoord, _SecondTex);
			return o;
		}
		
		float4 frag(fragmentInput o): COLOR
		{
			half4 mainFinalColor = tex2D(_MainTex, o.uv);//float4(1.0);
			half4 secondFinalColor = tex2D(_SecondTex, o.uv2);
			
			return lerp(mainFinalColor, secondFinalColor, _LerpValue); 
		}

		ENDCG
	} 
	}
	//FallBack "Diffuse"
}
