sampler uImage0 : register(s0); // 主纹理输入

float4 PixelShaderMain(float4 samplerColor : COLOR0, float2 coord : TEXCOORD0) : COLOR0
{
    return tex2D(uImage0, coord) * samplerColor;
}

// 技术定义
technique Basic
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderMain();
    }
}