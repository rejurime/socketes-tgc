/*
* Shader generico para TgcMesh con iluminacion dinamica por pixel (Phong Shading)
* utilizando un tipo de luz Point-Light con atenuacion por distancia
* Hay 3 Techniques, una para cada MeshRenderType:
*	- DIFFUSE_MAP
*/

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

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
	Texture = (texLightMap);
};

//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialAmbientColor; //Color RGB
float4 materialDiffuseColor; //Color ARGB (tiene canal Alpha)
float3 materialSpecularColor; //Color RGB
float materialSpecularExp; //Exponente de specular

//Parametros de las 4 luces
float3 lightColor[4]; //Color RGB de las 4 luces
float4 lightPosition[4]; //Posicion de las 4 luces
float lightIntensity[4]; //Intensidad de las 4 luces
float lightAttenuation[4]; //Factor de atenuacion de las 4 luces
float4 eyePosition; //Posicion de la camara



/**************************************************************************************/
/* DIFFUSE_MAP */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
	float3 LightVec1	: TEXCOORD3;
	float3 LightVec2	: TEXCOORD4;
	float3 LightVec3	: TEXCOORD5;
	float3 LightVec4	: TEXCOORD6;
	float3 HalfNAngleVec1	: TEXCOORD7;
	//float3 HalfNAngleVec2	: TEXCOORD8;
	//float3 HalfNAngleVec3	: TEXCOORD9;
	//float3 HalfNAngleVec4	: TEXCOORD10;
};


//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP vs_DiffuseMap(VS_INPUT_DIFFUSE_MAP input)
{
	VS_OUTPUT_DIFFUSE_MAP output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuacion por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.LightVec1 = lightPosition[0].xyz - output.WorldPosition;
	output.LightVec2 = lightPosition[1].xyz - output.WorldPosition;
	output.LightVec3 = lightPosition[2].xyz - output.WorldPosition;
	output.LightVec4 = lightPosition[3].xyz - output.WorldPosition;

	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.HalfNAngleVec1 = normalize(viewVector + output.LightVec1);
	//output.HalfNAngleVec2 = normalize(viewVector + output.LightVec2);
	//output.HalfNAngleVec3 = normalize(viewVector + output.LightVec3);
	//output.HalfNAngleVec4 = normalize(viewVector + output.LightVec4);

	return output;
}


//Input del Pixel Shader
struct PS_DIFFUSE_MAP
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
	float3 LightVec1	: TEXCOORD3;
	float3 LightVec2	: TEXCOORD4;
	float3 LightVec3	: TEXCOORD5;
	float3 LightVec4	: TEXCOORD6;
	float3 HalfNAngleVec1	: TEXCOORD7;
	//float3 HalfNAngleVec2	: TEXCOORD8;
	//float3 HalfNAngleVec3	: TEXCOORD9;
	//float3 HalfNAngleVec4	: TEXCOORD10;
};

//Pixel Shader
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{
	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten1 = length(input.LightVec1) * lightAttenuation[0];
	float distAtten2 = length(input.LightVec2) * lightAttenuation[1];
	float distAtten3 = length(input.LightVec3) * lightAttenuation[2];
	float distAtten4 = length(input.LightVec4) * lightAttenuation[3];

	//Normalizar vectores
	float3 Nn = normalize(input.WorldNormal);
	float3 Ln1 = normalize(input.LightVec1);
	float3 Ln2 = normalize(input.LightVec2);
	float3 Ln3 = normalize(input.LightVec3);
	float3 Ln4 = normalize(input.LightVec4);

	//Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	float intensity = lightIntensity[0] / distAtten1;
	intensity += lightIntensity[1] / distAtten2;
	intensity += lightIntensity[2] / distAtten3;
	intensity += lightIntensity[3] / distAtten4;

	//Obtener texel de la textura
	float4 texelColor = tex2D(diffuseMap, input.Texcoord);

	//Componente Ambient
  float3 ambientLight = intensity * (lightColor[0] + lightColor[1] + lightColor[2] + lightColor[3]) * materialAmbientColor;

	//Componente Diffuse: N dot L
	//float3 n_dot_l1 = dot(Nn, Ln1);
	//float3 n_dot_l2 = dot(Nn, Ln2);
	//float3 n_dot_l3 = dot(Nn, Ln3);
	//float3 n_dot_l4 = dot(Nn, Ln4);

	//Controlamos que no de negativo
	float3 diffuseLight = intensity * lightColor[0] * materialDiffuseColor.rgb * max(0.0, dot(Nn, Ln1));
	diffuseLight += intensity * lightColor[1] * materialDiffuseColor.rgb * max(0.0, dot(Nn, Ln2));
	diffuseLight += intensity * lightColor[2] * materialDiffuseColor.rgb * max(0.0, dot(Nn, Ln3));
	diffuseLight += intensity * lightColor[3] * materialDiffuseColor.rgb * max(0.0, dot(Nn, Ln4));

	//Componente Specular: (N dot H)^exp
	//float3 n_dot_h1 = dot(Nn, input.HalfNAngleVec1);
	//float3 n_dot_h2 = dot(Nn, input.HalfNAngleVec2);
	//float3 n_dot_h3 = dot(Nn, input.HalfNAngleVec3);
	//float3 n_dot_h4 = dot(Nn, input.HalfNAngleVec4);

/*
	float3 specularLight = n_dot_l1 <= 0.0	? float3(0.0, 0.0, 0.0)
	: (intensity * lightColor[0] * materialSpecularColor * pow(max( 0.0, n_dot_h1), materialSpecularExp));

	specularLight += n_dot_l2 <= 0.0	? float3(0.0, 0.0, 0.0)
	: (intensity * lightColor[1] * materialSpecularColor * pow(max( 0.0, n_dot_h2), materialSpecularExp));

	specularLight += n_dot_l3 <= 0.0	? float3(0.0, 0.0, 0.0)
	: (intensity * lightColor[2] * materialSpecularColor * pow(max( 0.0, n_dot_h3), materialSpecularExp));

	specularLight += n_dot_l4 <= 0.0	? float3(0.0, 0.0, 0.0)
	: (intensity * lightColor[3] * materialSpecularColor * pow(max( 0.0, n_dot_h4), materialSpecularExp));
*/
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
	El color Alpha sale del diffuse material */
	//float4 finalColor = float4(saturate(materialEmissiveColor + ambientLight + diffuseLight) * texelColor + specularLight, materialDiffuseColor.a);
	float4 finalColor = float4(saturate(materialEmissiveColor + ambientLight + diffuseLight) * texelColor, materialDiffuseColor.a);

	return finalColor;
}



/*
* Technique DIFFUSE_MAP
*/
technique DIFFUSE_MAP
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_DiffuseMap();
		PixelShader = compile ps_2_0 ps_DiffuseMap();
	}
}
