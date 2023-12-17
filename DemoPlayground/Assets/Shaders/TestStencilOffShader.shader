Shader "Custom/TestStencilOffShader"
{
    SubShader
    {
        Pass 
        {
            Stencil 
            {
                Ref 90
                Comp LEqual
                Pass keep
            }
        }
    }
    Fallback "Diffuse"
}
