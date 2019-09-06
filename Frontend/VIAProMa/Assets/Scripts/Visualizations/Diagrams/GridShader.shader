Shader "Custom/Grid Shader"
{
	Properties
	{
		_WireThickness("Wire Thickness", Float) = 0.01
		_CellWidth("Cell Width", Float) = 1
		_CellHeight("Cell Height", Float) = 1
		_OffsetX("Offset X", Float) = 0
		_OffsetY("Offset Y", Float) = 0
		_BackgroundColor("Background Color", Color) = (0, 0, 0, 0)
		_WireColor("Wire Color", Color) = (1, 1, 1, 1)
	}

		SubShader
	{
		Tags {"Queue" = "Transparent"}

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float _WireThickness;
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

			struct vertexOutput
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};


			vertexOutput vert(vertexData input)
			{
				vertexOutput o;
				o.position = UnityObjectToClipPos(input.position);
				o.uv = input.uv;
				return o;
			}

			float4 frag(vertexOutput input) : SV_TARGET
			{
				if (frac((input.uv.x + _OffsetX + _WireThickness / 2.0) / _CellWidth) < (_WireThickness / _CellWidth)
				|| frac((input.uv.y + _OffsetY + _WireThickness / 2.0) / _CellHeight) < (_WireThickness / _CellHeight))
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