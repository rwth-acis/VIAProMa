Shader "Custom/Two Sided Grid Shader"
{
	Properties
	{
		_WireThicknessX("Wire Thickness X", Float) = 0.01
		_WireThicknessY("Wire Thickness Y", Float) = 0.01
		_CellWidth("Cell Width", Float) = 1
		_CellHeight("Cell Height", Float) = 1
		_OffsetX("Offset X", Float) = 0
		_OffsetY("Offset Y", Float) = 0
		_BackgroundColor("Background Color", Color) = (0, 0, 0, 0)
		_WireColor("Wire Color", Color) = (1, 1, 1, 1)
	}

		SubShader
	{
		Tags {"Queue" = "Geometry"}

		Pass
		{
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float _WireThicknessX;
			uniform float _WireThicknessY;
			uniform float _CellWidth;
			uniform float _CellHeight;
			uniform float _OffsetX;
			uniform float _OffsetY;
			uniform float4 _BackgroundColor;
			uniform float4 _WireColor;

			struct vertexData
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};


			v2f vert(vertexData input)
			{
				v2f o;
				o.position = UnityObjectToClipPos(input.position);
				o.uv = input.uv;
				return o;
			}

			float4 frag(v2f input) : SV_TARGET
			{
				if (frac((input.uv.x + _OffsetX + _WireThicknessX / 2.0) / _CellWidth) < (_WireThicknessX / _CellWidth)
				|| frac((input.uv.y + _OffsetY + _WireThicknessY / 2.0) / _CellHeight) < (_WireThicknessY / _CellHeight))
				{
					return _WireColor;
				}
				else
				{
					return _BackgroundColor;
				}
			}

			ENDCG
		}
	}
}