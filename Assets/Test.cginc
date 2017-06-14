
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float3 worldPos : TEXCOORD0;
                    SHADOW_COORDS(1)
                };

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
                return o;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                UNITY_LIGHT_ATTENUATION(atten, IN, IN.worldPos)
                    fixed4 c = atten;
                // might want to take light color into account?
                c.rgb *= _LightColor0.rgb;
                return c;
            }
