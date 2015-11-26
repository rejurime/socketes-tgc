// ---------------------------------------------------------
// Sombras en el image space con la tecnica de Shadows Map
// ---------------------------------------------------------

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))
float4x4 matViewProj; //View * Projection
float lightIntensity; //Intensidad de la luz
float lightAttenuation; //Factor de atenuacion de la luz


//Matrix Pallette para skinning
static const int MAX_MATRICES = 26;
float4x3 bonesMatWorldArray[MAX_MATRICES];

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

float4x4 g_mViewLightProj;
float4x4 g_mProjLight;
float3   g_vLightPos;  // posicion de la luz (en World Space) = pto que representa patch emisor Bj
float3   g_vLightDir;  // Direcion de la luz (en World Space) = normal al patch Bj


//-----------------------------------------------------------------------------
// Vertex Shader para dibujar la escena pp dicha con sombras
//-----------------------------------------------------------------------------

//Vertex Shader
void VertexShadows(float4 iPos : POSITION , out float4 oPos : POSITION, out float Intensity : TEXCOORD4)
{
	float3 v = mul(iPos, matWorld);
	float k = v.y / (g_vLightPos.y - v.y);

	v = v + (v - g_vLightPos) * k;
	v.y = 0;

	// transformo al screen space
	oPos = mul(float4(v , 1), matViewProj);

	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten = length(g_vLightPos.xyz - mul(iPos, matWorld)) * lightAttenuation;

	//Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	Intensity = lightIntensity / distAtten;
}

//-----------------------------------------------------------------------------
// Vertex Shader para los Skeletal mesh
//-----------------------------------------------------------------------------

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
	float3 WorldTangent : TEXCOORD2;
	float3 WorldBinormal : TEXCOORD3;
	float Intensity : TEXCOORD4;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP VertexMeshShadows(VS_INPUT_DIFFUSE_MAP input)
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
	//output.Position = mul(float4(skinPosition.xyz, 1.0), matWorldViewProj);
	float3 v = mul( float4(skinPosition.xyz, 1.0), matWorld);
	float k = v.y / (g_vLightPos.y-v.y);

	v = v + (v-g_vLightPos)*k;
	v.y = 0;

	// transformo al screen space
	output.Position = mul( float4(v , 1), matViewProj );

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten = length(g_vLightPos.xyz - mul(input.Position, matWorld)) * lightAttenuation;

	//Dividimos intensidad sobre distancia (lo hacemos lineal pero tambien podria ser i/d^2)
	output.Intensity = lightIntensity / distAtten;

	return output;
}

	//Input del Pixel Shader
	struct INPUT_PS
	{
		float4 Position : POSITION0;
		float4 Color : COLOR0;
		float2 Texcoord : TEXCOORD0;
		float3 WorldNormal : TEXCOORD1;
		float3 WorldTangent : TEXCOORD2;
		float3 WorldBinormal : TEXCOORD3;
		float Intensity : TEXCOORD4;
	};

//-----------------------------------------------------------------------------
// Pixel Shader para dibujar la sombras
//-----------------------------------------------------------------------------
float4 PixelShadows(INPUT_PS input) :COLOR
{
	float alpha = clamp(input.Intensity, 0.1, 0.9);
	return float4(0,0,0, alpha);
}

//-----------------------------------------------------------------------------
// Techniques
//-----------------------------------------------------------------------------
technique RenderShadows
{
	pass p0
	{
		AlphaBlendEnable=True;
		VertexShader = compile vs_3_0 VertexShadows();
		PixelShader = compile ps_3_0 PixelShadows();
	}
}

technique RenderMeshShadows
{
	pass p0
	{
		AlphaBlendEnable=True;
		VertexShader = compile vs_3_0 VertexMeshShadows();
		PixelShader = compile ps_3_0 PixelShadows();
	}
}
