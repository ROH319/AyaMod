
texture uMask;// 遮罩纹理
texture uCloud; // 云纹理
texture uColorMap; // 色度图

sampler uImage0 : register(s0); // 主纹理输入

sampler2D maskTex = sampler_state
{
    texture = <uMask>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
};
sampler2D colorMapTex = sampler_state
{
    texture = <uColorMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = clamp;
    AddressV = clamp; //循环UV
};
sampler2D cloudTex = sampler_state
{
    texture = <uCloud>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap; //循环UV
};

float preMultR;
float flowx;
int multx;
float thresholdY;
float maskFactor;
float2 cloudScale;
float2 cloudOffset;
float uTime;
float uTime2;

struct PixelShaderInput
{
    float4 Position : SV_Position;
    float2 TextureCoordinate : TEXCOORD0;
    float4 Color : COLOR0;
};

// 直角坐标转极坐标方法
float2 RectToPolar(float2 uv, float2 centerUV)
{
    uv = uv - centerUV; //改变中心 将中心从UV左下角移到UV中心
    float theta = atan2(uv.y, uv.x); // atan()值域[-π/2, π/2]一般不用; atan2()值域[-π, π]确定一个完整的圆
    float r = length(uv); //UV上的某一点到我们确定的中心得距离
    return float2(theta, r);
}
float getLerpValue(float from, float to, float t, bool clamped = false)
{
    float value = (t - from) / (to - from);
    return clamped ? clamp(value, 0, 1) : value;
}

float remap(float fromValue, float fromMin, float fromMax, float toMin, float toMax, bool clamped = true)
{
    return lerp(toMin, toMax, getLerpValue(fromMin, fromMax, fromValue, clamped));
}

float4 PixelShaderMain(PixelShaderInput input) : SV_Target
{
    // 1. 直角坐标转极坐标
    float2 thetaR = RectToPolar(input.TextureCoordinate, float2(0.5, 0.5));
    
    // 2. 极坐标UV映射
    float thetaUV = (thetaR.x + flowx) / 3.141593 * (0.5 * multx) + (0.5 * multx);// θ从[-π, π]映射到[0, 1]
    float2 polarUV = float2(thetaUV, thetaR.y);
    
    // 3. 主纹理采样（时间流动融合）
    float4 mainColor = tex2D(uImage0, float2(polarUV.x, polarUV.y + frac(uTime)));
    
    // 4. 云纹理采样（缩放+偏移+时间流动）
    float2 cloudUV = cloudOffset + polarUV * cloudScale;
    cloudUV.y += uTime2;
    float4 cloudColor = tex2D(cloudTex, cloudUV);
    
    // 5. 遮罩纹理采样
    float normalR = clamp(remap(polarUV.y, thresholdY, 0.5, 0, 0.5) / 0.5, 0.0, 1.0);
    float4 maskColor = tex2D(maskTex, float2(polarUV.x, normalR));
    
    // 6. 纹理混合 + 遮罩叠加
    float4 finalColor = lerp(mainColor, cloudColor, maskFactor) * maskColor.r;
    
    // 7. 颜色映射采样 + 最终输出
    float4 mapColor = tex2D(colorMapTex, float2(finalColor.r * preMultR, 0.5));
    
    return mapColor * input.Color;
}

// 技术定义
technique Basic
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderMain();
    }
}