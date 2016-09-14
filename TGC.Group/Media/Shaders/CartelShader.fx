

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))
float time;
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


/*********************************************** Vertex Shaders ***************************************************/

struct VS_OUTPUT
{
	float4 Position : POSITION;
	float4 Color : COLOR0;
	float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT VS_rotacion (float4 Position : POSITION, float3 Normal : NORMAL, float4 Color : COLOR, float2 TexCoord : TEXCOORD0)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;

	Out.Position = mul(Position, matWorldViewProj);
	Out.Color = Color;
	Out.TexCoord.y = TexCoord.y + time/4;
	Out.TexCoord.x = TexCoord.x;

	
	return Out;
}

float rand(float2 co){
      return 0.5+(frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453))*0.5;
}

VS_OUTPUT VS_cartelFallando (float4 Position : POSITION, float3 Normal : NORMAL, float4 Color : COLOR, float2 TexCoord : TEXCOORD0)
{
	VS_OUTPUT Out = (VS_OUTPUT)0;

	Out.Position = mul(Position, matWorldViewProj);
	Out.Color = Color;
	Out.TexCoord.y = TexCoord.y + rand(Position.xy) + time/4;
	Out.TexCoord.x = TexCoord.x;

	return Out;
}




/*********************************************** Pixel Shaders ***************************************************/

float4 PS_cartelFallando(VS_OUTPUT In): COLOR
{	
	return tex2D(diffuseMap, In.TexCoord) * time;
}

float4 PS_onlyTexture(VS_OUTPUT In): COLOR
{
	return  tex2D(diffuseMap, In.TexCoord);
}

/*********************************************** Techniques ***************************************************/

technique Rotacion {
	pass p0 {
		VertexShader = compile vs_3_0 VS_rotacion();
		PixelShader = compile ps_3_0 PS_onlyTexture();
	}
}

technique CartelFallando {
	pass p0 {
		VertexShader = compile vs_3_0 VS_cartelFallando();
		PixelShader = compile ps_3_0 PS_onlyTexture();
	}
}

