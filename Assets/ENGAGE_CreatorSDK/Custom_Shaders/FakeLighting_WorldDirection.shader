// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/FakeLighting_WorldDirection"
{
     Properties {
        
        _MainTex ("Texture", 2D) = "white" {}
        //_Specular("Specular",Range(0,1000)) = 10
        _BumpMap("BumpMap", 2D) = "bump" {}
        //_Direction ("Direction", vector) = (0,0,0,0)
        _LightSoftness("Light /Distance",Range(0,10)) = 2
        _LightBrightness("Light Brightness",Range(0,10)) = 1
        _Direction_X("Direction X",Range(-1,1)) = 0
        _Direction_Y("Direction Y",Range(-1,1)) = 0
        _Direction_Z("Direction Z",Range(-1,1)) = 0
        _ColorTint("Color Tint",Color) = (1,1,1,1)
        _ShadowTint("Shadow Tint",Color) = (0,0,0,0)
        _ShadowBrightness("Shadow Brightness",Range(0,10)) = 1
        
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"


            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Specular;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float4 _Direction;
            float _Direction_X;
            float _Direction_Y;
            float _Direction_Z;
            fixed4 _ColorTint;
            fixed4 _ShadowTint;
            float _LightSoftness;
            float _LightBrightness;
            float _ShadowBrightness;

            struct input
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
                float2 uv_Normal : TEXCOORD6;
                
            };

            struct v2f {
                float3 worldPos : TEXCOORD0;
                // these three vectors will hold a 3x3 rotation matrix
                // that transforms from tangent to world space
                half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
                // texture coordinate for the normal map
                float2 uv : TEXCOORD4;
                float4 pos : SV_POSITION;

                float3 worldNormal : TEXCOORD5;
                float2 uv_Normal : TEXCOORD6;
                float3 normal : NORMAL;
            };

            
            v2f vert (input v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                o.normal = v.normal;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(worldNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, worldNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, worldNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, worldNormal.z);
                o.uv =TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_Normal = TRANSFORM_TEX(v.uv_Normal, _BumpMap);
                return o;
            }

            

        
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the normal map, and decode from the Unity encoding
                half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
                
                fixed4 colorTexture = tex2D(_MainTex, i.uv);
                // transform normal from tangent to world space
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);

                
                // half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                // half3 worldRefl = reflect(-worldViewDir, worldNormal);
                // half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
                // half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);
               


               

               
                fixed3 Direction;
                    Direction.x=_Direction_X;
                    Direction.y=_Direction_Y;
                    Direction.z=_Direction_Z;
                Direction = mul(normalize(Direction) ,_LightSoftness);
                fixed3 lightDiffrence = (worldNormal) - Direction;
                fixed3 lightDirection = normalize(lightDiffrence);
                fixed  intensity =  dot(-lightDirection, worldNormal);


                fixed4 c = colorTexture *_ShadowTint *_ShadowBrightness;
                               
                c.rgb +=  (intensity *_LightBrightness) * colorTexture *_ColorTint;
                return c;
            }
            ENDCG
        }
    }
}