sampler uImage0 : register(s0);
sampler uDreamImage : register(s1);
sampler uNoise : register(s2);
sampler uMask : register(s3);

float4 uScreenResolution;
float threshold = 0.9;
float alpha = 1;

struct PixelShaderInput
{
    float4 Position : SV_Position;
    float2 TextureCoordinate : TEXCOORD0;
    float4 Color : COLOR0;
};

float4 PixelShaderMain(PixelShaderInput input) : SV_Target
{
    float4 baseColor = tex2D(uImage0, input.TextureCoordinate);
    float4 maskColor = tex2D(uMask, input.TextureCoordinate);
    
    float2 screenCoord = input.Position.xy;
    
    float2 normalizedScreenCoord = float2(
        screenCoord.x / uScreenResolution.x,
        screenCoord.y / uScreenResolution.y
    );
    float4 mappedColor = tex2D(uDreamImage, normalizedScreenCoord) * maskColor.r;
    
    float a = max(maskColor.r, max(maskColor.g, maskColor.b));
    if (a < threshold)
    {
        float4 noiseColor = tex2D(uNoise, input.TextureCoordinate);
        //mappedColor *= noiseColor.r;
        mappedColor = lerp(mappedColor * noiseColor.r, mappedColor, maskColor.r);

    }
    
    return mappedColor * input.Color * alpha;
}
technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderMain();
    }
}