Shader "Unlit/AlphaOverlay" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue" = "Overlay+1"}
	LOD 100
	
	Ztest Always
	Blend SrcAlpha OneMinusSrcAlpha 
	Pass {
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
	}
	} 
	FallBack "Diffuse"
}
