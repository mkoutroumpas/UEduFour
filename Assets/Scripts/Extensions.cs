using Unity.Mathematics;

public static class Extensions
{
    public static float3 GetScale(this float4x4 matrix) =>
        new float3(math.length(matrix.c0.xyz), math.length(matrix.c1.xyz), math.length(matrix.c2.xyz));

    public static float GetUniformScale(this float4x4 matrix) =>
        (math.length(matrix.c0.xyz) == math.length(matrix.c1.xyz) &&
            math.length(matrix.c1.xyz) == math.length(matrix.c2.xyz)) ? math.length(matrix.c0.xyz) : default;
}
