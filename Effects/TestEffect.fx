sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float2 blackHolePosition; // 黑洞位置 (屏幕空间 [0,1])
float eventHorizonRadius; // 事件视界半径 (屏幕空间)
float accretionDiskRadius; // 吸积盘半径 (通常 > 3*事件视界半径)
float rotationSpeed; // 吸积盘旋转速度
float DistortPower;
float EdgeGlow;
float InfluenceRadius;
float GravitationalConstant;

float4 PixelFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 red = float4(0, 0, 1, 1);
    return color * 0.8 + red * 0.2;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelFunction();
    }
}