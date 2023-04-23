Shader "Custom/ScreenspaceTexturedShader"
{
	Properties
	{
			_Color("Tint", Color) = (0, 0, 0, 1)
			_MainTex("Texture", 2D) = "white" {}
			_Smoothness("Smoothness", Range(0, 1)) = 0
			_Metallic("Metalness", Range(0, 1)) = 0
			_EmissiveMap("Emissive Map", 2D) = "white" {}
			[HDR] _EmissionColor("Emission", color) = (0,0,0,1)
	}

	SubShader
	{
		Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows alpha:fade
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _EmissiveMap;
		float4 _EmissiveMap_ST;
		float4 _MainTex_ST;
		fixed4 _Color;

		half _Smoothness;
		half _Metallic;
		fixed4 _EmissionColor;

		struct Input {
			float4 screenPos;
		};

		void surf(Input i, inout SurfaceOutputStandard o) {
			float2 textureCoordinate = i.screenPos.xy / i.screenPos.w;
			float2 textureCoordinate_emis = i.screenPos.xy / i.screenPos.w;
			float aspect = _ScreenParams.x / _ScreenParams.y;
			textureCoordinate.x = textureCoordinate.x * aspect;
			textureCoordinate_emis.x = textureCoordinate_emis.x * aspect;
			textureCoordinate_emis.y = textureCoordinate_emis.y * aspect;
			textureCoordinate = TRANSFORM_TEX(textureCoordinate, _MainTex);
			textureCoordinate_emis = TRANSFORM_TEX(textureCoordinate_emis, _EmissiveMap);

			fixed4 col = tex2D(_MainTex, textureCoordinate);
			fixed4 c = tex2D(_MainTex, textureCoordinate) * _Color;
			fixed4 emis = tex2D(_EmissiveMap, textureCoordinate_emis) * _EmissionColor;

			col *= _Color;
			o.Albedo = col.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = c.a;
			o.Emission = emis.rgb;
		}
		ENDCG
	}
	FallBack "Standard"
}
