void GetCrossSampleUVs_float(
        float4 UV,
        float2 TextelSize,
        float OffsetMultiplier,
        out float2 UVOriginal,
        out float2 UVTopRight,
        out float2 UVBottomLeft,
        out float2 UVTopLeft,
        out float2 UBBottomRight)
{
    UVOriginal = UV;
    UVTopRight = UV.xy + TextelSize.xy * OffsetMultiplier;
    UVBottomLeft = UV.xy - TextelSize.xy * OffsetMultiplier;
    UVTopLeft = UV.xy + float2(-TextelSize.x, TextelSize.y) * OffsetMultiplier;
    UBBottomRight = UV.xy + float2(TextelSize.x, -TextelSize.y) * OffsetMultiplier;
}

void GetCrossSampleUVs_half(
        float4 UV,
        float2 TextelSize,
        float OffsetMultiplier,
        out float2 UVOriginal,
        out float2 UVTopRight,
        out float2 UVBottomLeft,
        out float2 UVTopLeft,
        out float2 UBBottomRight)
{
    UVOriginal = UV;
    UVTopRight = UV.xy + TextelSize.xy * OffsetMultiplier;
    UVBottomLeft = UV.xy - TextelSize.xy * OffsetMultiplier;
    UVTopLeft = UV.xy + float2(-1.0 * TextelSize.x, TextelSize.y) * OffsetMultiplier;
    UBBottomRight = UV.xy + float2(TextelSize.x, -1.0 * TextelSize.y) * OffsetMultiplier;
}