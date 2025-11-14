sampler noise : register(s0);
sampler mask : register(s1);
sampler uImage0 : register(s2);

float2 uScreenResolution;
float2 pos;
float threshold = 0.9;
float alpha;

float4 PixelShaderMain(float4 samplerColor : COLOR0, float2 coord : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(mask, coord);
    float a = max(c.r, max(c.g, c.b));
    float2 screenPos = coord * uScreenResolution.xy;
    float4 resultColor = tex2D(uImage0, screenPos) * c.r;
    if (a < threshold)
    {
        float4 noiseColor = tex2D(noise, coord);
        resultColor *= noiseColor.r;
    }
    
    return resultColor * alpha;
}


// 技术定义
technique Basic
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderMain();
    }
}