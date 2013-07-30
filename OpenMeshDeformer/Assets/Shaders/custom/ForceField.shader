Shader "Custom/ForceFieldAdvanced" {

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
                GLSLPROGRAM                          

                #ifdef VERTEX
                //#include "UnityCG.glslinc"  
                //float time=_Time.w;

                void main()
                {          
        			gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex; //+ sin(time/2.0);
                }

                #endif  

 

             #ifdef FRAGMENT
   			 #include "UnityCG.glslinc"

              //uniform sampler2D _MainTex;
              uniform vec4 baseColor;
              uniform vec4 individualLimit;
              uniform float slice;
              uniform float seed2;

              float time=_Time.w;

				void main( void ) {
			
			    vec2 position = ( gl_FragCoord.xy / _ScreenParams.xy ) ;
			
			    float color = 0.0;
			
			    color += sin( position.x * cos( time / slice ) * 8 * seed2 ) +
			     cos( position.y * cos( time / slice ) * seed2 );
			
			    color += sin( position.y * sin( time / 1000000.0 ) * 10 * seed2 ) +
			     cos( position.x * sin( time / 25.0 ) * 4000000.0 );
			
			    color += sin( position.x * sin( time / 5000000.0 ) * 10.0 ) +
			     sin( position.y * sin( time / 35000000.0 ) * 8 * seed2);
			
			    color *= sin( time / 5.0 ) * 0.5;
			
			 
			
			    gl_FragColor = vec4( vec3(baseColor.x/2.0 + color*individualLimit.x,
			     baseColor.y + color * individualLimit.y ,
			     baseColor.z/2.0 + sin( color + time / 3.0 ) * individualLimit.z ),
			      baseColor.w + color*individualLimit.w);
				}
                #endif                          

                ENDGLSL        

            }

     }

    FallBack "Diffuse"

}
