Shader "Custom/TestStencilShader"
{
    SubShader
    {
        Pass 
        {
                Stencil 
        {
            Ref 100
            Comp Always
            Pass replace
        }
        ZWrite Off
        }
    }
    Fallback "Diffuse"
}
