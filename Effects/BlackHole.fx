#define MAX_BLACKHOLES 8  // 支持最多8个黑洞
sampler screenSource : register(s0); // 场景背景纹理
struct BlackHole
{
    float2 Position; //x,y:屏幕位置[0-1]
    float Mass; //质量
    float Radius; //引力范围
    float EventHorizonSize; //事件视界范围
    float RotationSpeed;
    float3 AccretionColor; // 吸积盘光晕颜色
    float GlowRadius; // 新增: 边缘光晕范围 (单位: 倍事件视界大小)
    float FalloffPower; // 衰减曲线指数
};
BlackHole Holes[MAX_BLACKHOLES];
int ActiveHolesCount;
float2 uScreenResolution; // 屏幕分辨率
float aspectRatio;
float uTime; // 时间参数
float glowm;

//const float DEFAULT_GLOW_RADIUS = 1.3;

// 1. 精简的引力向量计算
float2 CalculateGravityVector(float2 uv, int index)
{
    [branch]
    if (index >= ActiveHolesCount)
        return 0;
    
    BlackHole hole = Holes[index];
    float2 dir = hole.Position.xy - uv;
    
    // 使用简单缩放替代单独乘以纵横比
    dir.x *= aspectRatio;
    
    // 向量长度计算优化
    float distSq = dot(dir, dir);
    
    // 距离阈值检查（优化分支）
    if (distSq < (hole.EventHorizonSize * hole.EventHorizonSize))
    {
        // 视界内部 - 完全位移到中心（原始方向向量）
        return dir;
    }
    
    float dist = sqrt(distSq);
    float distInv = 1.0 / dist; // 预计算倒数
    
    // 视界比率预计算
    float horizonRatio = dist * (1.0 / hole.EventHorizonSize);
    
    // 优化衰减曲线计算（避免saturate）
    float ratio = dist * (1.0 / hole.Radius);
    float ratioDiff = max(0, min(ratio - 1.0, 1.0));
    float falloff = pow(1.0 - ratioDiff * 0.5, hole.FalloffPower);
    
    // 位移因子计算优化
    float displacementFactor = hole.EventHorizonSize * falloff / horizonRatio;
    displacementFactor = min(displacementFactor, dist * 0.99 * distInv); // 合并倒数使用
    
    return dir * (displacementFactor * distInv);
}
float GetEventHorizonFactor(float2 uv)
{
    float horizonFactor = 0.0;
    
    [loop]
    for (int i = 0; i < ActiveHolesCount; i++)
    {
        float2 dir = Holes[i].Position.xy - uv;
        dir.x *= aspectRatio;
        float dist = length(dir);
        
        float horizonSize = Holes[i].EventHorizonSize;
        float horizonBlend = 1.0 - smoothstep(horizonSize * 0.95, horizonSize * 1.05, dist);
        
        horizonFactor = max(horizonFactor, horizonBlend);

    }
    
    return horizonFactor;
}
// 精确吸积盘边缘光晕 - 填补视觉空隙
float3 GetAccretionGlow(float2 uv)
{
    float3 glow = float3(0, 0, 0);
    
    [loop]
    for (int i = 0; i < ActiveHolesCount; i++)
    {
        float2 dir = Holes[i].Position.xy - uv;
        dir.x *= aspectRatio;
        float dist = length(dir);
        float horizonSize = Holes[i].EventHorizonSize;
        
        // 关键区域: 事件视界外边缘范围
        if (dist < horizonSize * Holes[i].GlowRadius && dist > horizonSize * 0.95)
        {
            // 精确边缘光晕
            float ringPosition = (dist - horizonSize) / (horizonSize * 0.3);
            float glowIntensity = sin(ringPosition * 3.14159 * glowm) * 0.5 + 0.5;
            glowIntensity *= saturate(1.0 - abs(ringPosition) * 2.0);
            glowIntensity *= 0.7;
            
            // 使用吸积盘颜色增强
            float3 glowColor = Holes[i].AccretionColor * 2.0;
            glow = max(glow, glowColor * glowIntensity);
        }
    }
    
    return glow;
}
// 3. 高效位移计算（带指令优化）
float2 CalculateTotalDisplacement(float2 uv)
{
    float2 totalGravity = 0;
    float timeFactor = 1.0 + sin(uTime * 0.3) * 0.05;
    
    [loop]
    for (int i = 0; i < ActiveHolesCount; i++)
    {
        totalGravity += CalculateGravityVector(uv, i) * timeFactor;
    }
    
    return totalGravity;
}

// 4. 优化的主渲染函数
float4 main(float2 uv : TEXCOORD0) : SV_Target
{
    return float4(0, 0, 0, 0);
    // 事件视界检测
    float horizonFactor = GetEventHorizonFactor(uv);
    
    // 如果完全在事件视界内
    [branch]
    if (horizonFactor > 0.99)
    {
        return float4(0, 0, 0, 1);
    }
    // 计算位移并采样
    float2 displacement = CalculateTotalDisplacement(uv);
    float2 distorteduv = saturate(uv + displacement);
    float4 baseColor = tex2D(screenSource, distorteduv);
    
    // 基础RGB
    float3 rgb = baseColor.rgb;
    
    // 4. 边缘光晕填补空隙
    float3 accretionGlow = GetAccretionGlow(uv);
    rgb = rgb + accretionGlow;
    
    // 事件视界边缘过渡
    float transition = horizonFactor * horizonFactor; // 使用平方避免smoothstep
    rgb = lerp(rgb, float3(0, 0, 0), transition);
    
    return float4(rgb, baseColor.a);
}

// 技术结构体
technique Technique1
{
    pass BlackHole
    {
        PixelShader = compile ps_3_0 main();
    }
}
