sampler uImage0 : register(s0); // ����������

float4 PixelShaderMain(float4 samplerColor : COLOR0, float2 coord : TEXCOORD0) : COLOR0
{
    return tex2D(uImage0, coord) * samplerColor;
}

// ��������
technique Basic
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderMain();
    }
}