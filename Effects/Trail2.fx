
texture uNoise; // 遮罩纹理
texture uCloud; // 云纹理
texture uColorMap; // 色度图

sampler uImage0 : register(s0); // 主纹理输入

sampler2D noiseTex = sampler_state
{
    texture = <uNoise>;
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

float maskScale;
float2 maskOffset;
float cloudScale;
float2 cloudOffset;
float distortIntensity;
float preMultR = 1;
float colorMult = 2;
float timer;

struct PixelShaderInput
{
    float4 Position : SV_Position;
    float2 TextureCoordinate : TEXCOORD0;
    float4 Color : COLOR0;
};


float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float2 coord = input.TextureCoordinate;
    float4 mainColor = tex2D(uImage0, coord.xy); //image1是形状，c1就是取色
    if(mainColor.r < 0.05)
        return float4(0, 0, 0, 0);
    
    float4 maskColor = tex2D(noiseTex, float2(coord.xy * maskScale + maskOffset));
    float2 distort = maskColor.rg;
    
    float2 cloudUV = coord.xy * cloudScale + cloudOffset;
    float4 cloudColor = tex2D(cloudTex, lerp(cloudUV, cloudUV + distort, distortIntensity)); //cloud是云彩纹理
    mainColor = mainColor * cloudColor;
    
    float4 c = tex2D(colorMapTex, float2(mainColor.r * preMultR, 0)); //image0是色度图，用了c1.r也就是黑白图片的亮度来做插值，亮度高的地方就映射到右边，亮度低就映射到左边
    //if (c.r < 0.1)
    //    return float4(0, 0, 0, 0);
    return colorMult * c * input.Color;
}

technique Technique1
{
    pass ColorBar
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}