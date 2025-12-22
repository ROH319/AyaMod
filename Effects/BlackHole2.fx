// BlackHole.fx
sampler TextureSampler : register(s0);
float2 uScreenResolution; // 屏幕分辨率
// 常量参数
float2 blackHolePosition; // 黑洞中心位置(屏幕坐标,0-1)
float eventHorizonRadius; // 事件视界半径(屏幕比例)
float InfluenceRadius; // 引力影响半径
float DistortPower; // 扭曲强度
float GravitationalConstant; // 引力常数
float EdgeGlow; // 吸积盘亮度
float uTime; // 时间参数

float2 gravitationalDisplacement(float2 uv)
{
    float2 delta = uv - blackHolePosition;
    float r = length(delta);
    
    // 物理模型：位移与1/r成正比 (爱因斯坦偏转角公式)
    // 避免除以零，使用max保护
    float rr = max(r, eventHorizonRadius * 0.1);
    float displacement = DistortPower / (rr);
    
    // 归一化方向向量
    float2 dir = delta / max(r, 0.0001);
    
    return dir * displacement;
}

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    float2 uv = texCoord;
    float aspectRatio = (uScreenResolution.x / uScreenResolution.y); // 根据实际分辨率调整
    
    // 计算相对黑洞中心的坐标
    float2 dir = blackHolePosition - uv;
    dir.x *= aspectRatio; // 校正宽高比
    
    float r = length(dir);
    
    // 事件视界内部(黑色区域)
    if (r < eventHorizonRadius * 0.8)
    {
        return float4(0.0, 0.0, 0.0, 1.0);
    }
    
    // 吸积盘效果
    float diskMask = 1.0 - saturate((r - eventHorizonRadius * 0.8) / (eventHorizonRadius * 0.2));
    float3 diskColor = float3(1.0, 0.4, 0.1); // 橙色光晕
    
    // 加入动态噪声
    float time = uTime * 0.5;
    float noise = frac(sin(uv.x * 45.0 + time * 3.0) * 1200.0);
    noise += frac(sin(uv.y * 30.0 + time * 4.0) * 1000.0);
    diskColor *= 1.0 + noise * 0.3;
    
    //// 引力透镜效应
    //float displacement = gravitationalDisplacement(texCoord); /*DistortPower / max(r, eventHorizonRadius * 0.5);*/
    //// 计算被黑洞引力扭曲的背景UV
    //float2 distortedUV = texCoord - displacement;
    
    //// 采样背景并应用扭曲
    //float4 color = tex2D(TextureSampler, distortedUV);
    
    // 计算扭曲强度 (距离减小扭曲增加)
    float falloff = 1.0 - saturate(r / eventHorizonRadius / 1.5);
    float distortion = DistortPower * falloff * (1.0 / r);
    //float rr = max(r, eventHorizonRadius * 0.5);
    //float distortion = DistortPower / rr;
    float2 offset = normalize(dir) * distortion;
    
    float4 color = tex2D(TextureSampler, uv + offset);
    
    // 添加吸积盘效果
    //color += diskColor * diskMask * EdgeGlow * 4.0;
    
    // 事件视界边缘
    //float horizon = saturate((r - eventHorizonRadius * 0.9) / (eventHorizonRadius * 0.1));
    //color = lerp(color * 0.5, color, horizon);
    
    return color;
}

technique BlackHole
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
