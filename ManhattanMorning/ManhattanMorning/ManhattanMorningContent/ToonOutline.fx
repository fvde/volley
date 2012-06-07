
struct VertexToPixel
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
    float LightingFactor: TEXCOORD0;
    float2 TextureCoords: TEXCOORD1;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};

//------- Constants --------
float4x4 worldViewProj;
int shadingLevels;

const static int NUM = 9;
const float2 c[9] = {
            float2(-0.003625, -0.0064444), 
            float2( 0.0 ,     -0.0064444),
            float2( 0.003625, -0.0064444),
            float2(-0.003625, 0.0 ),
            float2( 0.0,       0.0),
            float2( 0.003625, 0.0 ),
            float2(-0.003625,0.0064444),
            float2( 0.0 ,    0.0064444),
            float2( 0.003625,0.0064444),
};

//------- Texture Samplers --------
Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = Clamp; AddressV = Clamp;};


//------- Technique: TexturedCell --------
VertexToPixel CellVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0)
{	
	VertexToPixel Output = (VertexToPixel)0;
    
	Output.Position = mul(inPos, worldViewProj);
	Output.TextureCoords = inTexCoords;
    
	return Output;    
}

PixelToFrame CellPS(float2 PSIn:TEXCOORD0) 
{
	PixelToFrame Output = (PixelToFrame)0;		
	
	Output.Color = tex2D(TextureSampler, PSIn);
	
	Output.Color *= shadingLevels;
	Output.Color = ceil(Output.Color);
	Output.Color /= shadingLevels;

	return Output;
}

technique Cell
{
	pass Pass0
	{   
		//VertexShader = compile vs_2_0 CellVS();
		PixelShader  = compile ps_2_0 CellPS();
	}
}

//------- Technique: Outline --------

float4 OutlinePS(float2 PSIn: TEXCOORD0, float4 color:COLOR0) : COLOR0
{	
	float4 Output = 0.0f;
	float4 col[NUM];
	int edge = 0;

	for (int i=0; i < NUM; i++) {
      col[i] = tex2D(TextureSampler, PSIn + c[i]);
	  if(col[i].a < 0.2)
	  {
	  edge++;
	  }
    }
	
	if(edge <= 4 && edge > 1) Output.rgb = 0.0;
	else{
	 Output = col[4];
	 }
	
	return Output*color;
}

technique Outline
{
	pass Pass0
	{   
		PixelShader = compile ps_2_0 OutlinePS();
	}
}