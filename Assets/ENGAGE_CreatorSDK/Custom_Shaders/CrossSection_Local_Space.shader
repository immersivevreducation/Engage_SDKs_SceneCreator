Shader "CrossSection/CrossSection_Local_Space"{
    //show values to edit in inspector
    Properties{
        _Color ("Color", Color) = (0, 0, 0, 1)
        _Plane("plane", Vector ) = (0,0,0,0)
        _cutoffMask ("cutoffMask", Range(-10, 10)) = 0
        _MainTex ("Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0
        _Metallic ("Metalness", Range(0, 1)) = 0
        [HDR]_Emission ("Emission", color) = (0,0,0)

        [HDR]_CutoffColor("Cutoff Color", Color) = (1,0,0,0)
    }

    SubShader{
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

        // render faces regardless if they point towards the camera or away from it
        Cull Off

        CGPROGRAM
        //the shader is a surface shader, meaning that it will be extended by unity in the background
        //to have fancy lighting and other features
        //our surface shader function is called surf and we use our custom lighting model
        //fullforwardshadows makes sure unity adds the shadow passes the shader might need
        //vertex:vert makes the shader use vert as a vertex shader function
        #pragma surface surf Standard   vertex:vert fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;

        half _Smoothness;
        half _Metallic;
        half3 _Emission;

        float4 _Plane;
        float _cutoffMask;

        float4 _CutoffColor;

        //input struct which is automatically filled by unity
        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
            float facing : VFACE;
            float3 localPos;
        };
        void vert (inout appdata_full v, out Input o){
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.localPos = v.vertex.xyz;            
        }

        //the surface shader function which sets parameters the lighting function then uses
        void surf (Input i, inout SurfaceOutputStandard o) {
            //calculate signed distance to plane
            float distance = dot(i.localPos, _Plane.xyz);
            
            distance = distance + _Plane.w;
            //discard surface above plane
            clip(-distance);

            float facing = i.facing * 0.5 + 0.5;

            //normal color stuff
            fixed4 col = lerp(tex2D(_MainTex, i.uv_MainTex)*_Color, _CutoffColor,  clamp(distance - _cutoffMask,0,1));          

            o.Albedo =col.rgb * facing;
            o.Metallic = _Metallic * facing;
            o.Smoothness = _Smoothness * facing;
            o.Emission = lerp(_CutoffColor, _Emission, facing);
        }
        ENDCG
    }
    FallBack "Standard" //fallback adds a shadow pass so we get shadows on other objects
}