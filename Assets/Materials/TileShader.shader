// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/TileShader"
{
	Properties
	{
		_MainTex("Color (RGB) Alpha (A)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		repeatsX("Repeats X", Float) = 1
		repeatsY("Repeats Y", Float) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 worldVertex : COLOR0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float repeatsX;
			float repeatsY;
			
			float fract(float val)
			{
				return val - floor(val);	
			}

			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				i.uv.x = fract((i.uv.x - 0.5) * repeatsX);
				i.uv.y = fract((i.uv.y - 0.5) * repeatsY);
				float4 res = tex2D(_MainTex, i.uv);
				if (res.a == 0)
					discard;
				return res * _Color;
			}
			ENDCG
		}
	}
}
