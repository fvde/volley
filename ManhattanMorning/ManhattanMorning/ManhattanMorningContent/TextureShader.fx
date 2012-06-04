
struct VertexToPixel
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
    float LightingFactor: TEXCOORD0;
    float2 TextureCoords: TEXCOORD1;
};

struct VertexShaderInput 
{ 
    float4 Position : POSITION0; 
    float2 TextureCoords: TEXCOORD0;
    float4 Color: COLOR0;
};

//------- Constants --------
float4x4 worldViewProj;
float4x4 worldMatrix;
float4x4 projectionMatrix;
float4x4 viewMatrix;
const static int NUM = 9;
const float2 c[9] = {
            float2(-0.000781, -0.001388), 
            float2( 0.0 ,     -0.002777),
            float2( 0.000781, -0.001388),
            float2(-0.015625, 0.0 ),
            float2( 0.0,       0.0),
            float2( 0.015625, 0.0 ),
            float2(-0.000781,0.001388),
            float2( 0.0 ,0.002777),
            float2( 0.000781,0.001388),
};

//------- Texture Samplers --------

Texture xTexture;
Texture yTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = Wrap; AddressV = Wrap;};
sampler TextureSamplerY : register(s1) = sampler_state { texture = <yTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = Wrap; AddressV = Wrap;};
float4 tintingColor;

//------- Technique: Textured --------
VertexToPixel TexturedVS( VertexShaderInput VSIn)
{	
	VertexToPixel Output = (VertexToPixel)0;	
    
	//Output.Position = mul(VSIn.Position , worldViewProj);
	float4x4 preViewProjection = mul (viewMatrix, projectionMatrix);
	float4x4 preWorldViewProjection = mul (worldMatrix, preViewProjection);
    
	Output.Position = mul(VSIn.Position, preWorldViewProjection);	
	Output.TextureCoords = VSIn.TextureCoords;
    
	return Output;
}

float4 TexturedPS(VertexToPixel PSIn) : COLOR0
{
	float4 color = tex2D(TextureSampler, PSIn.TextureCoords);
	return color;
}


//------- Technique: Toon --------
float4 ToonPS(VertexToPixel PSIn) : COLOR0
{	
	float4 color = 0;
	
	for(int i=0;i<NUM;i++)
	{
		color += tex2D(TextureSampler, PSIn.TextureCoords + c[i]);
	}

	color /= NUM;
	color *= 8;
	color = ceil(color);
	color /= 8;

	return color;
}

//------- Technique: Light --------
float4 LightPS(float2 tex: TEXCOORD0, float4 col:COLOR0) : COLOR0
{	
	float4 color = tex2D(TextureSamplerY, tex);
	float4 light = tex2D(TextureSampler, tex);

	color *= tintingColor;
	color *= clamp((1/tintingColor)*light, 1, 1000);

	return col*(color+light);
}

//------- Technique: Water--------



float4 FadeOutWaterPS(float2 tex: TEXCOORD0, float4 col:COLOR0) : COLOR0
{	
	float2 mappedTex;
	mappedTex.x = tex.x *50/40.0f;
	mappedTex.y = tex.y* 640/50.0f ; // Magic Number DAFUQ???
	float4 color = tex2D(TextureSamplerY,mappedTex);
	float4 light = tex2D(TextureSampler, tex);


	return light*color.a;

}


//------- Technique: Shadow --------
float4 ShadowPS(VertexToPixel PSIn) : COLOR0
{	
	float4 color = tex2D(TextureSampler, PSIn.TextureCoords);    
	color *= float4(0,0,0,1);

	return color;
}

technique MapTexture
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 TexturedVS();
		PixelShader  = compile ps_2_0 TexturedPS();
	}
}

technique ShadowTexture
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 TexturedVS();
		PixelShader  = compile ps_2_0 ShadowPS();
	}
}

technique TexturedToon
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 TexturedVS();
		PixelShader  = compile ps_2_0 ToonPS();
	}
}

technique Light
{
	pass Pass0
	{
		PixelShader  = compile ps_2_0 LightPS();
	}
}

technique WaterFadeOut
{
	pass Pass0
	{
		PixelShader  = compile ps_2_0 FadeOutWaterPS();
	}
}