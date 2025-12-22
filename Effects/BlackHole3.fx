#define MAX_BLACKHOLES 8  // 支持最多8个黑洞
sampler screenSource : register(s0); // 场景背景纹理
struct BlackHole
{
    float2 Position;//x,y:屏幕位置[0-1]
    float Mass;//质量
    float Radius;//引力范围
    float EventHorizonSize;//事件视界范围
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


float2 CalculateGravityVector(float2 uv, int index)
{
    if (index >= ActiveHolesCount)
        return float2(0, 0);
    BlackHole hole = Holes[index];
    float2 dir = hole.Position.xy - uv;
    dir.x *= aspectRatio;//应用宽高比
    float dist = length(dir);
    
    dist = max(dist, 0.0001);
    dir = dir / dist;
    
    
    float falloff = 1.0 - saturate(dist / hole.Radius);
    float gravity = hole.Mass * falloff / (dist * dist * hole.FalloffPower);
    gravity = clamp(gravity, 0, hole.EventHorizonSize);
    return dir * gravity;
}

float2 CalculateTotalDisplacement(float2 uv)
{
    float2 totalGravity = float2(0, 0);
    [loop]
    for (int i = 0; i < ActiveHolesCount; i++)
    {
        float2 gravityVec = CalculateGravityVector(uv, i);
        totalGravity += gravityVec;
    }
    return totalGravity * (1.0 + sin(uTime * 0.3) * 0.05);
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

// 主渲染函数
float4 main(float2 uv : TEXCOORD0) : SV_Target
{
    //float aspectRatio = (uScreenResolution.x / uScreenResolution.y);
    
    float horizonFactor = GetEventHorizonFactor(uv);
    
    if (horizonFactor > 0.99)
        return float4(0, 0, 0, 1);

    float2 displacement = CalculateTotalDisplacement(uv);
    
    float2 distorteduv = uv + displacement;
    
    float4 baseColor = tex2D(screenSource, distorteduv);
    
    float3 rgb = baseColor.rgb;
    
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
