Shader "Unlit/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineThick ("Outline Thickness", Range(0,.25)) = 0.2
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _OutlineThick;
            float4 _OutlineColor;
            float4 _MainTex_ST;
            
            void remap01(inout float2 value)
            {
                value -= 0.5; // 0 - .5 = -.5
                value *= 1+_OutlineThick; // -.5 * (1+.2) = -0.6
                value += .5;
            }
            
            v2f vert (appdata v)
            {
                v.vertex *= 1+_OutlineThick;
                
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                remap01(o.uv);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //float2 scaledUV = i.uv;
                //scaledUV -= 0.5; // 0 - .5 = -.5
                //scaledUV *= 1-_OutlineThick; // -.5 * (1+.2) = -0.6
                //scaledUV += .5; // 0 + (1+.2)/2 = .1

                float2 leftUV = i.uv;
                leftUV.x += _OutlineThick/10;
                float2 rightUV = i.uv;
                rightUV.x -= _OutlineThick/10;
                float2 upUV = i.uv;
                upUV.y -= _OutlineThick/10;
                float2 downUV = i.uv;
                downUV.y += _OutlineThick/10;
                
                // sample the texture
                fixed4 colOrigin = tex2D(_MainTex, i.uv);
                fixed4 colScaled = tex2D(_MainTex, leftUV) + tex2D(_MainTex, rightUV) + tex2D(_MainTex, upUV) + tex2D(_MainTex, downUV);
                
                if (colScaled.a > 0.01 && colOrigin.a < 0.01)
                {
                    colOrigin = _OutlineColor;
                }

                clip(colOrigin.a - 0.01);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, colOrigin);
                return colOrigin;
            }
            ENDCG
        }
    }
}
