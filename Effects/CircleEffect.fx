#define PI 3.14159265358979323846
#define TWO_PI (2.0 * PI)
#define HALF_PI (0.5 * PI)

float4x4 MatrixTransform;
sampler Texture0 : register(s0);
float PixelWidth;
float WidthInPixel;
float ScreenWidth;
float segment;
float threshold;
float rad;

struct PixelShaderInput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
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

float4 PixelShaderFunction(PixelShaderInput input) : SV_TARGET
{
    // 计算纹理空间与屏幕空间的转换因子
    float pixelSizeInTexSpace = 1.0 / ScreenWidth;
    float actualRingWidth = max(0.001, PixelWidth * pixelSizeInTexSpace);
    
    // 计算圆心距离和尺寸
    float2 center = float2(0.5, 0.5);
    float dist = distance(input.TexCoord, center);
    float radius = 0.5; // 最大半径(正方形中心到边缘)
    
    float2 polar = CartesianToPolar(center, input.TexCoord);
    
    // 计算圆环边界
    float outerBound = radius;
    float innerBound = outerBound - actualRingWidth;
    float feather = pixelSizeInTexSpace; // 1像素的抗锯齿过渡
    
    // 计算内部边界过渡
    float innerAlpha = smoothstep(innerBound - feather, innerBound + feather, dist);
    if (polar.x * rad < rad / 2 - WidthInPixel)
        return float4(0, 0, 0, 0);
    
    float angle = polar.y / TWO_PI;
    float part = 1 / segment;
    float value = angle % part;
    if (abs(value - part / 2) > part / 2 * threshold)
        return float4(0, 0, 0, 0);
    
    // 计算外部边界过渡（反转smoothstep）
    float outerAlpha = 1.0 - smoothstep(outerBound - feather, outerBound + feather, dist);
    
    // 合并得到圆环Alpha
    float ringAlpha = innerAlpha * outerAlpha;
    
    float4 color = tex2D(Texture0, input.TexCoord);
    float4 ringColor = float4(1, 1, 1, ringAlpha);
    //ringAlpha *= color.a; // 保留原始纹理的透明通道
    //color.a = ringAlpha * input.Color.a; // 应用顶点颜色
    
    
    return color * input.Color * ringColor;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};