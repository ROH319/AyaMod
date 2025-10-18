sampler uImage0 : register(s0);
float2 uImageSize;
float4 uColor;

float4 edge(float4 sampleColor : COLOR0, float2 coord: TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coord);
    if (any(color))
        return color * sampleColor;
    // 获取每个像素的正确大小
    float dx = 1 / uImageSize.x;
    float dy = 1 / uImageSize.y;
    bool flag = false;
    // 对周围8格进行判定
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            float4 c = tex2D(uImage0, coord + float2(dx * i, dy * j));
            // 如果任何一个像素有颜色
            if (any(c))
            {
                // 不知道为啥，这里直接return会被编译器安排，所以只能打标记了
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