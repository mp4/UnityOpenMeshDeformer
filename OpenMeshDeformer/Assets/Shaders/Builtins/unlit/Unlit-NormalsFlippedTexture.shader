// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/TextureFlippedNormals" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
	Pass {
		Lighting Off
		Cull Front
		SetTexture [_MainTex] { combine texture } 
	}
}
}
