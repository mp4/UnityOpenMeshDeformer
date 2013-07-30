Shader "Unlit/FlippedNormalsSemiTransparent" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite On
	Blend SrcAlpha OneMinusSrcAlpha 
	Pass {
		Lighting Off
		Cull Front
		SetTexture [_MainTex] { combine texture } 
	}
}

	FallBack "Diffuse"
}
