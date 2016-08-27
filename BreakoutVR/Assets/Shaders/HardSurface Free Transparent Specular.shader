Shader "GlassColorChanging" {
	Properties {
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_PositionColorImpact ("Position Impact", Range (0.01, 1)) = 0.5
		_AlphaValue ("Alpha", Range (0.01 , 1)) = 0.5
		_RepeatLength ("Length for the colors to repeat", Range (1 , 100)) = 10
	}
	SubShader {
		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		LOD 300
		
		CGPROGRAM
			#pragma surface surf StandardSpecular alpha:auto nolightmap

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
		ENDCG
	}
	FallBack "Transparent/VertexLit"
}