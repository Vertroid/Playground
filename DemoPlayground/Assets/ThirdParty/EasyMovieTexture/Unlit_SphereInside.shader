
// Unlit shader. Simplest possible textured shader.
// - SUPPORTS lightmap
// - no lighting
// - no per-material color

Shader "Custom/Unlit_SphereInside" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_LightColor ("Light Color",COLOR) = (0,0,0,1)
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100

	// Non-lightmapped
	Pass {
		Lighting Off
		Cull Front
		SetTexture [_MainTex] 
		{
		constantColor [_LightColor]
		combine texture + constant
		}
	}
	
	
	

}
}



