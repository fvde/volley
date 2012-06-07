
//Texture Sampler
sampler Sampler1;

float2 blurDirection1;
float2 blurDirection2;
int index;

//Simple Pixel Shader
float4 PixelShaderFunction(float2 tex: TEXCOORD0, float4 col:COLOR0) : COLOR0
{
   
float4 SumColor = tex2D(Sampler1, tex);
if(SumColor.a < 0.1) return SumColor;

float2 BlurProportion = (0.004f, 0.0071f);
float BlurScale = 2.0f;
float BloomScale = 0.8f;
float blurDirection = 0.0f;
if(index == 0) blurDirection = blurDirection1;
if(index == 1) blurDirection = blurDirection2;

for (int i = 0; i < 6; i++)
	SumColor += tex2D(Sampler1, tex + BlurProportion * BlurScale * blurDirection * i);

SumColor.rgb = SumColor.rgb / 6 * BloomScale;
SumColor.a /= 6;

return SumColor*col;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }

}
