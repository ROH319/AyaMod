sampler uImage0 : register(s0);
float2 uImageSize;
float4 uColor;

float4 edge(float4 sampleColor : COLOR0, float2 coord: TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coord);
    if (any(color))
        return color * sampleColor;
    // ��ȡÿ�����ص���ȷ��С
    float dx = 1 / uImageSize.x;
    float dy = 1 / uImageSize.y;
    bool flag = false;
    // ����Χ8������ж�
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            float4 c = tex2D(uImage0, coord + float2(dx * i, dy * j));
            // ����κ�һ����������ɫ
            if (any(c))
            {
                // ��֪��Ϊɶ������ֱ��return�ᱻ���������ţ�����ֻ�ܴ�����
                flag = true;
            }
        }
    }
    if (flag)
        return uColor * 0.2 + uColor * sampleColor * 0.8;
    return color * sampleColor;
}
technique Technique1
{
    pass Edge
    {
        PixelShader = compile ps_2_0 edge();
    }
}