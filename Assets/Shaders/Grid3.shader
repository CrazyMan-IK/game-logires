Shader "Custom/Grid3"
{
	Properties
	{
		[HideInInspector] _MainTex("Main texture", 2D) = "white" {}
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" "PreviewType" = "Plane" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

			Cull Off
			Lighting Off
			Blend One OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.5

				#include "UnityCG.cginc"
				#include "UnityShaderVariables.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
					float4 world : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.world = mul(unity_ObjectToWorld, v.vertex);
					float2 objectPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));

					//o.uv = v.uv.xy * _MainTex_ST.xy + (_MainTex_ST.zw + objectPos.xy / 10);
					o.uv = v.uv.xy * _MainTex_ST.xy + float2(objectPos.x / 41.59259, objectPos.y / 20);
					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					float4 col = tex2D(_MainTex, i.uv);

					return col;
				}
				ENDCG
			}
		}
}
