

//------- Constants --------
const static int NUM = 9;
const float threshold = 0.01;
const float2 c[9] = {
            float2(-0.00090625, -0.0016111), 
            float2( 0.00 ,     -0.0016111),
            float2( 0.00090625, -0.0016111),
            float2(-0.00090625, 0.0 ),
            float2( 0.0,       0.0),
            float2( 0.00090625, 0.0 ),
            float2(-0.00090625,0.0016111),
            float2( 0.00 ,    0.0016111),
            float2( 0.00090625,0.0016111),
};
const float2 c2[9] = {
            float2(-0.002,-0.00355), 
            float2( 0.00 ,-0.00355),
            float2( 0.002,-0.00355),
            float2(-0.002, 0.0 ),
            float2( 0.00,   0.0),
            float2( 0.002, 0.0 ),
            float2(-0.002, 0.00355),
            float2( 0.00 , 0.00355),
            float2( 0.002, 0.00355),
};
const float gaussW[9] = { 1,2,1,2,4,2,1,2,1};
const float3 rgb2lum = float3(0.30, 0.59, 0.11);

//------- Texture Samplers --------
Texture yTexture;
sampler TextureSampler;
sampler TextureSamplerY : register(s1) = sampler_state { texture = <yTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = Clamp; AddressV = Clamp;};

float4 SobelEdgeFilterPS(float2 tex: TEXCOORD0) : COLOR0
{
	float4 col[NUM];

	for (int i=0; i < NUM; i++) {
      col[i] = tex2D(TextureSampler, tex.xy + c[i]);
    }

	if(col[4].a <0.2) return col[4];

	float lum[NUM];

	for (i = 0; i < NUM; i++) {
      lum[i] = dot(pow(col[i].xyz,2), rgb2lum);
    }

    float x = lum[2]+  lum[8]+2*lum[5]-lum[0]-2*lum[3]-lum[6];
    float y = lum[6]+2*lum[7]+  lum[8]-lum[0]-2*lum[1]-lum[2];

    float edge =(x*x + y*y < threshold)? 0.0:1.0;

	float4 OUT;

    OUT.xyz = col[4].xyz * edge.xxx;
    OUT.w = 1.0;

	return OUT;
}

float4 GaussFilterPS(float2 tex: TEXCOORD0) : COLOR0
{
	float4 col = tex2D(TextureSampler, tex.xy);
	
	col = float4(0,0,0,0);
	for (int i=0; i < NUM; i++) {
		col += (tex2D(TextureSampler, tex.xy + c2[i]*0.4) * gaussW[i]);
	}
	return col / 16;
}

float4 DarkenEdgesPS(float2 tex: TEXCOORD0) : COLOR0
{
	float4 col = tex2D(TextureSampler, tex.xy);
	int c = 0;

	for (int i=0; i < NUM; i++) {
      col = tex2D(TextureSampler, tex.xy + c2[i]*0.75);
	  if(dot(col,float4(1,1,1,0)) < 0.1) c++;
    }

	if(c == 9) return float4(0,0,0,0);
	if(c > 6 && c <=8) return float4(0,0,0,1);
	return tex2D(TextureSampler, tex.xy);
}

float4 InterpolateColorPS(float2 tex: TEXCOORD0) : COLOR0
{
	float4 col = tex2D(TextureSamplerY, tex.xy);

	if(dot(col,float4(1,1,1,0)) < 0.1) return float4(0,0,0,0);

	float4 dest = float4(0,0,0,0);
	for (int i=0; i < NUM; i++) {
      dest += tex2D(TextureSamplerY, tex.xy + c2[i]*1);
    }

	return dest / 9;
}



technique SobelEdgeFilter
{
	pass Pass0
	{   
		PixelShader  = compile ps_2_0 SobelEdgeFilterPS();
	}
}

technique GaussFilter
{
	pass Pass0
	{   
		PixelShader  = compile ps_2_0 GaussFilterPS();
	}
}

technique DarkenEdges
{
	pass Pass0
	{   
		PixelShader  = compile ps_2_0 DarkenEdgesPS();
	}
	pass Pass1
	{
		AlphaBlendEnable = true;
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;
		PixelShader  = compile ps_2_0 InterpolateColorPS();
	}
}