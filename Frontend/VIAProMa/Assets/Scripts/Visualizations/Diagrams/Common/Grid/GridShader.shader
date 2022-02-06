// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Custom/Grid Shader"
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
		Tags {"Queue" = "Transparent"}

		Pass
		{
			ZWrite Off
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};


			v2f vert(vertexData input)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.position = UnityObjectToClipPos(input.position);
				o.uv = input.uv;
				return o;
			}


			float4 frag(v2f input) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(input);
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