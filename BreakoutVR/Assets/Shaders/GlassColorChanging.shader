Shader "GlassColorChanging" {
  Properties
  {
    _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_PositionColorImpact ("Position Impact", Range (0.01, 1)) = 0.5
	_AlphaValue ("Alpha", Range (0.01 , 1)) = 0.5
	_RepeatLength ("Length for the colors to repeat", Range (1 , 100)) = 10
    _LineColor ("Line Color", Color) = (0,0,0,1)
    _LineWidth ("Line Width", float) = 0.075
	_GlowAmount ("Glow Amount", float) = 0.5
  }
  SubShader
  {
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardBase" }
		Blend One OneMinusSrcAlpha
		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma multi_compile_fog
		#pragma multi_compile_fwdbasealpha nodynlightmap nolightmap noshadow
		#include "HLSLSupport.cginc"
		#include "UnityShaderVariables.cginc"
		#define UNITY_PASS_FORWARDBASE
		#define _ALPHAPREMULTIPLY_ON 1
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "UnityPBSLighting.cginc"
		#include "AutoLight.cginc"
		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl
		#define WorldNormalVector(data,normal) normal

		// vertex-to-fragment interpolation data
		// no lightmaps:
		#ifdef LIGHTMAP_OFF
		struct v2f_surf {
		  float4 pos : SV_POSITION;
		  half3 worldNormal : TEXCOORD0;
		  float3 worldPos : TEXCOORD1;
		  #if UNITY_SHOULD_SAMPLE_SH
		  half3 sh : TEXCOORD2; // SH
		  #endif
		  UNITY_FOG_COORDS(3)
		  UNITY_INSTANCE_ID
		};
		#endif
		// with lightmaps:
		#ifndef LIGHTMAP_OFF
		struct v2f_surf {
		  float4 pos : SV_POSITION;
		  half3 worldNormal : TEXCOORD0;
		  float3 worldPos : TEXCOORD1;
		  float4 lmap : TEXCOORD2;
		  UNITY_FOG_COORDS(3)
		  UNITY_INSTANCE_ID
		};
		#endif

		half _Shininess;
		half _PositionColorImpact;
		half _AlphaValue;
		half _RepeatLength;
			
		struct Input {
			float3 worldPos;
		};
			
		void surf (Input IN, inout SurfaceOutputStandardSpecular  o) {
			o.Albedo = _SpecColor;
			o.Specular = _Shininess;
			o.Alpha = _AlphaValue;

			float posModifiedX = IN.worldPos[0] / _RepeatLength;
			float posModifiedY = IN.worldPos[1] / _RepeatLength;
			float posModifiedZ = IN.worldPos[2] / _RepeatLength;

			o.Albedo[0] += sin(posModifiedX*posModifiedY) * _PositionColorImpact;
			o.Albedo[1] += sin(posModifiedY*posModifiedZ) * _PositionColorImpact;
			o.Albedo[2] += sin(posModifiedZ*posModifiedX) * _PositionColorImpact;
		}

		// vertex shader
		v2f_surf vert_surf (appdata_full v) {
		  UNITY_SETUP_INSTANCE_ID(v);
		  v2f_surf o;
		  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
		  UNITY_TRANSFER_INSTANCE_ID(v,o);
		  o.pos = UnityObjectToClipPos(v.vertex);
		  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		  fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
		  #if !defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_COMBINED)
		  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
		  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
		  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
		  #endif
		  #if !defined(LIGHTMAP_OFF) && defined(DIRLIGHTMAP_COMBINED)
		  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
		  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
		  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
		  #endif
		  o.worldPos = worldPos;
		  o.worldNormal = worldNormal;
		  #ifndef LIGHTMAP_OFF
		  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		  #endif

		  // SH/ambient and vertex lights
		  #ifdef LIGHTMAP_OFF
			#if UNITY_SHOULD_SAMPLE_SH
			  o.sh = 0;
			  // Approximated illumination from non-important point lights
			  #ifdef VERTEXLIGHT_ON
				o.sh += Shade4PointLights (
				  unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				  unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				  unity_4LightAtten0, worldPos, worldNormal);
			  #endif
			  o.sh = ShadeSHPerVertex (worldNormal, o.sh);
			#endif
		  #endif // LIGHTMAP_OFF

		  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
		  return o;
		}

		// fragment shader
		fixed4 frag_surf (v2f_surf IN) : SV_Target {
		  UNITY_SETUP_INSTANCE_ID(IN);
		  // prepare and unpack data
		  Input surfIN;
		  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
		  surfIN.worldPos.x = 1.0;
		  float3 worldPos = IN.worldPos;
		  #ifndef USING_DIRECTIONAL_LIGHT
			fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
		  #else
			fixed3 lightDir = _WorldSpaceLightPos0.xyz;
		  #endif
		  fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
		  surfIN.worldPos = worldPos;
		  #ifdef UNITY_COMPILER_HLSL
		  SurfaceOutputStandardSpecular o = (SurfaceOutputStandardSpecular)0;
		  #else
		  SurfaceOutputStandardSpecular o;
		  #endif
		  o.Albedo = 0.0;
		  o.Emission = 0.0;
		  o.Specular = 0.0;
		  o.Alpha = 0.0;
		  o.Occlusion = 1.0;
		  fixed3 normalWorldVertex = fixed3(0,0,1);
		  o.Normal = IN.worldNormal;
		  normalWorldVertex = IN.worldNormal;

		  // call surface function
		  surf(surfIN, o);

		  // compute lighting & shadowing factor
		  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
		  fixed4 c = 0;

		  // Setup lighting environment
		  UnityGI gi;
		  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
		  gi.indirect.diffuse = 0;
		  gi.indirect.specular = 0;
		  #if !defined(LIGHTMAP_ON)
			  gi.light.color = _LightColor0.rgb;
			  gi.light.dir = lightDir;
			  gi.light.ndotl = LambertTerm (o.Normal, gi.light.dir);
		  #endif
		  // Call GI (lightmaps/SH/reflections) lighting function
		  UnityGIInput giInput;
		  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
		  giInput.light = gi.light;
		  giInput.worldPos = worldPos;
		  giInput.worldViewDir = worldViewDir;
		  giInput.atten = atten;
		  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
			giInput.lightmapUV = IN.lmap;
		  #else
			giInput.lightmapUV = 0.0;
		  #endif
		  #if UNITY_SHOULD_SAMPLE_SH
			giInput.ambient = IN.sh;
		  #else
			giInput.ambient.rgb = 0.0;
		  #endif
		  giInput.probeHDR[0] = unity_SpecCube0_HDR;
		  giInput.probeHDR[1] = unity_SpecCube1_HDR;
		  #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
			giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
		  #endif
		  #if UNITY_SPECCUBE_BOX_PROJECTION
			giInput.boxMax[0] = unity_SpecCube0_BoxMax;
			giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
			giInput.boxMax[1] = unity_SpecCube1_BoxMax;
			giInput.boxMin[1] = unity_SpecCube1_BoxMin;
			giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
		  #endif
		  LightingStandardSpecular_GI(o, giInput, gi);

		  // realtime lighting: call lighting function
		  c += LightingStandardSpecular (o, worldViewDir, gi);
		  UNITY_APPLY_FOG(IN.fogCoord, c); // apply fog
		  return c;
		}

		ENDCG

	}

	// ---- forward rendering additive lights pass:
	Pass {
		Name "FORWARD"
		Tags { "LightMode" = "ForwardAdd" }
		ZWrite Off Blend One One
		Blend One One

		CGPROGRAM
		// compile directives
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma multi_compile_fog
		#pragma multi_compile_fwdadd nodynlightmap nolightmap noshadow
		#pragma skip_variants INSTANCING_ON
		#include "HLSLSupport.cginc"
		#include "UnityShaderVariables.cginc"
		// Surface shader code generated based on:
		// writes to per-pixel normal: no
		// writes to emission: no
		// writes to occlusion: no
		// needs world space reflection vector: no
		// needs world space normal vector: no
		// needs screen space position: no
		// needs world space position: YES
		// needs view direction: no
		// needs world space view direction: no
		// needs world space position for lighting: YES
		// needs world space view direction for lighting: YES
		// needs world space view direction for lightmaps: no
		// needs vertex color: no
		// needs VFACE: no
		// passes tangent-to-world matrix to pixel shader: no
		// reads from normal: no
		// 0 texcoords actually used
		#define UNITY_PASS_FORWARDADD
		#define _ALPHAPREMULTIPLY_ON 1
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "UnityPBSLighting.cginc"
		#include "AutoLight.cginc"

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl
		#define WorldNormalVector(data,normal) normal

		// Original surface shader snippet:

		half _Shininess;
		half _PositionColorImpact;
		half _AlphaValue;
		half _RepeatLength;
			
		struct Input {
			float3 worldPos;
		};
			
		void surf (Input IN, inout SurfaceOutputStandardSpecular  o) {
			o.Albedo = _SpecColor;
			o.Specular = _Shininess;
			o.Alpha = _AlphaValue;

			float posModifiedX = IN.worldPos[0] / _RepeatLength;
			float posModifiedY = IN.worldPos[1] / _RepeatLength;
			float posModifiedZ = IN.worldPos[2] / _RepeatLength;

			o.Albedo[0] += sin(posModifiedX*posModifiedY) * _PositionColorImpact;
			o.Albedo[1] += sin(posModifiedY*posModifiedZ) * _PositionColorImpact;
			o.Albedo[2] += sin(posModifiedZ*posModifiedX) * _PositionColorImpact;
		}

		// vertex-to-fragment interpolation data
		struct v2f_surf {
		  float4 pos : SV_POSITION;
		  half3 worldNormal : TEXCOORD0;
		  float3 worldPos : TEXCOORD1;
		  UNITY_FOG_COORDS(2)
		};

		// vertex shader
		v2f_surf vert_surf (appdata_full v) {
		  v2f_surf o;
		  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
		  o.pos = UnityObjectToClipPos(v.vertex);
		  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		  fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
		  o.worldPos = worldPos;
		  o.worldNormal = worldNormal;

		  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
		  return o;
		}

		// fragment shader
		fixed4 frag_surf (v2f_surf IN) : SV_Target {
		  // prepare and unpack data
		  Input surfIN;
		  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
		  surfIN.worldPos.x = 1.0;
		  float3 worldPos = IN.worldPos;
		  #ifndef USING_DIRECTIONAL_LIGHT
			fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
		  #else
			fixed3 lightDir = _WorldSpaceLightPos0.xyz;
		  #endif
		  fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
		  surfIN.worldPos = worldPos;
		  #ifdef UNITY_COMPILER_HLSL
		  SurfaceOutputStandardSpecular o = (SurfaceOutputStandardSpecular)0;
		  #else
		  SurfaceOutputStandardSpecular o;
		  #endif
		  o.Albedo = 0.0;
		  o.Emission = 0.0;
		  o.Specular = 0.0;
		  o.Alpha = 0.0;
		  o.Occlusion = 1.0;
		  fixed3 normalWorldVertex = fixed3(0,0,1);
		  o.Normal = IN.worldNormal;
		  normalWorldVertex = IN.worldNormal;

		  // call surface function
		  surf (surfIN, o);
		  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
		  fixed4 c = 0;

		  // Setup lighting environment
		  UnityGI gi;
		  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
		  gi.indirect.diffuse = 0;
		  gi.indirect.specular = 0;
		  #if !defined(LIGHTMAP_ON)
			  gi.light.color = _LightColor0.rgb;
			  gi.light.dir = lightDir;
			  gi.light.ndotl = LambertTerm (o.Normal, gi.light.dir);
		  #endif
		  gi.light.color *= atten;
		  c += LightingStandardSpecular (o, worldViewDir, gi);
		  UNITY_APPLY_FOG(IN.fogCoord, c); // apply fog
		  return c;
		}
	ENDCG
	}

	
    Pass
    {
	  NAME "OUTLINE"
      Tags { "RenderType" = "Transparent" }
      Blend SrcAlpha OneMinusSrcAlpha
      AlphaTest Greater 0.5
 
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
 
      uniform float4 _LineColor;
      uniform float _LineWidth;
	  uniform float _GlowAmount;
 
      // vertex input: position, uv1, uv2
      struct appdata
      {
        float4 vertex : POSITION;
        float4 texcoord1 : TEXCOORD0;
        float4 color : COLOR;
      };
 
      struct v2f
      {
        float4 pos : POSITION;
        float4 texcoord1 : TEXCOORD0;
        float4 color : COLOR;
      };

      v2f vert (appdata v)
      {
        v2f o;
        o.pos = mul( UNITY_MATRIX_MVP, v.vertex);
        o.texcoord1 = v.texcoord1;
        o.color = v.color;
        return o;
      }

	  float ilerp(float a, float b, float c)
	  {
			return ((c-a)/(b-a));
	  }

	  float computeglowfactor(float texCoordAxis)
	  {
			float glowFactor=0.0;
			if(texCoordAxis <= _LineWidth)
			{
				glowFactor = ilerp(_LineWidth,0.0,texCoordAxis);
			}
			else if(texCoordAxis >= 1.0  -_LineWidth)
			{
				glowFactor = ilerp(1-_LineWidth,1.0,texCoordAxis);
			}
			return glowFactor;
	  }
 
      fixed4 frag(v2f i) : COLOR
      {
        fixed4 answer;
 
		//return true if the current x texcoord pos is inferior to 0+_LineWidth
        float lx = step(_LineWidth, i.texcoord1.x);
		//return true if the current y texcoord pos is inferior to 0+_LineWidth
        float ly = step(_LineWidth, i.texcoord1.y);
		//return true if the current x texcoord pos is superior to 1-_LineWidth
        float hx = step(i.texcoord1.x, 1.0 - _LineWidth);
		//return true if the current y texcoord pos is superior to 1-_LineWidth
        float hy = step(i.texcoord1.y, 1.0 - _LineWidth);
		

		float glowFactorX = computeglowfactor(i.texcoord1.x);
		float glowFactorY = computeglowfactor(i.texcoord1.y);
		float finalGlowFactor = 0.0f;

        answer = lerp(_LineColor, float4(0,0,0,0), lx*ly*hx*hy);

		if(lx == 1 && ly == 1)
		{ 
			if(glowFactorX>glowFactorY)
			{
				finalGlowFactor = glowFactorX;
			}
			else
			{
				finalGlowFactor = glowFactorY;
			}
		}
		else if (lx == 1 && hy == 1)
		{
			if(glowFactorX>glowFactorY)
			{
				finalGlowFactor = glowFactorX;
			}
			else
			{
				finalGlowFactor = glowFactorY;
			}
		}
		else if (lx == 1)
		{
			finalGlowFactor = glowFactorX;
		}

		else if(hx == 1 && ly == 1)
		{
			if(glowFactorX>glowFactorY)
			{
				finalGlowFactor = glowFactorX;
			}
			else
			{
				finalGlowFactor = glowFactorY;
			}
		}
		else if(hx == 1 && hy == 1)
		{
			if(glowFactorX>glowFactorY)
			{
				finalGlowFactor = glowFactorX;
			}
			else
			{
				finalGlowFactor = glowFactorY;
			}
		}
		else if(hx == 1)
		{
			finalGlowFactor = glowFactorX;
		}
		else if (ly == 1)
		{
			finalGlowFactor = glowFactorY;
		}
		else if (hy == 1)
		{
			finalGlowFactor = glowFactorY;
		}


        return answer * finalGlowFactor * _GlowAmount;
      }
      ENDCG
  }
 }
}