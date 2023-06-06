// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// modified from Terrain-Standard-FirstPass.shader from Unity 2021.2.0f1 built-in shader source
// 27 October 2021: add Triplanar cliff shading

Shader "Nature/Terrain/StandardTriplanar" {
    Properties {
        // used in fallback on old cards & base map
        [HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
        [HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
        [HideInInspector] _TerrainHolesTexture("Holes Map (RGB)", 2D) = "white" {}
        _Smoothness("Smoothness", float) = 0.5

        // BEGIN TRIPLANAR PROPERTIES
        _ThresholdLow("Triplanar Threshold (Low)", Range(0, 1)) = 0.8
        _ThresholdHigh("Triplanar Threshold (High)", Range(0, 1)) = 0.9
        _AntiTilingStrength("Anti-tiling Threshold", Range(0, 1)) = 1
	    _CliffTexture("Cliff texture", 2D) = "white" {}
        _NoiseTexture("Noise texture", 2D) = "white" {}
        [Normal]_CliffNormal("Cliff normal", 2D) = "bump" {} 
        _CliffNormalStrength("Cliff normal strength", float) = 1

    }

    SubShader {
        Tags {
            "Queue" = "Geometry-100"
            "RenderType" = "Opaque"
            "TerrainCompatible" = "True"
        }

        CGPROGRAM
        #pragma surface surf Standard vertex:SplatmapVert finalcolor:SplatmapFinalColor finalgbuffer:SplatmapFinalGBuffer addshadow fullforwardshadows
        #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        #pragma multi_compile_fog // needed because finalcolor oppresses fog code generation.
        #pragma target 3.0
        #include "UnityPBSLighting.cginc"

        #pragma multi_compile_local_fragment __ _ALPHATEST_ON
        #pragma multi_compile_local __ _NORMALMAP

        #define TERRAIN_STANDARD_SHADER
        #define TERRAIN_INSTANCED_PERPIXEL_NORMAL
        #define TERRAIN_SURFACE_OUTPUT SurfaceOutputStandard
        
        // TRIPLANAR: must use custom cginc to override Input struct
        #include "TerrainSplatmapTriplanar.cginc"

        half _Metallic0;
        half _Metallic1;
        half _Metallic2;
        half _Metallic3;

        half _Smoothness0;
        half _Smoothness1;
        half _Smoothness2;
        half _Smoothness3;
		
        // BEGIN TRIPLANAR PROPERTIES
        half _ThresholdLow;
        half _ThresholdHigh;
        sampler2D _CliffTexture;
        float4 _CliffTexture_ST;
        sampler2D _NoiseTexture;
        float4 _NoiseTexture_ST;
	    sampler2D _CliffNormal;
        float4 _CliffNormal_ST;
        float _CliffNormalStrength;
        float _AntiTilingStrength;
        float _Smoothness;

        float3 textureNoTile(sampler2D iChannel0, sampler2D iChannel1, in float2 x, float v);

        // END TRIPLANAR PROPERTIES

        void surf (Input IN, inout SurfaceOutputStandard o) {
            half4 splat_control;
            half weight;
            fixed4 mixedDiffuse;
            half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);
            SplatmapMix(IN, defaultSmoothness, splat_control, weight, mixedDiffuse, o.Normal);
            o.Albedo = mixedDiffuse.rgb;
            o.Alpha = weight;
            o.Smoothness = _Smoothness;
            o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3));


            // BEGIN TRIPLANAR SHADING
	    // get terrain normal and calculate triplanar threshold
	    float3 vec = abs(WorldNormalVector (IN, float3(0,0,1)));
            half vertDot = dot(vec, float3(0, 1, 0));
            half threshold = smoothstep(_ThresholdLow, _ThresholdHigh, abs(vertDot));
			
	    // apply triplanar mapping
            fixed4 cliffColorXY = tex2D(_CliffTexture, IN.worldPos.xy * _CliffTexture_ST.xy);
            fixed4 cliffColorYZ = tex2D(_CliffTexture, IN.worldPos.zy * _CliffTexture_ST.xy);
            fixed4 cliffColor = vec.x * cliffColorYZ + vec.z * cliffColorXY;
 
            o.Albedo = lerp(cliffColor, o.Albedo, threshold);
            o.Smoothness = lerp(0.25 + cliffColor.r, o.Smoothness, threshold); // hack to reuse albedo for cliff smoothness
            o.Occlusion = lerp(0.5 + cliffColor.r * 2, 1, threshold); // hack to reuse albedo for cliff occlusion
            o.Metallic = lerp(0, o.Metallic, threshold);
			
	        float3 cliffNormalXY = UnpackNormalWithScale(tex2D(_CliffNormal, IN.worldPos.xy * _CliffNormal_ST.xy), _CliffNormalStrength);
            float3 cliffNormalYZ = UnpackNormalWithScale(tex2D(_CliffNormal, IN.worldPos.zy * _CliffNormal_ST.xy), _CliffNormalStrength);
            float3 cliffNormal = vec.x * cliffNormalYZ + vec.z * cliffNormalXY;
	        o.Normal = lerp(cliffNormal, o.Normal, threshold);
            // END TRIPLANAR SHADING

            //APPLY ANTI-TILING SHADING
      /*      float3 noiseXY = textureNoTile(_CliffTexture, _NoiseTexture, IN.worldPos.xy * _NoiseTexture_ST.xy * 0.25, 0.1);
            float3 noiseXZ = textureNoTile(_CliffTexture, _NoiseTexture, IN.worldPos.xz * _NoiseTexture_ST.xy * 0.25, 0.1);
            float3 noiseZY = textureNoTile(_CliffTexture, _NoiseTexture, IN.worldPos.zy * _NoiseTexture_ST.xy * 0.25, 0.1);*/
            //fixed4 cliffSecondColorXY = tex2D(_NoiseTexture, IN.worldPos.xy * _NoiseTexture_ST.xy);
            //fixed4 cliffSecondColorYZ = tex2D(_NoiseTexture, IN.worldPos.zy * _NoiseTexture_ST.xy);
            //fixed4 cliffSecondColor = vec.x * cliffSecondColorYZ + vec.z * cliffSecondColorXY;

            o.Albedo = o.Albedo; //+ cliffSecondColor.rgb * 0.5;
        }

        float sum(float3 v) { return v.x + v.y + v.z; }

        float3 textureNoTile(sampler2D iChannel0, sampler2D iChannel1, in float2 x, float v)
        {
            float k = tex2D(iChannel1, 0.005 * x).x; // cheap (cache friendly) lookup

            float2 duvdx = ddx(x);
            float2 duvdy = ddy(x);

            float l = k * 8.0;
            float f = frac(l);

#if 1
            float ia = floor(l); // my method
            float ib = ia + 1.0;
#else
            float ia = floor(l + 0.5); // suslik's method (see comments)
            float ib = floor(l);
            f = min(f, 1.0 - f) * 2.0;
#endif    

            float2 offa = sin(float2(3.0, 7.0) * ia); // can replace with any other hash
            float2 offb = sin(float2(3.0, 7.0) * ib); // can replace with any other hash

            float3 cola = tex2Dgrad(iChannel0, x + v * offa, duvdx, duvdy).xyz;
            float3 colb = tex2Dgrad(iChannel0, x + v * offb, duvdx, duvdy).xyz;

            return lerp(cola, colb, smoothstep(0.2, 0.8, f - 0.1 * sum(cola - colb)));
        }

        ENDCG

        UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
        UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
    }

    Dependency "AddPassShader"    = "Hidden/TerrainEngine/Splatmap/Standard-AddPass"
    Dependency "BaseMapShader"    = "Hidden/TerrainEngine/Splatmap/Standard-Base"
    Dependency "BaseMapGenShader" = "Hidden/TerrainEngine/Splatmap/Standard-BaseGen"

    Fallback "Nature/Terrain/Diffuse"
}
