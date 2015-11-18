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

float time = 0;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float4 Color : 		COLOR0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :	POSITION0;
	float2 Texcoord :	TEXCOORD0;
	float4 Color :		COLOR0;
};

//Pixel Shader
float4 ps_main(float2 Texcoord: TEXCOORD0, float4 Color:COLOR0) : COLOR0
{
	return Color;
}

/**************************************************************************************/
/* VertexShader Ejercicios */
/**************************************************************************************/

// Ejercicio1
// ------------------------------------------------------------------
VS_OUTPUT vs_ejercicio1(VS_INPUT Input)
{
	VS_OUTPUT Output;

	// Animar posicion
	Input.Position.y = sin(Input.Position.x + time) + cos(Input.Position.z + time);

	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

// Ejercicio2
// ------------------------------------------------------------------
texture height_map;
sampler2D heightMap = sampler_state
{
	Texture = (height_map);
};

VS_OUTPUT vs_ejercicio2(VS_INPUT Input)
{
	VS_OUTPUT Output;

	float4 positionN = normalize(Input.Position);
	float4 color = tex2Dlod(heightMap, float4(Input.Texcoord.x, Input.Texcoord.y, 0,0));
	float constante = 0.75;

	// Animar posicion
	Input.Position = Input.Position + (positionN * constante * color.r * sin(time));

	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

// Ejercicio3
// ------------------------------------------------------------------
VS_OUTPUT vs_ejercicio3(VS_INPUT Input)
{
	VS_OUTPUT Output;

	// Animar posicion
	Input.Position.y = 0;

	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

// EjercicioParcial
// ------------------------------------------------------------------
VS_OUTPUT vs_ejercicio_parcial(VS_INPUT Input)
{
	VS_OUTPUT Output;

	float4 positionN = normalize(Input.Position);
	float4 color = tex2Dlod(heightMap, float4(Input.Texcoord.x, Input.Texcoord.y, 0,0));
	float constante = 0.75;

	// Animar posicion
	Input.Position =  Input.Position + (positionN * constante * color.r * abs(sin(time * 9)));

	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

// ------------------------------------------------------------------

technique VS1
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_ejercicio1();
		PixelShader = compile ps_3_0 ps_main();
	}
}

technique VS2
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_ejercicio2();
		PixelShader = compile ps_3_0 ps_main();
	}
}

technique VS3
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_ejercicio3();
		PixelShader = compile ps_3_0 ps_main();
	}
}

technique VSP
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_ejercicio_parcial();
		PixelShader = compile ps_3_0 ps_main();
	}
}
