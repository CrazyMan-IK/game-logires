Shader "Custom/Grid"
{
	Properties
	{
		[HideInInspector] _MainTex("Main texture", 2D) = "white" {}
		_GridColor("Grid color", Color) = (0.0, 0.0, 0.0, 1.0)
		_GridSize("Grid size", Float) = 10.0
		_LineSizeMultiper("Line size multiper", Range(0.0, 20.0)) = 1
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
				float4 _GridColor;
				float _GridSize;
				float _LineSizeMultiper;

				float map(float value, float min1, float max1, float min2, float max2)
				{
					return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
				}

				float r(float n)
				{
					return frac(abs(sin(n * 55.753) * 367.34));
				}

				float r(float2 n)
				{
					return r(dot(n, float2(2.46, -1.21)));
				}

				float4 grid(float3 pos)
				{
					float a = (radians(60.0));
					float zoom = 0.125;
					float2 c = (pos.xy + float2(0.0, pos.z)) * float2(sin(a), 1.0) / _GridSize;

					float l = min(min(1.0,
											(1.0 - (2.0 * abs(frac(c.y * 3.5) - 0.5)))),
											(1.0 - (2.0 * abs(frac(c.x * 4.0) - 0.5))));
					l = smoothstep(0.03 * _LineSizeMultiper, 0.02 * _LineSizeMultiper, l);

					return lerp(0.01, l, 1.0) * _GridColor;
				}

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.world = mul(unity_ObjectToWorld, v.vertex);
					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					float lsMul = _LineSizeMultiper;// / (unity_OrthoParams.y / 10);
					float gSize = _GridSize;// / (unity_OrthoParams.y / 10);

					if (frac((i.world.x + lsMul / 10.0 / 2.0) / gSize) < ((lsMul / gSize) / gSize) ||
						frac((i.world.y + lsMul / 10.0 / 2.0) / gSize) < ((lsMul / gSize) / gSize))
					{
						//return _GridColor;
					}
					else
					{
						//return i.color;
					}

					float4 col = tex2D(_MainTex, i.uv);

					col *= i.color;
					col.rgb *= col.a;

					float4 g = grid(i.world);
					//g = lerp(col, g, g.a);
					//g = map(g, 0.01, 1.0, -1.0, 1.0);

					g.rgb *= g.a;

					col.rgb += g.rgb;

					//col.a = g.a;

					return col;
				}
				ENDCG
			}
		}
}
