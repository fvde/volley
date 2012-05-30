sampler firstSampler;
float2 targetSize;

float4 PixelShaderFunction(float2 tex: TEXCOORD0) : COLOR0
{
   float4 SumColor = tex2D(firstSampler, tex);
   float softness = 0.002f;

   SumColor += tex2D(firstSampler, tex + softness * float2(-1,0));
   SumColor += tex2D(firstSampler, tex + softness * float2(0,-1));
   SumColor += tex2D(firstSampler, tex + softness * float2(1,0));
   SumColor += tex2D(firstSampler, tex + softness * float2(0,1));
   SumColor /= 4.0f;
   
   return SumColor;
}

technique Technique1
{
   pass pass0
   {
      PixelShader = compile ps_2_0 PixelShaderFunction();
   }
} 