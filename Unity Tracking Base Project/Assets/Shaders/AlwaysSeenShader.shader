Shader "Custom/AlwaysSeenShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _BehindTransparency ("Behind Transparency", Range(0,1)) = 0.3
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGINCLUDE
        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 worldNormal : TEXCOORD1;
            float3 worldPos : TEXCOORD2;
            float4 screenPos : TEXCOORD3; // Stores screen-space position for depth testing
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _Color;
        float _BehindTransparency;
        float _Glossiness;
        float _Metallic;
        fixed4 _LightColor0;
        sampler2D _CameraDepthTexture; // Unity-provided depth texture

        v2f vert (appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.worldNormal = UnityObjectToWorldNormal(v.normal);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.screenPos = ComputeScreenPos(o.pos); // Compute screen position for depth testing
            return o;
        }

        fixed4 LightingFunction(v2f i)
        {
            fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;
            fixed3 normal = normalize(i.worldNormal);
            fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
            float NdotL = max(0, dot(normal, lightDir));

            fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * texColor.rgb;
            fixed3 diffuse = NdotL * _LightColor0.rgb * texColor.rgb;
            fixed3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
            fixed3 reflectDir = reflect(-lightDir, normal);
            float spec = pow(max(dot(viewDir, reflectDir), 0.0), _Glossiness * 128.0);
            fixed3 specular = spec * _LightColor0.rgb * _Metallic;
            fixed3 finalColor = ambient + diffuse + specular;

            return fixed4(finalColor, texColor.a);
        }
        ENDCG

        // *** Single Pass with Per-Pixel Transparency ***
        Pass
        {
            Name "PerPixelTransparencyPass"
            Tags { "LightMode"="ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            ZTest Always
            Cull Off 

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragPerPixel
            #include "UnityCG.cginc"

            fixed4 fragPerPixel (v2f i) : SV_Target
            {
                fixed4 color = LightingFunction(i);

                // Sample scene depth (from the camera's depth texture)
                float sceneDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r;
                sceneDepth = LinearEyeDepth(sceneDepth); // Convert to linear depth space

                // Get the fragment depth (from screen position)
                float fragDepth = i.screenPos.z / i.screenPos.w; 
                fragDepth = LinearEyeDepth(fragDepth); // Convert to linear depth space

                // If the fragment is behind others, reduce its transparency
                if (fragDepth > sceneDepth + 0.0001)
                {
                    color.a *= _BehindTransparency; // Adjust transparency based on depth
                }

                return color;
            }
            ENDCG
        }

    }
    FallBack "Diffuse"
}
