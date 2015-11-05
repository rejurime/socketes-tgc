/*
* Shader generico para TgcSkeletalMesh con iluminacion dinamica por pixel (Phong Shading)
* utilizando un tipo de luz Point-Light con atenuacion por distancia
* Hay 2 Techniques, una para cada MeshRenderType:
*	- VERTEX_COLOR
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

//Matrix Pallette para skinning
static const int MAX_MATRICES = 26;
float4x3 bonesMatWorldArray[MAX_MATRICES];

//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialAmbientColor; //Color RGB
float4 materialDiffuseColor; //Color ARGB (tiene canal Alpha)
float3 materialSpecularColor; //Color RGB
float materialSpecularExp; //Exponente de specular

//Parametros de las Luces
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
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
	float3 Normal :   NORMAL0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
	float4 BlendWeights : BLENDWEIGHT;
	float4 BlendIndices : BLENDINDICES;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 WorldTangent	: TEXCOORD2;
	float3 WorldBinormal : TEXCOORD3;
	float3 WorldPosition : TEXCOORD4;
	float3 AmbientLight : TEXCOORD5;
	float3 DiffuseLight : TEXCOORD6;
	float3 SpecularLight : TEXCOORD7;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP vs_DiffuseMap(VS_INPUT_DIFFUSE_MAP input)
{
	VS_OUTPUT_DIFFUSE_MAP output;

	//Pasar indices de float4 a array de int
	int BlendIndicesArray[4] = (int[4])input.BlendIndices;

	//Skinning de posicion
	float3 skinPosition = mul(input.Position, bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;;
	skinPosition += mul(input.Position, bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinPosition += mul(input.Position, bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinPosition += mul(input.Position, bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;

	//Skinning de normal
	float3 skinNormal = mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;
	skinNormal += mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinNormal += mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinNormal += mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;
	output.WorldNormal = normalize(skinNormal);

	//Skinning de tangent
	float3 skinTangent = mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;
	skinTangent += mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinTangent += mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinTangent += mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;
	output.WorldTangent = normalize(skinTangent);

	//Skinning de binormal
	float3 skinBinormal = mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;
	skinBinormal += mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinBinormal += mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinBinormal += mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;
	output.WorldBinormal = normalize(skinBinormal);

	//Proyectar posicion (teniendo en cuenta lo que se hizo por skinning)
	output.Position = mul(float4(skinPosition.xyz, 1.0), matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuaci�n por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

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
	float3 Hn2 = normalize(halfAngleVec1);
	float3 Hn3 = normalize(halfAngleVec1);
	float3 Hn4 = normalize(halfAngleVec1);

	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten1 = length(lightPosition[0].xyz - output.WorldPosition) * lightAttenuation[0];
	//Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	float intensity1 = lightIntensity[0] / distAtten1;

	float distAtten2 = length(lightPosition[1].xyz - output.WorldPosition) * lightAttenuation[1];
	//Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	float intensity2 = lightIntensity[1] / distAtten2;

	float distAtten3 = length(lightPosition[2].xyz - output.WorldPosition) * lightAttenuation[2];
	//Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	float intensity3 = lightIntensity[2] / distAtten3;

	float distAtten4 = length(lightPosition[3].xyz - output.WorldPosition) * lightAttenuation[3];
	//Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	float intensity4 = lightIntensity[3] / distAtten4;

	//Componente Ambient
	float3 ambientLight = (intensity1 * lightColor[0] + intensity2 * lightColor[1] + intensity3 * lightColor[2] + intensity4 * lightColor[3])* materialAmbientColor;

	//Componente Diffuse: N dot L
	float3 n_dot_l1 = dot(Nn, Ln1);
	//Controlamos que no de negativo
	float3 diffuseLight1 = intensity1 * lightColor[0] * materialDiffuseColor.rgb * max(0.0, n_dot_l1);

	float3 n_dot_l2 = dot(Nn, Ln1);
	//Controlamos que no de negativo
	float3 diffuseLight2 = intensity2 * lightColor[1] * materialDiffuseColor.rgb * max(0.0, n_dot_l2);

	float3 n_dot_l3 = dot(Nn, Ln1);
	//Controlamos que no de negativo
	float3 diffuseLight3 = intensity3 * lightColor[2] * materialDiffuseColor.rgb * max(0.0, n_dot_l3);

	float3 n_dot_l4 = dot(Nn, Ln1);
	//Controlamos que no de negativo
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
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 WorldPosition : TEXCOORD4;
	float3 AmbientLight : TEXCOORD5;
	float3 DiffuseLight : TEXCOORD6;
	float3 SpecularLight : TEXCOORD7;
};

//Pixel Shader
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{
	//Obtener texel de la textura
	float4 texelColor = tex2D(diffuseMap, input.Texcoord);

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
		VertexShader = compile vs_2_0 vs_DiffuseMap();
		PixelShader = compile ps_2_0 ps_DiffuseMap();
	}
}
