#include "Lighting.cginc"
#include "UnityStandardCore.cginc"
#include "UnityShadowLibrary.cginc"

#if defined(DIRECTIONAL)
UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);
#endif

half4 _Albedo;
float4 _LightDir;
float4 _LightPos;

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    float3 wpos = float3((i.uv - float2(0.5, 0)) * float2(6, 6.0 / 16 * 9), 2);

#if defined(DIRECTIONAL)

    half3 lightDir = -_LightDir.xyz;
    float4 shadowCoord = mul(unity_WorldToShadow[0], float4(wpos, 1));
    float atten = UNITY_SAMPLE_SHADOW(_ShadowMapTexture, shadowCoord.xyz);

#else

    float3 tolight = _LightPos.xyz - wpos;
    half3 lightDir = normalize (tolight);
    float4 uvCookie = mul (unity_WorldToLight, float4(wpos,1));
    // negative bias because http://aras-p.info/blog/2010/01/07/screenspace-vs-mip-mapping/
    float atten = tex2Dbias (_LightTexture0, float4(uvCookie.xy / uvCookie.w, 0, -8)).w;
    atten *= uvCookie.w < 0;
    float att = dot(tolight, tolight) * _LightPos.w;
    atten *= tex2D (_LightTextureB0, att.rr).UNITY_ATTEN_CHANNEL;

    float4 shadowCoord = mul(unity_WorldToShadow[1], float4(wpos, 1));
    atten *= UnitySampleShadowmap(shadowCoord);

#endif

    SurfaceOutput s;
    s.Albedo = _Albedo;
    s.Normal = float3(0, 0, -1);
    s.Emission = 0;
    s.Specular = 0;
    s.Gloss = 0;
    s.Alpha = 1;

    UnityLight l;
    l.color = _LightColor0.rgb;
    l.dir = lightDir;

    return UnityLambertLight(s, l) * atten;
}
