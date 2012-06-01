

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
            float2( 0.0,   0.0),
            float2( 0.002, 0.0 ),
            float2(-0.002, 0.00355),
            float2( 0.00 , 0.00355),
            float2( 0.002, 0.00355),
};
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

float4 DarkenEdgesPS(float2 tex: TEXCOORD0) : COLOR0
{
	float4 col = tex2D(TextureSamplerY, tex.xy);
	float4 dest = tex2D(TextureSampler, tex.xy);
	if(col.a < 0.1) return dest;

	float alpha = 1;

	for (int i=0; i < NUM; i++) {
      col = tex2D(TextureSamplerY, tex.xy + c2[i]*1.5);
	  alpha *= col.a;
    }
	if(alpha < 0.1) return float4(0,0,0,1);

	return col;
}

technique SobelEdgeFilter
{
	pass Pass0
	{   
		PixelShader  = compile ps_2_0 SobelEdgeFilterPS();
	}
}

technique DarkenEdges
{
	pass Pass0
	{   
		PixelShader  = compile ps_2_0 DarkenEdgesPS();
	}
}