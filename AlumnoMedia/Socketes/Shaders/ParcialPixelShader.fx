// ---------------------------------------------------------
// Ejemplo shader Minimo:
// ---------------------------------------------------------

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float3 g_vLightPos; // posicion de la luz en World Space
float3 g_vLightDir; // Direcion de la luz en World Space

// mapa de normales
texture g_Normals;						// mapa de normales
sampler Normales =
sampler_state
{
	Texture = <g_Normals>;
	MipFilter = NONE;
	MinFilter = NONE;
	MagFilter = NONE;
};

float screen_dx;					// tamano de la pantalla en pixels
float screen_dy;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :	POSITION0;
	float2 Texcoord :	TEXCOORD0;
	float4 Color :		COLOR0;
	float4 vPosition :	TEXCOORD1;
};

//Vertex Shader
VS_OUTPUT vs_main(VS_INPUT Input)
{
	VS_OUTPUT Output;

	Output.vPosition = Input.Position;

	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

/**************************************************************************************/
/* PixelShader Ejercicios */
/**************************************************************************************/

float4 ps_ejercicio1(float2 Texcoord : TEXCOORD0, float4 Color : COLOR0, float4 vPosition : TEXCOORD1) : COLOR0
{
	float3 vDirVN = normalize(vPosition - g_vLightPos);
	float3 vDirLN = normalize(g_vLightDir);

	float angle = 50; //Angulo de iluminacion de la luz

	float vl_ld = dot(vDirVN, vDirLN);

	if(vl_ld < cos(angle))
	{
		Color = float4(0,0,0,0);
	}

	return Color;
}

// Border detect
void ps_ejercicio2(float2 screen_pos  : TEXCOORD0,	out float4 Color : COLOR)
{
	float ep = 0.5;
	float4 c0 = tex2D(Normales, screen_pos);
	float4 c1 = tex2D(Normales, screen_pos + float2(0, 1 / screen_dy));

	Color.a = 1;

	if (distance(c0, c1) > ep)
	{
		Color.rgb = 1;
	}
	else
	{
		c1 = tex2D(Normales, screen_pos + float2(1 / screen_dy, 0));

		if (distance(c0, c1) > ep)
		{
			Color.rgb = 1;
		}
		else
		{
			Color.rgb = 0;
		}
	}
}

float4 ps_ejercicio3(float2 Texcoord : TEXCOORD0, float4 Color : COLOR0) : COLOR0
{
	//Formula sacada de la practica de Shaders
	float gris = Color.r * 0.222 + Color.g * 0.707 + Color.b * 0.071;
	return float4(gris, gris, gris, Color.a);
}

float4 ps_ejercicio4(float2 Texcoord : TEXCOORD0, float4 Color : COLOR0) : COLOR0
{
//agregar bandas oscuras de 5 pixeles intercaladas con bandas normales.

//Tenes que usar la posicion en pantalla, eso te lo da float2 vPos : VPOS, que tiene la posicion del pixel en screen space,
//esta en pixeles, o sea de 0 a viewport size. con eso podes aplicar alguna formula con modulo 5 por ejemplo.
//if(Texcoord.x % 5 )
	return Color;
}

float4 ps_ejercicio5(float2 Texcoord : TEXCOORD0, float4 Color : COLOR0) : COLOR0
{
	//Para hacer este deberia aprender a hacer RenderToTexture pero la logica creo que la entiendo
	return Color;
}

//No hay ejercicio 6 en la guia
float4 ps_ejercicio7(float2 Texcoord : TEXCOORD0, float4 Color : COLOR0) : COLOR0
{
	float umbral = 0.5; //Pide 50% y la intensidad va de 0.0 a 1.0
	//Formula sacada de la unidad 8, la parte de HDR
	float intensidad = Color.r * 0.2126 + Color.g * 0.7152 + Color.b * 0.0722;

	if (intensidad > umbral) {
		return 1;
	}
	else
	{
		return 0;
	}
}

// ------------------------------------------------------------------

technique PS1
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_3_0 ps_ejercicio1();
	}
}

technique PS2
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_3_0 ps_ejercicio2();
	}
}

technique PS3
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_3_0 ps_ejercicio3();
	}
}

technique PS4
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_3_0 ps_ejercicio4();
	}
}

technique PS5
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_3_0 ps_ejercicio5();
	}
}

technique PS7
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_3_0 ps_ejercicio7();
	}
}
