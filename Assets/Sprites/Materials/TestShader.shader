Shader "Unlit/TestShader"
{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		 _MainTex("Base (RGB) Alpha (A)", 2D) = "white"
	}
		Category{
		Lighting Off
		ZWrite On
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		SubShader{
		Pass{
		SetTexture[_MainTex]{
		constantColor[_Color]
		Combine texture * constant, texture * constant
	}
	}
	}
	}
}
