Shader "Custom/cgLambert"{
	Properties{
	_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader{
	Pass{
	Tags{"LightMode" = "ForwardBase"}
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	
	uniform float4 _Color;
	
	uniform float4 _LightColor0;
	
	struct vertexInput{
	float4 nuVert:POSITION;
	float3 norm:NORMAL;
	};
	
	struct vertexOutput{
	float4 anotherName : SV_POSITION;
	float4 meaninglessName :TEXCOORD0;//: COLOR;
	};
	
	vertexOutput vert(vertexInput v){
		vertexOutput o;
		float3 NormalDirection = normalize(mul(float4(v.norm, 0.0), _World2Object).xyz);
		float3 lightDirection;
		float attenuation = 1.0;
		
		lightDirection = normalize(_WorldSpaceLightPos0.xyz);
		float3 difuseReflection = attenuation * _LightColor0.xyz * max(0.0, dot(NormalDirection, lightDirection));
		
		o.anotherName = mul(UNITY_MATRIX_MVP, v.nuVert);
		o.meaninglessName = float4(difuseReflection, 0.0);
		return o;
	}
	float4 frag(vertexOutput i): COLOR{
		return i.meaninglessName;
	}
	
	ENDCG
	}
	}
}
