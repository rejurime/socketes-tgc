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

//Output del computeComponents
struct VS_OUTPUT_COMPUTE_COMPONENTS
{
	float3 Intensity;
	float3 DiffuseLight;
	float3 SpecularLight;
};

//Funcion para calcular los componentes de la luz
VS_OUTPUT_COMPUTE_COMPONENTS computeComponents(int i, float3 worldPosition, float3 viewVector, float3 Nn)
{
	VS_OUTPUT_COMPUTE_COMPONENTS output;

	//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	float3 lightVec = lightPosition[i].xyz - worldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	float3 halfAngleVec = viewVector + lightVec;

	//Normalizar vectores
	float3 Ln = normalize(lightVec);
	float3 Hn = normalize(halfAngleVec);

	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten = length(lightPosition[i].xyz - worldPosition) * lightAttenuation[i];

	//Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	output.Intensity = lightIntensity[i] / distAtten;

	//Componente Diffuse: N dot L
	float3 n_dot_l = dot(Nn, Ln);

	//Controlamos que no de negativo
	output.DiffuseLight = output.Intensity * lightColor[i] * materialDiffuseColor.rgb * max(0.0, n_dot_l);

	//Componente Specular: (N dot H)^exp
	float3 n_dot_h = dot(Nn, Hn);

	output.SpecularLight = n_dot_l <= 0.0	? float3(0.0, 0.0, 0.0)
	: (output.Intensity * lightColor[i] * materialSpecularColor * pow(max( 0.0, n_dot_h), materialSpecularExp));

	return output;
}

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

	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;

	//Normalizar vectores
	float3 Nn = normalize(output.WorldNormal);

	VS_OUTPUT_COMPUTE_COMPONENTS componentsLight1 = computeComponents(0, output.WorldPosition, viewVector, Nn);
	VS_OUTPUT_COMPUTE_COMPONENTS componentsLight2 = computeComponents(1, output.WorldPosition, viewVector, Nn);
	VS_OUTPUT_COMPUTE_COMPONENTS componentsLight3 = computeComponents(2, output.WorldPosition, viewVector, Nn);
	VS_OUTPUT_COMPUTE_COMPONENTS componentsLight4 = computeComponents(3, output.WorldPosition, viewVector, Nn);

	//Componente Ambient
	output.AmbientLight = (componentsLight1.Intensity * lightColor[0] + componentsLight2.Intensity * lightColor[1] +
		componentsLight3.Intensity * lightColor[2] + componentsLight4.Intensity * lightColor[3])* materialAmbientColor;

		output.DiffuseLight = componentsLight1.DiffuseLight + componentsLight2.DiffuseLight +
		componentsLight3.DiffuseLight + componentsLight4.DiffuseLight;

		output.SpecularLight = componentsLight1.SpecularLight + componentsLight2.SpecularLight +
		componentsLight3.SpecularLight + componentsLight4.SpecularLight;

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
		VertexShader = compile vs_3_0 vs_DiffuseMap();
		PixelShader = compile ps_3_0 ps_DiffuseMap();
	}
}
