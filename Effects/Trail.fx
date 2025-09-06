sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float timer;

float4x4 uTransform;

struct VSInput
{
    float2 Pos : POSITION0;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};


float4 PixelShaderFunction(PSInput input) : COLOR0
{
    float3 coord = input.Texcoord;
    float y = timer + coord.x; //��ֵ����ͼƬѭ������
    float4 c1 = tex2D(uImage1, float2(coord.x, coord.y)); //image1����״�����⣩��c1����ȡɫ
    float4 c3 = tex2D(uImage2, float2(y, coord.y)); //image2���ɰ�
    c1 *= c3;
    float4 c = tex2D(uImage0, float2(c1.r, 0)); //image0��ɫ��ͼ������c1.rҲ���Ǻڰ�ͼƬ������������ֵ�����ȸߵĵط���ӳ�䵽�ұߣ����ȵ;�ӳ�䵽���
    //if (c.r < 0.1)
    //    return float4(0, 0, 0, 0);
    return 2 * c * coord.z; //���������z��͸����
}

PSInput VertexShaderFunction(VSInput input)
{
    PSInput output;
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}


technique Technique1
{
    pass ColorBar
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}