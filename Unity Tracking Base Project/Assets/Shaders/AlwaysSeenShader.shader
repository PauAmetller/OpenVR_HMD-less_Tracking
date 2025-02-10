Shader "Custom/AlwaysSeenShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _BehindTransparency ("Behind Transparency", Range(0,1)) = 0.5
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
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _Color;
        float _BehindTransparency;
        float _Glossiness;
        float _Metallic;
        fixed4 _LightColor0;

        v2f vert (appdata v)
        {
            v2f o;

            // Transform vertex position to clip space
            o.pos = UnityObjectToClipPos(v.vertex);

            // Transform UVs
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);

            // Calculate world normal
            o.worldNormal = UnityObjectToWorldNormal(v.normal);

            // Calculate world position
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            return o;
        }

        fixed4 LightingFunction(v2f i)
        {
            // Sample the main texture
            fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;

            // Compute lighting
            fixed3 normal = normalize(i.worldNormal);
            fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
            float NdotL = max(0, dot(normal, lightDir));

            // Apply simple diffuse and specular lighting
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

        // *** First Pass: Behind Other Objects ***
        Pass
        {
            Name "BehindPass"
            Tags { "LightMode"="ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Greater
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragBehind
            #include "UnityCG.cginc"

            fixed4 fragBehind (v2f i) : SV_Target
            {
                fixed4 color = LightingFunction(i);
                color.a *= _BehindTransparency;
                return color;
            }
            ENDCG
        }

        // *** Second Pass: In Front of Other Objects ***
        Pass
        {
            Name "InFrontPass"
            Tags { "LightMode"="ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragInFront
            #include "UnityCG.cginc"

            fixed4 fragInFront (v2f i) : SV_Target
            {
                fixed4 color = LightingFunction(i);
                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
