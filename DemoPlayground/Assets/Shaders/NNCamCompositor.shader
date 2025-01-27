Shader "Hidden/NNCam/Compositor"
{
    Properties
    {
        _Background("", 2D) = ""{}
        _CameraFeed("", 2D) = ""{}
        _Mask("", 2D) = ""{}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _Background;
    sampler2D _CameraFeed;
    sampler2D _Mask;
    float _Threshold;

    void Vertex(float4 position : POSITION,
                float2 uv : TEXCOORD0,
                out float4 outPosition : SV_Position,
                out float2 outUV : TEXCOORD0)
    {
        outPosition = UnityObjectToClipPos(position);
        outUV = uv;
    }

    float4 Fragment(float4 position : SV_Position,
                    float2 uv : TEXCOORD0) : SV_Target
    {
        float3 bg = tex2D(_Background, uv).rgb;
        float3 fg = tex2D(_CameraFeed, uv).rgb;
        float3 mask = tex2D(_Mask, uv).rgb;
        float th1 = max(0, _Threshold - 0.1);
        float th2 = min(1, _Threshold + 0.1);
        return float4(lerp(bg, fg, smoothstep(th1, th2, mask.r)), mask.r);
    }

    ENDCG

    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }

    //SubShader 
    //{
    //    Pass 
    //    {
    //        CGPROGRAM
    //        #pragma vertex Vertex
    //        #pragma fragment frag2
    //        struct v2f
    //        {
    //            float4 vertex : SV_POSITION;
    //            float2 uv: TEXCOORD1;
    //        };
    //        #include "UnityCG.cginc"

    //        float4 frag(v2f i) : SV_Target
    //        {
    //            float4 color = tex2D(_SecTex, i.uv);
    //            return color;
    //        }
    //        ENDCG
    //    }
    //}
}
