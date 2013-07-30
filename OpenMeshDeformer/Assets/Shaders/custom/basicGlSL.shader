Shader "Custom/ForceFieldBasic" {

    Properties {

       // _MainTex ("Base (RGB)", 2D) = "white" {}
       testColor ("color start", Range(0.0, 1.0)) = 0.0

    }

    SubShader

    {

        Tags { "Queue" = "Geometry" }

        Pass

            {            

                GLSLPROGRAM                          

                #ifdef VERTEX  

                void main()

                {          

        gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;

                }

                #endif  

 

             #ifdef FRAGMENT

    #include "UnityCG.glslinc"

              //uniform sampler2D _MainTex;
              uniform float testColor;

              float time=_Time.w;

 

void main( void ) {

 

    vec2 position = ( gl_FragCoord.xy / _ScreenParams.xy ) ;

 

    float color = testColor;//0.0;

    color += sin( position.x * cos( time / 15000000.0 ) * 8000000.0 ) + cos( position.y * cos( time / 15000000.0 ) * 1000000.0 );

    color += sin( position.y * sin( time / 1000000.0 ) * 1000000.0 ) + cos( position.x * sin( time / 25.0 ) * 4000000.0 );

    color += sin( position.x * sin( time / 5000000.0 ) * 10.0 ) + sin( position.y * sin( time / 35000000.0 ) * 8000000.0 );

    color *= sin( time / 5.0 ) * 0.5;

 

    gl_FragColor = vec4( vec3( color, color * 0.5, sin( color + time / 3.0 ) * 0.75 ), 1.0 );

 

}

 

                #endif                          

                ENDGLSL        

            }

     }

    FallBack "Diffuse"

}