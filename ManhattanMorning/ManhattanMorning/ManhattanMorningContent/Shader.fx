
//Texture Sampler
sampler Sampler1;


//Simple Pixel Shader
float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{

float4 color = 0;
float2 samp = texCoord;

//Add 10 Color Values
for (int i = 0; i < 10; i++) {
        
		samp = texCoord - float2(0, 0.012f + i * 0.004f); 
        color += tex2D(Sampler1, samp);
    
}

//Calculate Mean-Color
color = color / 10.0f;

return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }

}
