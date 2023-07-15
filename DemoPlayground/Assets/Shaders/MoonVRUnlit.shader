// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Moon/VR/Unlit Opaque (texture+color+fog+stereo support)"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_ChromaTex ("Chroma", 2D) = "gray" {}
		_Color("Main Color", Color) = (1,1,1,1)
		
		// @usunyu added, support brightness feature
		_Brightness ("Brightness", Range (-0.3, 0.3)) = 0.0
		
		// @usunyu added, support mirror feature
		[Toggle(FLIP_X)] _FlipX("Flip X", Float) = 0

		// @usunyu modified, support Right_Left, Bottom_Top, Half_Left, Half_Right, Half_Top, Half_Bottom
		//[KeywordEnum(None, Top_Bottom, Left_Right, Custom_UV)] Stereo("Stereo Mode", Float) = 0
		[KeywordEnum(None, Top_Bottom, Bottom_Top, Left_Right, Right_Left, Half_Left, Half_Right, Half_Top, Half_Bottom)] Stereo("Stereo Mode", Float) = 0
		
		[Toggle(STEREO_DEBUG)] _StereoDebug("Stereo Debug Tinting", Float) = 0
		[Toggle(APPLY_GAMMA)] _ApplyGamma("Apply Gamma", Float) = 0
		[Toggle(USE_YPCBCR)] _UseYpCbCr("Use YpCbCr", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="False" "Queue"="Geometry" }
		LOD 100
		Lighting Off
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// #pragma multi_compile_fog
			// @usunyu modified, support Right_Left, Bottom_Top, Half_Left, Half_Right, Half_Top, Half_Bottom
			#pragma multi_compile MONOSCOPIC STEREO_TOP_BOTTOM STEREO_BOTTOM_TOP STEREO_LEFT_RIGHT STEREO_RIGHT_LEFT STEREO_HALF_LEFT STEREO_HALF_RIGHT STEREO_HALF_TOP STEREO_HALF_BOTTOM STEREO_CUSTOM_UV

			// TODO: Change XX_OFF to __ for Unity 5.0 and above
			// this was just added for Unity 4.x compatibility as __ causes
			// Android and iOS builds to fail the shader
			#pragma multi_compile __ STEREO_DEBUG
			#pragma multi_compile __ APPLY_GAMMA
			#pragma multi_compile __ USE_YPCBCR
			// @usunyu added, support mirror feature
			#pragma multi_compile FLIP_X_OFF FLIP_X

			#include "UnityCG.cginc"
			#include "./AVProVideo.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
#if STEREO_CUSTOM_UV
				float2 uv2 : TEXCOORD1;	// Custom uv set for right eye (left eye is in TEXCOORD0)
#endif
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
#if UNITY_VERSION >= 500
				UNITY_FOG_COORDS(1)
#endif
#if STEREO_DEBUG
				float4 tint : COLOR;
#endif
			};

			uniform sampler2D _MainTex;
#if USE_YPCBCR
			uniform sampler2D _ChromaTex;
#endif
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color;
			uniform float3 _cameraPosition;
			
			

			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = XFormObjectToClip(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				
				// @usunyu modified, support Right_Left, Bottom_Top, Half_Left, Half_Right, Half_Top, Half_Bottom
#if STEREO_TOP_BOTTOM | STEREO_BOTTOM_TOP | STEREO_LEFT_RIGHT | STEREO_RIGHT_LEFT | STEREO_HALF_LEFT | STEREO_HALF_RIGHT | STEREO_HALF_TOP | STEREO_HALF_BOTTOM
				float4 scaleOffset = GetStereoScaleOffset(IsStereoEyeLeft(), _MainTex_ST.y < 0.0);
				o.uv.xy *= scaleOffset.xy;
				o.uv.xy += scaleOffset.zw;
#elif STEREO_CUSTOM_UV
				if (!IsStereoEyeLeft())
				{
					o.uv.xy = TRANSFORM_TEX(v.uv2, _MainTex);
				}
#endif

#if STEREO_DEBUG
				o.tint = GetStereoDebugTint(IsStereoEyeLeft());
#endif

#if UNITY_VERSION >= 500
				UNITY_TRANSFER_FOG(o, o.vertex);
#endif

				// @usunyu added, support mirror feature
#if FLIP_X
				o.uv.x = 1 - o.uv.x;
#endif
				return o;
			}
			
			// @usunyu added, support brightness feature
			float _Brightness;
			
			fixed4 frag (v2f i) : SV_Target
			{
#if USE_YPCBCR
	#if SHADER_API_METAL || SHADER_API_GLES || SHADER_API_GLES3
				float3 ypcbcr = float3(tex2D(_MainTex, i.uv).r, tex2D(_ChromaTex, i.uv).rg);
	#else
				float3 ypcbcr = float3(tex2D(_MainTex, i.uv).r, tex2D(_ChromaTex, i.uv).ra);
	#endif
				fixed4 col = fixed4(Convert420YpCbCr8ToRGB(ypcbcr), 1.0);
#else
				fixed4 col = tex2D(_MainTex, i.uv.xy);
#endif
				col *= _Color;
#if APPLY_GAMMA
				col.rgb = GammaToLinear(col.rgb);
#endif

#if STEREO_DEBUG
				col *= i.tint;
#endif				

#if UNITY_VERSION >= 500
				UNITY_APPLY_FOG(i.fogCoord, col);
#endif
				// @usunyu added, support brightness feature
				col += _Brightness;
				return fixed4(col.rgb, 1.0);
			}
			ENDCG
		}
	}
}
