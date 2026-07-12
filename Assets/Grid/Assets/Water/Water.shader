Shader "Custom/Water" {
	Properties {
		[MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
	}

	SubShader {
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

		Pass {
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes {
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Varyings {
				float4 positionHCS : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			CBUFFER_START(UnityPerMaterial)
				half4 _BaseColor;
			CBUFFER_END

			Varyings vert(Attributes IN) {
				Varyings OUT;
				OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
				OUT.uv = IN.uv;
				return OUT;
			}

			half4 frag(Varyings IN) : SV_Target {
				return half4(IN.uv, 0, 1);
			}
			ENDHLSL
		}
	}
}
