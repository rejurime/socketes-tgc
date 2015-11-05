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

//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialAmbientColor; //Color RGB
float4 materialDiffuseColor; //Color ARGB (tiene canal Alpha)
float3 materialSpecularColor; //Color RGB
float materialSpecularExp; //Exponente de specular

//Parametros de la Luz
float3 lightColor[4]; //Color RGB de la luz
float4 lightPosition[4]; //Posicion de la luz
float4 eyePosition; //Posicion de la camara
float lightIntensity[4]; //Intensidad de la luz
float lightAttenuation[4]; //Factor de atenuacion de la luz



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
	float3 AmbientLight : TEXCOORD3;
	float3 DiffuseLight : TEXCOORD4;
	float3 SpecularLight : TEXCOORD5;
};


//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP vs_DiffuseMap(VS_INPUT_DIFFUSE_MAP input)
{
	VS_OUTPUT_DIFFUSE_MAP output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuaci�n por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	float3 lightVec1 = lightPosition[0].xyz - output.WorldPosition;
	float3 lightVec2 = lightPosition[1].xyz - output.WorldPosition;
	float3 lightVec3 = lightPosition[2].xyz - output.WorldPosition;
	float3 lightVec4 = lightPosition[3].xyz - output.WorldPosition;

	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	float3 halfAngleVec1 = viewVector + lightVec1;
	float3 halfAngleVec2 = viewVector + lightVec2;
	float3 halfAngleVec3 = viewVector + lightVec3;
	float3 halfAngleVec4 = viewVector + lightVec4;

	//Normalizar vectores
	float3 Nn = normalize(output.WorldNormal);
	float3 Ln1 = normalize(lightVec1);
	float3 Ln2 = normalize(lightVec2);
	float3 Ln3 = normalize(lightVec3);
	float3 Ln4 = normalize(lightVec4);

	float3 Hn1 = normalize(halfAngleVec1);
	float3 Hn2 = normalize(halfAngleVec2);
	float3 Hn3 = normalize(halfAngleVec3);
	float3 Hn4 = normalize(halfAngleVec4);

	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten1 = length(lightPosition[0].xyz - output.WorldPosition) * lightAttenuation[0];
	float distAtten2 = length(lightPosition[1].xyz - output.WorldPosition) * lightAttenuation[1];
	float distAtten3 = length(lightPosition[2].xyz - output.WorldPosition) * lightAttenuation[2];
	float distAtten4 = length(lightPosition[3].xyz - output.WorldPosition) * lightAttenuation[3];

	//Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	float intensity1 = lightIntensity[0] / distAtten1;
	float intensity2 = lightIntensity[1] / distAtten2;
	float intensity3 = lightIntensity[2] / distAtten3;
	float intensity4 = lightIntensity[3] / distAtten4;

	//Componente Ambient
	float3 ambientLight = (intensity1 * lightColor[0] + intensity2 * lightColor[1] + intensity3 * lightColor[2] + intensity4 * lightColor[3])* materialAmbientColor;

	//Componente Diffuse: N dot L
	float3 n_dot_l1 = dot(Nn, Ln1);
	float3 n_dot_l2 = dot(Nn, Ln2);
	float3 n_dot_l3 = dot(Nn, Ln3);
	float3 n_dot_l4 = dot(Nn, Ln4);

	//Controlamos que no de negativo
	float3 diffuseLight1 = intensity1 * lightColor[0] * materialDiffuseColor.rgb * max(0.0, n_dot_l1);
	float3 diffuseLight2 = intensity2 * lightColor[1] * materialDiffuseColor.rgb * max(0.0, n_dot_l2);
	float3 diffuseLight3 = intensity3 * lightColor[2] * materialDiffuseColor.rgb * max(0.0, n_dot_l3);
	float3 diffuseLight4 = intensity4 * lightColor[3] * materialDiffuseColor.rgb * max(0.0, n_dot_l4);

	//Componente Specular: (N dot H)^exp
	float3 n_dot_h1 = dot(Nn, Hn1);
	float3 n_dot_h2 = dot(Nn, Hn2);
	float3 n_dot_h3 = dot(Nn, Hn3);
	float3 n_dot_h4 = dot(Nn, Hn4);

	float3 specularLight1 = n_dot_l1 <= 0.0
	? float3(0.0, 0.0, 0.0)
	: (intensity1 * lightColor[0] * materialSpecularColor * pow(max( 0.0, n_dot_h1), materialSpecularExp));
	float3 specularLight2 = n_dot_l2 <= 0.0
	? float3(0.0, 0.0, 0.0)
	: (intensity2 * lightColor[1] * materialSpecularColor * pow(max( 0.0, n_dot_h2), materialSpecularExp));
	float3 specularLight3 = n_dot_l3 <= 0.0
	? float3(0.0, 0.0, 0.0)
	: (intensity3 * lightColor[2] * materialSpecularColor * pow(max( 0.0, n_dot_h3), materialSpecularExp));
	float3 specularLight4 = n_dot_l4 <= 0.0
	? float3(0.0, 0.0, 0.0)
	: (intensity4 * lightColor[3] * materialSpecularColor * pow(max( 0.0, n_dot_h4), materialSpecularExp));

	output.AmbientLight = ambientLight;
	output.DiffuseLight = diffuseLight1 + diffuseLight2 + diffuseLight3 + diffuseLight4;
	output.SpecularLight = specularLight1 + specularLight2 + specularLight3 + specularLight4;

	return output;
}


//Input del Pixel Shader
struct PS_DIFFUSE_MAP
{
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
	float3 AmbientLight : TEXCOORD3;
	float3 DiffuseLight : TEXCOORD4;
	float3 SpecularLight : TEXCOORD5;
};

//Pixel Shader
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{
	//Obtener texel de la textura
	float4 texelColor = tex2D(diffuseMap, input.Texcoord);
	/*
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
	El color Alpha sale del diffuse material */
	float4 finalColor = float4(saturate(materialEmissiveColor + input.AmbientLight + input.DiffuseLight) * texelColor + input.SpecularLight, materialDiffuseColor.a);

	return finalColor;
}



/*
* Technique DIFFUSE_MAP
*/
technique DIFFUSE_MAP
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_DiffuseMap();
		PixelShader = compile ps_3_0 ps_DiffuseMap();
	}
}
