// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Cel" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Threshold("Threshold", float) = 5
		_AmbientStrength("Ambient Strength", Range(0, 5)) = 0.1
		_SpecularColor("SpecularColor", Color) = (1, 1, 1, 1)
		_Shininess("Shininess", float) = 1
		_OutlineColor("OutlineColor", Color) = (0, 0, 0, 0)
		_OutlineScale("OutlineScale", float) = 1.1
		_Highlighted("HighLighted", int) = 1
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" "Queue" = "Transparent" }
		Cull Off
		ZWrite Off
		Pass
	{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	float4 _OutlineColor;
	float _OutlineScale;
	int _Highlighted;

	struct v2f {
		float4 pos : SV_POSITION;
	};

	v2f vert(appdata_full v)
	{
		v2f o;
		v.vertex.xyz *= _OutlineScale;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		if (_Highlighted == 0)
		{
		discard;
			return float4(0, 0, 0, 0);
			}
		else
			return _OutlineColor;
	}

		ENDCG
	}

		Cull Back
		ZWrite On
		Pass
	{
		CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 worldNormal : NORMAL;
	};

	float _Threshold;
	float _Shininess;
	float _AmbientStrength;
	float4 _SpecularColor;
	float4 _Color;
	sampler2D _MainTex;
	float4 _MainTex_ST;

	fixed4 _LightColor0;
	fixed4 _ModelLightColor0;

	float DiffuseCelShading(float3 normal, float3 lightDir)
	{
		float NdotL = max(0.0, dot(normal, lightDir));
		return floor(NdotL * _Threshold) / (_Threshold - 0.5);
	}

	v2f vert(appdata_full v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.worldNormal = mul(v.normal.xyz, (float3x3) unity_WorldToObject);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 color = tex2D(_MainTex, i.uv) * _Color;// *_ModelLightColor0;

	float3 normal = normalize(i.worldNormal);
	float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

	//Diffuse
	float diffuse = DiffuseCelShading(normal, lightDirection);
	color.rgb *= (diffuse + _AmbientStrength) * _LightColor0.rgb;

	//Specular
	float3 r = reflect(lightDirection, normal);
	float vdc = max(dot(normalize(-_WorldSpaceCameraPos), r), 0.0f);
	float specular = pow(vdc, _Shininess);
	color.rgb += (floor(specular * _Threshold) / _Threshold) * _SpecularColor.rgb;

	return color;
	}

		ENDCG
	}
	}
}
