Shader "Custom/difuseFlippedNormals" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
	Pass {
		Lighting On
		Cull Front
		SetTexture [_MainTex] { combine texture } 
	}
	}
	FallBack "Diffuse"
}

