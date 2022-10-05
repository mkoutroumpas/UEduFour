using Unity.Entities;

public struct Rotator : IComponentData
{
    public float Speed;
    public float Angle;
}

public struct Scaler : IComponentData
{
    public float From;
    public float To;
    public float Speed;
    public float Scale;
}