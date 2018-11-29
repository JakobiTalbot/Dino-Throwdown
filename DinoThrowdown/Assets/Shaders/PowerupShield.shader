// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:32872,y:32706,varname:node_2865,prsc:2|emission-460-OUT;n:type:ShaderForge.SFN_Tex2d,id:8907,x:32114,y:32816,ptovrint:False,ptlb:node_8907,ptin:_node_8907,varname:node_8907,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:347466fa4c22f6d4897bf82e0584665f,ntxv:2,isnm:False|UVIN-7986-UVOUT;n:type:ShaderForge.SFN_Fresnel,id:82,x:32114,y:33019,varname:node_82,prsc:2|EXP-7877-OUT;n:type:ShaderForge.SFN_Multiply,id:5180,x:32334,y:32816,varname:node_5180,prsc:2|A-8907-RGB,B-82-OUT,C-9129-OUT;n:type:ShaderForge.SFN_Clamp01,id:5199,x:32504,y:32816,varname:node_5199,prsc:2|IN-5180-OUT;n:type:ShaderForge.SFN_Color,id:4660,x:32389,y:32655,ptovrint:False,ptlb:node_4660,ptin:_node_4660,varname:node_4660,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.4344827,c3:0,c4:1;n:type:ShaderForge.SFN_Clamp01,id:7496,x:32404,y:32492,varname:node_7496,prsc:2|IN-2820-RGB;n:type:ShaderForge.SFN_SceneColor,id:2820,x:32209,y:32492,varname:node_2820,prsc:2;n:type:ShaderForge.SFN_Lerp,id:460,x:32619,y:32613,varname:node_460,prsc:2|A-7496-OUT,B-4660-RGB,T-5199-OUT;n:type:ShaderForge.SFN_Slider,id:7877,x:31792,y:33046,ptovrint:False,ptlb:Fresnel Strech,ptin:_FresnelStrech,varname:node_7877,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2615074,max:3;n:type:ShaderForge.SFN_Slider,id:9129,x:32088,y:33233,ptovrint:False,ptlb:Transparent,ptin:_Transparent,varname:node_9129,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.631499,max:1;n:type:ShaderForge.SFN_TexCoord,id:8382,x:31420,y:32540,varname:node_8382,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:8240,x:31773,y:32684,varname:node_8240,prsc:2,spu:0,spv:1|UVIN-8382-UVOUT,DIST-9311-OUT;n:type:ShaderForge.SFN_Panner,id:7986,x:31927,y:32816,varname:node_7986,prsc:2,spu:1,spv:0|UVIN-8240-UVOUT,DIST-7550-OUT;n:type:ShaderForge.SFN_Multiply,id:7550,x:31627,y:32848,varname:node_7550,prsc:2|A-2963-T,B-528-OUT;n:type:ShaderForge.SFN_Multiply,id:9311,x:31570,y:32694,varname:node_9311,prsc:2|A-2963-T,B-3779-OUT;n:type:ShaderForge.SFN_Time,id:2963,x:31255,y:32669,varname:node_2963,prsc:2;n:type:ShaderForge.SFN_Slider,id:3779,x:31239,y:32906,ptovrint:False,ptlb:ChangeSpeedVertical,ptin:_ChangeSpeedVertical,varname:node_3779,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5555553,max:1;n:type:ShaderForge.SFN_Slider,id:528,x:31216,y:33034,ptovrint:False,ptlb:ChangeSpeedHorizontal,ptin:_ChangeSpeedHorizontal,varname:node_528,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;proporder:8907-4660-7877-9129-3779-528;pass:END;sub:END;*/

Shader "Shader Forge/PowerupShield" {
    Properties {
        _node_8907 ("node_8907", 2D) = "black" {}
        _node_4660 ("node_4660", Color) = (1,0.4344827,0,1)
        _FresnelStrech ("Fresnel Strech", Range(0, 3)) = 0.2615074
        _Transparent ("Transparent", Range(0, 1)) = 0.631499
        _ChangeSpeedVertical ("ChangeSpeedVertical", Range(0, 1)) = 0.5555553
        _ChangeSpeedHorizontal ("ChangeSpeedHorizontal", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _node_8907; uniform float4 _node_8907_ST;
            uniform float4 _node_4660;
            uniform float _FresnelStrech;
            uniform float _Transparent;
            uniform float _ChangeSpeedVertical;
            uniform float _ChangeSpeedHorizontal;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float4 node_2963 = _Time + _TimeEditor;
                float2 node_7986 = ((i.uv0+(node_2963.g*_ChangeSpeedVertical)*float2(0,1))+(node_2963.g*_ChangeSpeedHorizontal)*float2(1,0));
                float4 _node_8907_var = tex2D(_node_8907,TRANSFORM_TEX(node_7986, _node_8907));
                float3 emissive = lerp(saturate(sceneColor.rgb),_node_4660.rgb,saturate((_node_8907_var.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelStrech)*_Transparent)));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _node_8907; uniform float4 _node_8907_ST;
            uniform float4 _node_4660;
            uniform float _FresnelStrech;
            uniform float _Transparent;
            uniform float _ChangeSpeedVertical;
            uniform float _ChangeSpeedHorizontal;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 node_2963 = _Time + _TimeEditor;
                float2 node_7986 = ((i.uv0+(node_2963.g*_ChangeSpeedVertical)*float2(0,1))+(node_2963.g*_ChangeSpeedHorizontal)*float2(1,0));
                float4 _node_8907_var = tex2D(_node_8907,TRANSFORM_TEX(node_7986, _node_8907));
                o.Emission = lerp(saturate(sceneColor.rgb),_node_4660.rgb,saturate((_node_8907_var.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelStrech)*_Transparent)));
                
                float3 diffColor = float3(0,0,0);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, 0, specColor, specularMonochrome );
                o.Albedo = diffColor;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
