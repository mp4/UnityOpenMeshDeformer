Shader "Effect/ForceField" {

    Properties {

        //_MainTex ("Base (RGB)", 2D) = "white" {}
        baseColor("Base", Color) = (0.0, 0.0, 0.0, 1.0)
        individualLimit("max", Color) = (1.0, 0.5, 0.75, 0.0)
        slice("PhaseTime", Range(0, 30000000.0)) = 15000000.0
        seed2("horizontalCount", Range(0, 20000000.0)) = 1000000.0

    }

    SubShader
    {

        Tags { "Queue" = "Transparent" }

	
        Pass
            {            
           Blend SrcAlpha OneMinusSrcAlpha
                CGPROGRAM                         

				#pragma exclude_renderers gles flash xbox360 ps3
                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                
                uniform float4 baseColor;
	            uniform float4 individualLimit;
	            uniform float slice;
	            uniform float seed2;
	            
                struct vertexInput{
                	float4 vertex :POSITION;
                	
                };
                struct vertexOutput{
                float4 pos : SV_POSITION;
                float4 vpos : TEXCOORD0;
                };
                vertexOutput vert(vertexInput v)
                {   
                	vertexOutput o;
                	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                	o.vpos = v.vertex + sin(_Time.w/2.0);
        			return o;
                }
              //uniform sampler2D _MainTex;
              

				float4 frag(vertexOutput o): COLOR 
				{
					float time = _Time.w;
				    float2 position = float2( o.vpos.xy / _ScreenParams.xy ) ;
				
				    float color = 0.0;
				
				    color += sin( position.x * cos( time / slice ) * 8 * seed2 ) +
				     cos( position.y * cos( time / slice ) * seed2 );
				
				    color += sin( position.y * sin( time / 1000000.0 ) * 10 * seed2 ) +
				     cos( position.x * sin( time / 25.0 ) * 4000000.0 );
				
				    color += sin( position.x * sin( time / 5000000.0 ) * 10.0 ) +
				     sin( position.y * sin( time / 35000000.0 ) * 8 * seed2);
				
				    color *= sin( time / 5.0 ) * 0.5;
				
				 
				
				    return float4( float3(baseColor.x/2.0 + color*individualLimit.x,
				     baseColor.y + color * individualLimit.y ,
				     baseColor.z/2.0 + sin( color + time / 3.0 ) * individualLimit.z ),
				      baseColor.w + color*individualLimit.w);
					//return float4(1, 0, 0, 1);
				}
                ENDCG        

            }

     }

    FallBack "Diffuse"

}
