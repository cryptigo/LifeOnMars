Shader "Custom/TerrainShader"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _WallTex("WallTexture", 2D) = "white" {}
        _TexScale("Texture Scale", Float) = 1
    }

    SubShader{
        Tags {"RenderType" = "Opaque"} // None of the terrain is going to be transparent.
        LOD 200                        // Only diffuse is needed

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows 
        #pragma target 3.0

        sampler2D _WallTex;
        sampler2D _MainTex;
        float _TexScale;

        struct Input {

            float3 worldPos;
            float3 worldNormal;

        };

        //Run for every pixel on screen
        void surf(Input IN, inout SurfaceOutputStandard o) {

            float3 scaledWorldPos = IN.worldPos / _TexScale;
            float3 pWeight = abs(IN.worldNormal);
            pWeight /= pWeight.x + pWeight.y + pWeight.z;


            float3 xP = tex2D(_WallTex, scaledWorldPos.yz) * pWeight.x;
            float3 yP = tex2D(_MainTex, scaledWorldPos.xz) * pWeight.y;
            float3 zP = tex2D(_WallTex, scaledWorldPos.xy) * pWeight.z;

            o.Albedo = xP + yP + zP;
            


        }

        ENDCG

    }
        Fallback "Diffuse"
}
