#define PI 3.14159265358979323846
#define TWO_PI (2.0 * PI)
#define HALF_PI (0.5 * PI)

texture uShape;
texture uNoise;
float extraU;
float extraV;
float scale;
float borderWidth;
float borderFeatherWidth;
float borderRadius;
float size;
bool drawInner;

float4 borderInnerColor;
float4 borderColor;

sampler uImage0 : register(s0);
sampler uMask : register(s1);
sampler uMask2 : register(s2);
sampler uGradient : register(s3);

sampler2D shapeTex = sampler_state
{
    texture = <uShape>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //Ñ­»·UV
};
sampler2D noiseTex = sampler_state
{
    texture = <uNoise>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //Ñ­»·UV
};

struct PixelShaderInput
{
    float4 Position : SV_Position;
    float2 TextureCoordinate : TEXCOORD0;
    float4 Color : COLOR0;
};

float2 CartesianToPolar(float2 center, float2 uv)
{
    float2 relative = uv - center;
    float radius = length(relative);
    float angle = atan2(relative.y, relative.x);
    angle = angle < 0.0 ? angle + TWO_PI : angle;
    return float2(radius, angle);
}

float remap(float t, float fromMin, float fromMax, float toMin, float toMax)
{
    return (t - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
}

float InExpo(float t, float min, float max)
{
    float x = remap(t, min, max, 0, 1);
    return pow(2.0, (10 * (x / 1 - 1)));
}

float4 PixelShaderMain(PixelShaderInput input) : SV_Target
{
    float4 baseColor = tex2D(uImage0, input.TextureCoordinate);
    float4 maskColor = tex2D(uMask, input.TextureCoordinate);
    
    float4 flowColor = tex2D(shapeTex, input.TextureCoordinate * scale + float2(extraU, extraV));
    
    float4 gradientColor = drawInner ? tex2D(uGradient, float2(flowColor.r, 1)) : float4(0, 0, 0, 0);
    
    float2 polar = CartesianToPolar(float2(0.5, 0.5), input.TextureCoordinate);
    
    float dx = 1 / size;
    
    if (polar.x < borderRadius - dx * borderFeatherWidth)
    {
        return gradientColor * input.Color;
    }
    if (polar.x < borderRadius)
    {
        float factor = InExpo(polar.x, borderRadius - dx * borderFeatherWidth, borderRadius);
        return lerp(gradientColor, borderColor, factor) * input.Color;
    }
    if (polar.x < borderRadius + dx * borderWidth)
    {
        return borderInnerColor * input.Color;
    }
    if (polar.x < borderRadius + dx * (borderWidth + borderFeatherWidth))
    {
        float factor = InExpo(polar.x - borderRadius, dx * (borderWidth + borderFeatherWidth), dx * borderWidth);
        return lerp(float4(0, 0, 0, 0), borderColor, factor) * input.Color;
    }
    
    
    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderMain();
    }
}