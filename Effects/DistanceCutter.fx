sampler uImage0 : register(s0);

float2 uScreenResolution;
float2 uCenter;
float radius;

struct PixelShaderInput
{
    float4 Position : SV_Position;
    float2 TextureCoordinate : TEXCOORD0;
    float4 Color : COLOR0;
};

float4 PixelShaderMain(PixelShaderInput input) : SV_Target
{
    float2 screenPosition = input.Position;
    if (distance(screenPosition, uCenter) > radius)
        return float4(0, 0, 0, 0);
    float4 c = tex2D(uImage0, input.TextureCoordinate);
    
    return c * input.Color;
}

technique DistanceCutter
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderMain();
    }
}