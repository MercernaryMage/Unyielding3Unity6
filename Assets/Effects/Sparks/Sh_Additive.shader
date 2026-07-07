// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "HDR/Additive"
{
	Properties
	{
		_MainTex("TextureRGBA", 2D) = "white" {}
		[HDR]_Color("Color", Color) = (1,1,1,1)
	}

		Category
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		 Cull Off Lighting Off ZWrite Off

		SubShader
		{

		Pass
	{
		//Then Additive
		Blend SrcAlpha One

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float4 _Color;

		struct appdata_t {
			fixed4 vertex : POSITION;
			fixed2 texcoord : TEXCOORD0;
			fixed4 color : COLOR;
		};

		struct v2f {
			fixed4 vertex : SV_POSITION;
			fixed2 texcoord : TEXCOORD0;
			fixed4 color : COLOR;
		};

		fixed4 _MainTex_ST;

		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.color = v.color;

			return o;
		}

		fixed4 frag(v2f i) : Color
		{
			fixed4 tex = tex2D(_MainTex, i.texcoord) * i.color;

			return tex * _Color;
		}
		ENDCG
	}
}
	}

}
