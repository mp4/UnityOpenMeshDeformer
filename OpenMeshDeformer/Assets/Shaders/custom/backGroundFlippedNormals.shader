Shader "Custom/backGroundFlippedNormals" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Background"}
	LOD 100
	
	ZWrite OFF
	Blend SrcAlpha OneMinusSrcAlpha 
	Pass {
		Lighting Off
		Cull Front
		SetTexture [_MainTex] { combine texture } 
	}
}

	FallBack "Diffuse"
}
