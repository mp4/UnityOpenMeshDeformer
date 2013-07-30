Shader "Effect/dataField" {
	Properties {
		_MainTex("just for uv coordinates doesn't do anything", 2D) = "white"{}
		_Color("color of the dots", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader {
		Tags { "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass{
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		
		#include "UnityCG.cginc"
		
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color;
		
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
			if(sin(o.uv.x * 1000 + _SinTime.x) > 0.9 &&  sin(o.uv.y*1000 + _SinTime.y) > 0.9)
				return _Color;
			return float4(0.0, 0.0, 0.0, 0.0);
		}

		ENDCG
	} 
	}
	//FallBack "Diffuse"
}
