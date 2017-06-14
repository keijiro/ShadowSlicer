Shader "Hidden/ShadowSampler"
{
    Properties
    {
        _Albedo("", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Blend one one
        Pass
        {
            CGPROGRAM
            #define SPOT
            #define SHADOWS_DEPTH
            #include "ShadowCollector.cginc"
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #define DIRECTIONAL
            #include "ShadowCollector.cginc"
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
