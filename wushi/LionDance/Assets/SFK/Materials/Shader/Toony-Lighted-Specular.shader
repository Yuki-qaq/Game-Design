Shader "Toon/Lighted-Specular" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
		
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
CGPROGRAM
#pragma surface surf ToonRamp

sampler2D _Ramp;
half _Shininess;

// custom lighting function that uses a texture ramp based
// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	#ifndef USING_DIRECTIONAL_LIGHT
	lightDir = normalize(lightDir);
	#endif
	
	half d = dot (s.Normal, lightDir) * 0.5 + 0.5;
	half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;

	s.Normal = normalize(s.Normal);
	half3 h = normalize(lightDir + viewDir);
	float nh = max(0, dot(s.Normal, h));
	float spec = pow(nh, s.Specular*256.0);
	
	half4 c;
	c.rgb = (s.Albedo *  ramp + spec * 3.0) * _LightColor0.rgb * atten;
	c.a = 0;
	return c;
}


sampler2D _MainTex;

struct Input {
	float2 uv_MainTex : TEXCOORD0;
};

void surf (Input IN, inout SurfaceOutput o) {
	half4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Specular = _Shininess;
}
ENDCG

	} 

	Fallback "Diffuse"
}
