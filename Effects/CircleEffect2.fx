#define PI 3.14159265358979323846
#define TWO_PI (2.0 * PI)

sampler noiseTex : register(s0);
float uTime;
float innerRadius;
float Radius;
float4 edgeColor;
float4 innerColor;
float twistIntensity;
float maskFactor;

float2 CartesianToPolar(float2 center, float2 uv)
{
    float2 relative = uv - center;
    float radius = length(relative);
    float angle = atan2(relative.y, relative.x);
    angle = angle < 0.0 ? angle + TWO_PI : angle;
    return float2(radius, angle);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : SV_Target
{
    coords = (coords * Radius - coords * Radius % 2) / Radius;
    float2 RTheta = CartesianToPolar(float2(0.5, 0.5), coords);
    
    float2 polarUV = float2(RTheta.y / PI * 0.5 + 0.5, RTheta.x + uTime);
    
    float2 noiseValue = (tex2D(noiseTex, polarUV).xy - float2(0.5, 0.5)) * twistIntensity;
    
    float2 newuv = coords + noiseValue / Radius;
    
    RTheta = CartesianToPolar(float2(0.5, 0.5), newuv);
    polarUV = float2(RTheta.y / PI * 0.5 + 0.5, RTheta.x);
    
    float currentLength = polarUV.y * Radius;
    if(currentLength < innerRadius - 2)
        return innerColor * (maskFactor + polarUV.y / 0.5 * (1 - maskFactor));
    if(currentLength < innerRadius)
        return edgeColor;
    
    return float4(0, 0, 0, 0);
}
technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};